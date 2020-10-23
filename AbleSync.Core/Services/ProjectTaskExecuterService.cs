using AbleSync.Core.Entities;
using AbleSync.Core.Exceptions;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.ProjectTaskExecuters;
using AbleSync.Core.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace AbleSync.Core.Services
{
    /// <summary>
    ///     Service for processing project tasks.
    /// </summary>
    /// <remarks>
    ///     This will first create the project task in the
    ///     data store, then execute, then sync execution
    ///     outcome (success/failure) with the data store.
    /// </remarks>
    public class ProjectTaskExecuterService : IProjectTaskExecuterService
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly UploadAudioExecuter _uploadAudioExecuter;

        private readonly ILogger<ProjectTaskExecuterService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectTaskExecuterService(IProjectTaskRepository projectTaskRepository,
            UploadAudioExecuter uploadAudioExecuter,
            ILogger<ProjectTaskExecuterService> logger)
        {
            _projectTaskRepository = projectTaskRepository ?? throw new ArgumentNullException(nameof(projectTaskRepository));
            _uploadAudioExecuter = uploadAudioExecuter ?? throw new ArgumentNullException(nameof(uploadAudioExecuter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Processes a single project task.
        /// </summary>
        /// <remarks>
        ///     First this syncs the task with the data store, then 
        ///     the task is executed, then the success or failure is
        ///     synced with the data store.
        /// </remarks>
        /// <param name="task">The project task.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task ProcessProjectTaskAsync(ProjectTask task, CancellationToken token)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            _logger.LogInformation($"Starting project task {task.Id} of type {task.ProjectTaskType}");

            // This should be transactional --> scope?
            var taskCreated = await _projectTaskRepository.CreateAsync(task, token);

            try
            {
                await (task.ProjectTaskType switch
                {
                    ProjectTaskType.UploadAudio => _uploadAudioExecuter.ExecuteUploadAudioAsync(task, token),
                    ProjectTaskType.BackupFull => throw new NotImplementedException(),
                    _ => throw new InvalidOperationException(nameof(task.ProjectTaskType)),
                });

                await _projectTaskRepository.MarkStatusAsync(taskCreated.Id, ProjectTaskStatus.Done, token);

                _logger.LogInformation($"Finished project task {task.Id} of type {task.ProjectTaskType}");
            }
            catch (AbleSyncBaseException e)
            {
                _logger.LogError($"Could not process project task {task.Id} of type {task.ProjectTaskType}", e);
                await _projectTaskRepository.MarkStatusAsync(task.Id, ProjectTaskStatus.Failed, token);
            }
        }

        /// <summary>
        ///     Processes a collection of tasks.
        /// </summary>
        /// <param name="tasks">The tasks to process.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task ProcessProjectTasksAsync(IEnumerable<ProjectTask> tasks, CancellationToken token)
        {
            if (tasks == null || !tasks.Any())
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            // FUTURE AsyncEnumerable
            foreach (var task in tasks)
            {
                await ProcessProjectTaskAsync(task, token);
            }
        }
    }
}
