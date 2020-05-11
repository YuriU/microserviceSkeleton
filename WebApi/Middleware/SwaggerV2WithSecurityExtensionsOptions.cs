using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace WebApi.Middleware
{
    public class SwaggerV2WithSecurityExtensionsOptions
    {
        public SwaggerV2WithSecurityExtensionsOptions()
        {
            this.PreSerializeFilters = new List<Action<OpenApiDocument, HttpRequest>>();
            this.AuthSchemaToExtensions = new Dictionary<string, Dictionary<string, string>>();
        }

        /// <summary>
        /// Sets a custom route for the Swagger JSON endpoint(s). Must include the {documentName} parameter
        /// </summary>
        public string RouteTemplate { get; set; } = "swagger/{documentName}/swagger.json";

        /// <summary>
        /// Actions that can be applied SwaggerDocument's before they're serialized to JSON.
        /// Useful for setting metadata that's derived from the current request
        /// </summary>
        public List<Action<OpenApiDocument, HttpRequest>> PreSerializeFilters { get; private set; }
        
        /// <summary>
        /// Contains extensions by security schema
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> AuthSchemaToExtensions { get; private set; }
    }
}