using AbleSync.Core.Extensions;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using AbleSync.Infrastructure.Storage;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AbleSync.Api
{
    /// <summary>
    ///     Configures all our services.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public Startup(IConfiguration configuration)
            => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>
        ///     Configure our service collection.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers.
            services.AddControllers();

            // Configure AbleSync services.
            services.AddAbleSyncInfrastructureServices();

            // Setup actual configuration.
            services.Configure<BlobStorageOptions>(options => _configuration.GetSection("BlobStorage").Bind(options));
            services.Configure<DbProviderOptions>(config =>
            {
                config.ConnectionStringName = "DatabaseInternal";
            });

            // Configure mapping.
            services.AddAutoMapper(typeof(MapperProfile));
        }

        /// <summary>
        ///     Configure our pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // TODO What does pathstring do? Might be useful
            app.UsePathBase(new PathString("/api"));
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
