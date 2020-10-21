using AbleSync.Core;
using AbleSync.Core.Extensions;
using AbleSync.Core.Host.BackgroundServices;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using AbleSync.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
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
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();

        /// <summary>
        ///     Creates the host builder.
        /// </summary>
        /// <param name="args">Command line argument.</param>
        /// <returns>The created host builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services);
                });

        /// <summary>
        ///     Configure our service collection.
        /// </summary>
        /// <param name="services">The instantiated collection.</param>
        /// <returns></returns>
        private static void ConfigureServices(IServiceCollection services)
        {
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

            // Add queue manager as singleton.
            services.AddSingleton<QueueManager>();

            // Add hosted services.
            services.AddHostedService<PeriodicScrapingBackgroundService>();
            services.AddHostedService<PeriodicAnalyzingBackgroundService>();
            services.AddHostedService<QueueManagerBackgroundService>();
        }
    }
}
