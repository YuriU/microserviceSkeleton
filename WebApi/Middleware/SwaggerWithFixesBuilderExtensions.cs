using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebApi.Middleware
{
    public static class SwaggerWithFixesBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerWithFixes(
            this IApplicationBuilder app,
            Action<SwaggerWithFixesOptions> setupAction = null)
        {
            SwaggerWithFixesOptions swaggerOptions = new SwaggerWithFixesOptions();
            if (setupAction != null)
                setupAction(swaggerOptions);
            else
                swaggerOptions = app.ApplicationServices.GetRequiredService<IOptions<SwaggerWithFixesOptions>>().Value;
            app.UseMiddleware<SwaggerWithFixesMiddleware>((object) swaggerOptions);
            return app;
        }
    }
}