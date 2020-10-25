using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Host.BackgroundServices
{
    // TODO Is this correct use of DI?
    // TODO This should be event based, not polling.
    /// <summary>
    ///     Wrapper to launch <see cref="QueueManager"/> so 
    ///     it can be accessed through the DI.
    /// </summary>
    public sealed class QueueManagerBackgroundService : IHostedService, IDisposable
    {
        private Timer _timer;

        private readonly QueueManager _queueManager;
        private readonly IServiceProvider _provider;
        private readonly ILogger<QueueManagerBackgroundService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public QueueManagerBackgroundService(QueueManager queueManager,
            IServiceProvider provider,
            ILogger<QueueManagerBackgroundService> logger)
        {
            _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Start this hosted service.
        /// </summary>
        /// <remarks>
        ///     This will periodically check the queue.
        /// </remarks>
        /// <param name="token">The cancellation token.</param>
        public Task StartAsync(CancellationToken token)
        {
            _timer = new Timer(CheckQueue, token, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        // TODO Async void is dangerous
        /// <summary>
        ///     Check if we have items in the queue.
        /// </summary>
        /// <param name="state">Should be <see cref="CancellationToken"/>.</param>
        private async void CheckQueue(object state)
        {
            try
            {
                if (state == null)
                {
                    throw new ArgumentNullException(nameof(state));
                }

                // TODO Find different solution. Store internally?
                var token = (CancellationToken)state;

                // TODO Race condition? Use DequeueOrNull?
                if (_queueManager.GetCount() == 0)
                {
                    return;
                }

                using var scope = _provider.CreateScope();
                var projectTaskExecuterService = scope.ServiceProvider.GetService<IProjectTaskExecuterService>();

                var item = _queueManager.Dequeue();

                _logger.LogTrace($"Passing item {item.Id} to project task executer service");

                await projectTaskExecuterService.ProcessProjectTaskAsync(item, token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        /// <summary>
        ///     Stop the hosted service gracefully.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public Task StopAsync(CancellationToken token)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Called on graceful shutdown.
        /// </summary>
        public void Dispose()
            => _timer?.Dispose();
    }
}
