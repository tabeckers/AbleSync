using AbleSync.Core;
using AbleSync.Core.Extensions;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using AbleSync.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.BackgroundWorker
{
    /// <summary>
    ///     Application entry class.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // Add required services.
            services.AddAbleSyncCoreServices();
            services.AddAbleSyncInfrastructureServices();

            // Setup utility.
            // TODO The config doesn't enfore the level
            services.AddLogging(options =>
            {
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Trace);
            });

            // Add configuration.
            // TODO Make environment dependent.
            // TODO Clean up directory mess.
            var basePath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath.FullName)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            services.AddScoped<IConfiguration>(_ => configuration);

            // Setup actual configuration.
            services.Configure<AbleSyncOptions>(options => configuration.GetSection("AbleSyncOptions").Bind(options));
            services.Configure<BlobStorageOptions>(options => configuration.GetSection("BlobStorage").Bind(options));
            services.Configure<DbProviderOptions>(config =>
            {
                config.ConnectionStringName = "DatabaseInternal";
            });

            var provider = services.BuildServiceProvider();
           
            // Run the scraper.
            var scraper = provider.GetRequiredService<IProjectScrapingService>();
            using var cts = new CancellationTokenSource();
            await scraper.ProcessRootDirectoryRecursivelyAsync(cts.Token);

            // Run the project task processor.
            var processor = provider.GetRequiredService<IProjectTaskProcessingService>();
            var projectRepository = provider.GetRequiredService<IProjectRepository>();
            var projectTaskRepository = provider.GetRequiredService<IProjectTaskRepository>();

            using var cts2 = new CancellationTokenSource();

            var project = await projectRepository.GetAsync(new Guid("48640e0a-ea95-4d4e-acda-b1cfb44d3296"), cts2.Token);
            var projectTask = (await projectTaskRepository.GetAllForProjectAsync(project.Id, cts2.Token).ToListAsync()).First();

            await processor.ProcessProjectTaskAsync(project, projectTask, cts2.Token);
        }
    }
}
