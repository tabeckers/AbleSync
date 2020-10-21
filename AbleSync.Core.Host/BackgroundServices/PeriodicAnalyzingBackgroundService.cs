﻿using AbleSync.Core.Host.Exceptions;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;

namespace AbleSync.Core.Host.BackgroundServices
{
    /// <summary>
    ///     Periodically calls the <see cref="IProjectAnalyzingService"/>.
    /// </summary>
    public sealed class PeriodicAnalyzingBackgroundService : PeriodicBackgroundService<PeriodicAnalyzingBackgroundService>
    {
        private readonly QueueManager _queueManager;
        private readonly IProjectAnalyzingService _projectAnalyzingService;
        private readonly IProjectRepository _projectRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PeriodicAnalyzingBackgroundService(QueueManager queueManager,
            IProjectAnalyzingService projectAnalyzingService,
            IProjectRepository projectRepository,
            ILogger<PeriodicAnalyzingBackgroundService> logger)
            : base(TimeSpan.FromSeconds(15), logger)
        {
            _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
            _projectAnalyzingService = projectAnalyzingService ?? throw new ArgumentNullException(nameof(projectAnalyzingService));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }

        // TODO Async voids are dangerous.
        // TODO Move getting the projects to analzying service?
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
                await foreach (var project in _projectRepository.GetAllAsync(token))
                {
                    var tasks = await _projectAnalyzingService.AnalyzeProjectAsync(project.Id, token);
                    _logger.LogTrace($"Analyzed project {project.Id} {project.Name}, found {tasks.Count()} tasks");

                    foreach (var task in tasks)
                    {
                        try
                        {
                            _queueManager.Enqueue(task);
                        }
                        catch (QueueFullException e)
                        {
                            // TODO Do we want to skip the queue when it's full?
                            _logger.LogWarning($"Queue was full, skipping task {task.Id}", e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}