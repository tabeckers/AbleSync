﻿using AbleSync.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace AbleSync.Core.Host.BackgroundServices
{
    // TODO Make timed hosted service abstract class?
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
        private readonly Timer _timer;
        private readonly IProjectScrapingService _projectScrapingService;
        private readonly ILogger<PeriodicScrapingBackgroundService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PeriodicScrapingBackgroundService(IProjectScrapingService projectScrapingService,
            ILogger<PeriodicScrapingBackgroundService> logger)
            : base(TimeSpan.FromSeconds(20), logger)
        {
            _projectScrapingService = projectScrapingService ?? throw new ArgumentNullException(nameof(projectScrapingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // TODO Async voids are dangerous.
        /// <summary>
        ///     Call the scraper to analyze the entire root folder.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        protected override async void DoPeriodicWork(CancellationToken token)
        {
            try
            {
                await _projectScrapingService.ProcessRootDirectoryRecursivelyAsync(token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}