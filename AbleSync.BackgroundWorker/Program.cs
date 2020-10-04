using AbleSync.Core;
using AbleSync.Core.Extensions;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
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
            services.AddLogging();

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
            // TODO Is this correct?
            services.AddScoped<IConfiguration>(_ => configuration);

            // Setup actual configuration.
            services.Configure<AbleSyncOptions>(options => configuration.GetSection("AbleSyncOptions").Bind(options));
            services.Configure<DbProviderOptions>(config =>
            {
                config.ConnectionStringName = "DatabaseInternal";
            });

            // Run the service.
            // TODO This is a work in progress and should be done using a periodic background service.
            var provider = services.BuildServiceProvider();
            var scraper = provider.GetRequiredService<IProjectScrapingService>();

            using var cts = new CancellationTokenSource();
            await scraper.ProcessRootDirectoryRecursivelyAsync(cts.Token);
        }
    }
}
