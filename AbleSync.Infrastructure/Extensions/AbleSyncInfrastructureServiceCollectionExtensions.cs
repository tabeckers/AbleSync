using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Infrastructure.Provider;
using AbleSync.Infrastructure.Repositories;
using AbleSync.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AbleSync.Infrastructure.Extensions
{
    /// <summary>
    ///     Extensions functionality for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class AbleSyncInfrastructureServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds all our infrastructure services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>Same as <paramref name="services"/>.</returns>
        public static IServiceCollection AddAbleSyncInfrastructureServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Add custom db provider.
            services.AddScoped<DbProvider, NpgsqlDbProvider>();

            // Add repositories.
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();

            // TODO Split for file storage.
            // TODO Options.
            services.AddSingleton<IBlobStorageService, SpacesBlobStorageService>();

            return services;
        }
    }
}
