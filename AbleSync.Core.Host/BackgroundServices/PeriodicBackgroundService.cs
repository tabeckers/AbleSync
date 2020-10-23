using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Host.BackgroundServices
{
    /// <summary>
    ///     Abstract base class for a timer based
    ///     hosted service.
    /// </summary>
    /// <remarks>
    ///     The default <see cref="_timerTimeSpan"/> value is 1 hour.
    /// </remarks>
    /// <typeparam name="TServiceType">Service class that implements this abstract base class.</typeparam>
    public abstract class PeriodicBackgroundService<TServiceType> : IHostedService, IDisposable
    {
        /// <summary>
        ///     The class name of <typeparamref name="TServiceType"/>.
        /// </summary>
        public string ServiceName => GetType().Name;

        protected readonly ILogger<TServiceType> _logger;
        private uint count = 0;
        private TimeSpan _timerTimeSpan = TimeSpan.FromHours(1);
        private Timer _timer;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PeriodicBackgroundService(ILogger<TServiceType> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        ///     Start the service and trigger periodic execution
        ///     of <see cref="DoPeriodicWork"/>.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken token)
        {
            _logger.LogInformation($"Starting periodic service for {ServiceName} with interval {_timerTimeSpan}");

            _timer = new Timer(PeriodicWorkWrapper, token, TimeSpan.Zero, _timerTimeSpan);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Function that will be called periodically
        ///     by our timer.
        /// </summary>
        /// <remarks>
        ///     Logging the start, finish and count of each cycle is
        ///     already implemented within this base class.
        /// </remarks>
        /// <param name="token">The cancellation token.</param>
        protected abstract void DoPeriodicWork(CancellationToken token);

        /// <summary>
        ///     Stop the service.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public Task StopAsync(CancellationToken token)
        {
            _logger.LogInformation($"Stopping periodic service for {ServiceName}");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Called on graceful shutdown.
        /// </summary>
        public void Dispose()
            => _timer?.Dispose();

        // TODO Is this the way to go?
        /// <summary>
        ///     Sets the timer timespan for this periodic service.
        /// </summary>
        /// <remarks>
        ///     This has to be set at object creation, otherwise it will
        ///     have no effect.
        /// </remarks>
        /// <param name="timerTimeSpan"></param>
        protected void SetInterval(TimeSpan timerTimeSpan)
        {
            if (_timer != null)
            {
                throw new InvalidOperationException($"Service of type {ServiceName} is already running");
            }
            if (timerTimeSpan.TotalSeconds == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timerTimeSpan));
            }

            _timerTimeSpan = timerTimeSpan;
        }

        /// <summary>
        ///     Wrapper that parses the cancellation token and
        ///     calls <see cref="DoPeriodicWork"/>.
        /// </summary>
        /// <param name="state">Should be the cancellation token.</param>
        private void PeriodicWorkWrapper(object state)
        {
            count++;

            _logger.LogInformation($"Starting periodic work cycle {count} for {ServiceName}");


            var token = (CancellationToken)state;

            DoPeriodicWork(token);

            _logger.LogTrace($"Finished periodic work cycle {count} for {ServiceName}");
        }
    }
}
