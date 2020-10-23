using AbleSync.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace AbleSync.Core.Host.BackgroundServices
{
    // TODO Move to different project?
    /// <summary>
    ///     Hosted service for periodically scraping the
    ///     configured root directory.
    /// </summary>
    /// <remarks>
    ///     TODO Is this correct?
    ///     The <see cref="IProjectScrapingService"/> will perform
    ///     both asynchronous operations (project syncing) and IO
    ///     work, meaning sysnchronous operations. Hence this is run
    ///     in it's own thread.
    /// </remarks>
    public sealed class PeriodicScrapingBackgroundService : PeriodicBackgroundService<PeriodicScrapingBackgroundService>
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PeriodicScrapingBackgroundService(IServiceProvider provider,
            ILogger<PeriodicScrapingBackgroundService> logger)
            : base(TimeSpan.FromSeconds(20), logger)
            => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        // TODO Async voids are dangerous.
        /// <summary>
        ///     Call the scraper to analyze the entire root folder.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        protected override async void DoPeriodicWork(CancellationToken token)
        {
            try
            {
                using var scope = _provider.CreateScope();

                var service = scope.ServiceProvider.GetService<IProjectScrapingService>();
                await service.ProcessRootDirectoryRecursivelyAsync(token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
