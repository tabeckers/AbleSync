using AbleSync.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;

namespace AbleSync.Core.Host.BackgroundServices
{
    // TODO Move to different project?
    /// <summary>
    ///     Periodically calls the <see cref="IProjectAnalyzingService"/>.
    /// </summary>
    /// <remarks>
    ///     Logging is handled by the base class.
    /// </remarks>
    public sealed class PeriodicAnalyzingBackgroundService : PeriodicBackgroundService<PeriodicAnalyzingBackgroundService>
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PeriodicAnalyzingBackgroundService(IServiceProvider provider,
            IOptions<AbleSyncOptions> options,
            ILogger<PeriodicAnalyzingBackgroundService> logger)
            : base(logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            var interval = options?.Value?.IntervalAnalyzingMinutes ?? throw new ArgumentNullException(nameof(options));
            SetInterval(TimeSpan.FromMinutes(interval));
        }

        // TODO Async voids are dangerous.
        /// <summary>
        ///     Get all projects and process each of them using
        ///     the <see cref="IProjectAnalyzingService"/>. The
        ///     results are then enqueued in the queue manager.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        protected override async void DoPeriodicWork(CancellationToken token)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var projectAnalyzingService = scope.ServiceProvider.GetService<IProjectAnalyzingService>();

                await projectAnalyzingService.AnalyzeAllProjectsEnqueueTasksAsync(token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
