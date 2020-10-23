using AbleSync.Core;
using AbleSync.Core.Extensions;
using AbleSync.Core.Host.BackgroundServices;
using AbleSync.Core.Managers;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using AbleSync.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
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
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var basePath = Directory.GetCurrentDirectory();

                    // If we specified a config location, use that.
                    basePath = GetValueOrNull(args, "--configdir") ?? basePath;

                    config.SetBasePath(basePath);
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables(prefix: "ABLESYNC_");
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((hostContext, config) =>
                {
                    config.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services, hostContext.Configuration);
                });

        /// <summary>
        ///     Configure our service collection.
        /// </summary>
        /// <param name="services">The instantiated collection.</param>
        /// <param name="configuration">The configuration collection.</param>
        /// <returns></returns>
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add required services.
            services.AddAbleSyncCoreServices();
            services.AddAbleSyncInfrastructureServices();

            services.AddOptions();

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

        /// <summary>
        ///     Attempts to extract a value from the command line args. If the
        ///     requested value isn't present this returns null.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The dashes are expected to be present in <paramref name="key"/>.
        ///     </para>
        ///     <para>
        ///         This does not consider invalid command line arguments, for
        ///         example when the --key is specified without the value. The
        ///         application will crash on startup if this is the case.
        ///     </para>
        /// </remarks>
        /// <param name="args">Command line args.</param>
        /// <param name="key">The key to get.</param>
        /// <returns>The value or null if it isn't present.</returns>
        private static string GetValueOrNull(string[] args, string key)
        {
            if (args.Length == 0)
            {
                return null;
            }

            if (args.Where(x => x.Equals(key)).Any())
            {
                var keyIndex = Array.IndexOf(args, key);
                return args[keyIndex + 1];
            }

            return null;
        }
    }
}
