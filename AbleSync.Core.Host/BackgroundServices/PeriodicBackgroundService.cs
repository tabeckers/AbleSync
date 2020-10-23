using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Host.BackgroundServices
{
    // TODO How do we pass the interval? () => .. = ..?
    /// <summary>
    ///     Abstract base class for a timer based
    ///     hosted service.
    /// </summary>
    public abstract class PeriodicBackgroundService<TLoggerType> : IHostedService, IDisposable
    {
        protected readonly ILogger<TLoggerType> _logger;
        private readonly TimeSpan _timerTimeSpan;
        private Timer _timer;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PeriodicBackgroundService(TimeSpan timerTimeSpan, ILogger<TLoggerType> logger)
        {
            // TODO Validate TimeSpan
            _timerTimeSpan = timerTimeSpan;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Start the service and trigger periodic execution
        ///     of <see cref="DoPeriodicWork"/>.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken token)
        {
            _timer = new Timer(PeriodicWorkWrapper, token, TimeSpan.Zero, _timerTimeSpan);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Function that will be called periodically
        ///     by our timer.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        protected abstract void DoPeriodicWork(CancellationToken token);

        /// <summary>
        ///     Stop the service.
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

        /// <summary>
        ///     Wrapper that parses the cancellation token and
        ///     calls <see cref="DoPeriodicWork"/>.
        /// </summary>
        /// <param name="state">Should be the cancellation token.</param>
        private void PeriodicWorkWrapper(object state)
        {
            var token = (CancellationToken)state;

            DoPeriodicWork(token);
        }
    }
}
