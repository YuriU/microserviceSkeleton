using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebApi.Middleware
{
    public static class SwaggerV2WithSecurityExtensionsBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerV2WithSecurityExtensions(
            this IApplicationBuilder app,
            Action<SwaggerV2WithSecurityExtensionsOptions> setupAction = null)
        {
            SwaggerV2WithSecurityExtensionsOptions swaggerV2Options = new SwaggerV2WithSecurityExtensionsOptions();
            if (setupAction != null)
                setupAction(swaggerV2Options);
            else
                swaggerV2Options = app.ApplicationServices.GetRequiredService<IOptions<SwaggerV2WithSecurityExtensionsOptions>>().Value;
            app.UseMiddleware<SwaggerV2WithSecurityExtensionsMiddleware>((object) swaggerV2Options);
            return app;
        }
    }
}