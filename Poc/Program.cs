using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Poc
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var root = new Uri(@"C:\Users\thoma\AbleSync Testing Area\Baaki\v1\");
            var directoryInfo = new DirectoryInfo(root.LocalPath);

            IServiceCollection services = new ServiceCollection();
            
            services.AddScoped<IProjectScrapingService, ProjectScrapingService>();
            services.AddScoped<IFileTrackingService, FileTrackingService>();
            services.AddScoped<IProjectRepository, ProjectRepositoryMock>();

            services.AddLogging(config =>
            {
                config.SetMinimumLevel(LogLevel.Trace);
            });

            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IProjectScrapingService>();

            await service.ProcessDirectoryRecursivelyAsync(directoryInfo);
        }
    }
}
