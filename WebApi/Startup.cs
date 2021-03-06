using System;
using System.Collections.Generic;
using Autofac;
using Cache;
using Cache.Redis;
using Contracts.Donation;
using Data.Repository;
using Data.Repository.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Services.Donations;
using Swashbuckle.AspNetCore.SwaggerUI;
using WebApi.Middleware;
using WebApi.Settings;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Built in container setup
        public void ConfigureServices(IServiceCollection services)
        {
            var authConfiguration = new AuthSettings();
            Configuration.GetSection("Auth").Bind(authConfiguration);
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Name = "Authorization",
                    Flows = new OpenApiOAuthFlows 
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{authConfiguration.ExternalIdentityUrl}/token"),
                            AuthorizationUrl = new Uri($"{authConfiguration.ExternalIdentityUrl}/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "token" }
                            }
                        },
                    },
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                        },
                        new List<string>() { }
                    }
                });
            });
            
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = authConfiguration.Authority;
                    options.Audience = authConfiguration.Audience;
                    options.RequireHttpsMetadata = false;
                });
        }

        // Autofac container setup
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(container =>
            {
                var configuration = container.Resolve<IConfiguration>();
                var databaseSettings = new DatabaseSettings();
                configuration.Bind("Database", databaseSettings);
                return databaseSettings;
            }).As<DatabaseSettings>();

            builder.Register(container =>
            {
                var settings = container.Resolve<DatabaseSettings>();
                DonationsDbContext cx = new DonationsDbContext(c =>
                {
                    c.UseNpgsql(settings.ConnectionString,
                        b => b.MigrationsAssembly(typeof(Program).Assembly.FullName));
                });
                return cx; 
            });
            
            builder.RegisterType<DonationService>().As<IDonationService>();
            builder.RegisterType<DonationsRepository>().As<IDonationsRepository>();
            
            builder.Register(container =>
            {
                var configuration = container.Resolve<IConfiguration>();
                var databaseSettings = new CacheSettings();
                configuration.Bind("Cache", databaseSettings);
                return databaseSettings;
            })
            .As<CacheSettings>()
            .SingleInstance();

            // TODO: Split up multiplexor and cache service objects, because they have different lifetime
            builder.Register(container =>
            {
                var cacheSettings = container.Resolve<CacheSettings>();
                return new CacheService(cacheSettings.ConnectionString);
            })
            .As<ICacheService>()
            .SingleInstance();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var authConfiguration = new AuthSettings();
            Configuration.GetSection("Auth").Bind(authConfiguration);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // To make it to connect to OpenId, we need to replace access_token to id_token
            app.UseSwaggerV2WithSecurityExtensions(c =>
            {
                c.AuthSchemaToExtensions.Add("oauth2", new Dictionary<string, string>()
                {
                    { "x-tokenName", "id_token" } 
                });
            });
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.OAuthClientId(authConfiguration.ClientId);
                c.OAuthClientSecret(authConfiguration.ClientSecretId);
            });

            var option = new RewriteOptions();
            
            option.AddRedirect("^$", "swagger");
            
            app.UseRewriter(option);
            
            app.UseHttpsRedirection();
            
            app.UseAuthentication();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
