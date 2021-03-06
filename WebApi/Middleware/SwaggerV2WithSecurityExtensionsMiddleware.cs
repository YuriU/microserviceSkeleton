﻿using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApi.Middleware
{
  public class SwaggerV2WithSecurityExtensionsMiddleware
  {
      private readonly RequestDelegate _next;
      private readonly SwaggerV2WithSecurityExtensionsOptions _options;
      private readonly TemplateMatcher _requestMatcher;

      public SwaggerV2WithSecurityExtensionsMiddleware(RequestDelegate next, SwaggerV2WithSecurityExtensionsOptions options)
      {
        this._next = next;
        this._options = options ?? new SwaggerV2WithSecurityExtensionsOptions();
        this._requestMatcher = new TemplateMatcher(TemplateParser.Parse(this._options.RouteTemplate), new RouteValueDictionary());
      }

    public async Task Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
    {
      string documentName;
      if (!this.RequestingSwaggerDocument(httpContext.Request, out documentName))
      {
        await this._next(httpContext);
      }
      else
      {
        string basePath = !string.IsNullOrEmpty((string) httpContext.Request.PathBase) ? httpContext.Request.PathBase.ToString() : (string) null;
        try
        {
          OpenApiDocument swagger = swaggerProvider.GetSwagger(documentName, (string) null, basePath);
          foreach (Action<OpenApiDocument, HttpRequest> preSerializeFilter in this._options.PreSerializeFilters)
            preSerializeFilter(swagger, httpContext.Request);
          await this.RespondWithSwaggerJson(httpContext.Response, swagger);
        }
        catch (UnknownSwaggerDocument ex)
        {
          this.RespondWithNotFound(httpContext.Response);
        }
      }
    }

    private bool RequestingSwaggerDocument(HttpRequest request, out string documentName)
    {
      documentName = (string) null;
      if (request.Method != "GET")
        return false;
      RouteValueDictionary values = new RouteValueDictionary();
      if (!this._requestMatcher.TryMatch(request.Path, values) || !values.ContainsKey(nameof (documentName)))
        return false;
      documentName = values[nameof (documentName)].ToString();
      return true;
    }

    private void RespondWithNotFound(HttpResponse response)
    {
      response.StatusCode = 404;
    }

    /// <summary>
    /// Updating JObject, adding extensions
    /// </summary>
    /// <param name="jObject"></param>
    private void AddSecuritySchemaExtensions(JObject jObject)
    {
      var securityDefinitions = jObject["securityDefinitions"];

      foreach (var securityDefinition in securityDefinitions)
      {
        if (securityDefinition.Type == JTokenType.Property)
        {
          var securityDefinitionProperty = (JProperty)securityDefinition;
          var name = securityDefinitionProperty.Name;
          var value = securityDefinitionProperty.Value as JObject;

          // If extensions for security schema exists
          if (this._options.AuthSchemaToExtensions.TryGetValue(name, out var extensions) && value != null)
          {
            // Apply extensions
            foreach (var extension in extensions)
            {
              value[extension.Key] = extension.Value;    
            }
          }
        }
      }
    }
    
    private async Task RespondWithSwaggerJson(HttpResponse response, OpenApiDocument swagger)
    {
      response.StatusCode = 200;
      response.ContentType = "application/json;charset=utf-8";

      using (var memoryStream = new MemoryStream())
      using (var streamWriter = new StreamWriter(memoryStream))
      {
          // 1. Writing OpenApiDocument to MemoryStream
          OpenApiJsonWriter openApiJsonWriter = new OpenApiJsonWriter((TextWriter) streamWriter);
          swagger.SerializeAsV2((IOpenApiWriter) openApiJsonWriter);
          streamWriter.Flush();
          memoryStream.Seek(0l, SeekOrigin.Begin);
          
          // 2. Reading memory stream to construct JObject
          var streamReader = new StreamReader(memoryStream);
          JsonReader jsonReader = new JsonTextReader(streamReader);

          var jObject = JObject.Load(jsonReader);
          
          // 3. Apply changes to JObject
          AddSecuritySchemaExtensions(jObject);
          
          // 4. Write updated JObject to response
          await response.WriteAsync(JsonConvert.SerializeObject(jObject, Formatting.Indented), (Encoding) new UTF8Encoding(false), new CancellationToken());
      }
    }
  }
}