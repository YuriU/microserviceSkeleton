using Autofac;
using Cache;
using Cache.Redis;
using Contracts.Donation;
using Data.Repository;
using Data.Repository.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Services.Donations;

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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
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
            app.UseSwagger();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
