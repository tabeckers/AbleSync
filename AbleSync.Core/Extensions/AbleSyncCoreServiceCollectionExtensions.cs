using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.ProjectTaskExecuters;
using AbleSync.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AbleSync.Core.Extensions
{
    /// <summary>
    ///     Extensions functionality for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class AbleSyncCoreServiceCollectionExtensions
    {
        // TODO Move
        /// <summary>
        ///     Adds all our infrastructure services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>Same as <paramref name="services"/>.</returns>
        public static IServiceCollection AddAbleSyncBackgroundWorkerServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO Look at service lifetimes.

            // Add services.
            services.AddScoped<IProjectScrapingService, ProjectScrapingService>();
            services.AddScoped<IProjectAnalyzingService, ProjectAnalyzingService>();
            services.AddScoped<IProjectTaskExecuterService, ProjectTaskExecuterService>();
            services.AddScoped<ITrackingFileService, TrackingFileService>();

            // Add task executors
            services.AddScoped<UploadAudioExecuter>();

            return services;
        }

        /// <summary>
        ///     Adds all our infrastructure services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>Same as <paramref name="services"/>.</returns>
        public static IServiceCollection AddAbleSyncCoreServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO Look at service lifetimes.

            // Add services.
            services.AddScoped<IAudioFileService, AudioFileService>();

            return services;
        }
    }
}
