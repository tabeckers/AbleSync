using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AbleSync.Api
{
    /// <summary>
    ///     Application main entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Application entry function.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        /// <summary>
        ///     Creates our webhost.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>The web host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
