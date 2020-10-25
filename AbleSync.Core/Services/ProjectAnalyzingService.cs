using AbleSync.Core.Entities;
using AbleSync.Core.Exceptions;
using AbleSync.Core.Helpers;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Managers;
using AbleSync.Core.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RenameMe.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Services
{
    /// <summary>
    ///     Analyzer class for project folders to determine
    ///     which <see cref="ProjectTask"/>s should be created.
    /// </summary>
    /// <remarks>
    ///     This does not sync any project tasks with the data store.
    /// </remarks>
    internal class ProjectAnalyzingService : IProjectAnalyzingService
    {
        protected readonly IProjectRepository _projectRepository;
        protected readonly ITrackingFileService _fileTrackingService;
        protected readonly QueueManager _queueManager;
        protected readonly AbleSyncOptions _options;
        protected readonly ILogger<ProjectAnalyzingService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectAnalyzingService(IProjectRepository projectRepository,
            ITrackingFileService fileTrackingService,
            QueueManager queueManager,
            IOptions<AbleSyncOptions> options,
            ILogger<ProjectAnalyzingService> logger)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _fileTrackingService = fileTrackingService ?? throw new ArgumentNullException(nameof(fileTrackingService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _queueManager = queueManager ?? throw new ArgumentNullException(nameof(queueManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Analyze a project, determine which project tasks have
        ///     to be exectued and return all these project tasks. All
        ///     tasks are then enqueued.
        /// </summary>
        /// <remarks>
        ///     This does not sync any project tasks with the data store.
        /// </remarks>
        /// <param name="projectId">The project to analyze.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task AnalyzeProjectEnqueueTasksAsync(Guid projectId, CancellationToken token)
        {
            projectId.ThrowIfNullOrEmpty();
            var project = await _projectRepository.GetAsync(projectId, token);

            // TODO This should not have directory info
            var directoryInfo = DirectoryInfoHelper.GetFromProject(_options.RootDirectoryPath, project);

            var trackingFile = _fileTrackingService.GetTrackingFile(directoryInfo);

            var result = new List<ProjectTask>();

            if (DoesDirectoryContainAudioFiles(directoryInfo))
            {
                // FUTURE Check for changes in audio file. https://github.com/tabeckers/AbleSync/issues/27
                result.Add(new ProjectTask
                {
                    Id = Guid.NewGuid(), // TODO Like this?
                    ProjectId = trackingFile.ProjectId,
                    ProjectTaskType = ProjectTaskType.UploadAudio,
                });
            }

            // FUTURE Other task types as well

            EnqueueAll(result);

            _logger.LogTrace($"Analyzed and enqueued {result.Count} tasks for project {projectId}");
        }

        /// <summary>
        ///     Calls <see cref="AnalyzeAllProjectsEnqueueTasksAsync"/> for
        ///     each project in our data store.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public async Task AnalyzeAllProjectsEnqueueTasksAsync(CancellationToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            await foreach (var project in _projectRepository.GetAllAsync(token))
            {
                await AnalyzeProjectEnqueueTasksAsync(project.Id, token);
            }
        }

        private void EnqueueAll(IEnumerable<ProjectTask> projectTasks)
        {
            foreach (var projectTask in projectTasks)
            {
                try
                {
                    _queueManager.Enqueue(projectTask);
                }
                catch (QueueFullException e)
                {
                    // TODO Do we want to skip the queue when it's full?
                    _logger.LogWarning($"Queue was full, skipping task {projectTask.Id} of type {projectTask.ProjectTaskType}", e);
                }
            }
        }

        // TODO Move to helper.
        /// <summary>
        ///     Checks if a directory contains one or more audio files.
        /// </summary>
        /// <param name="directoryInfo">The directory to analyze.</param>
        /// <returns><c>true</c> if there is one or more audio file.</returns>
        private static bool DoesDirectoryContainAudioFiles(DirectoryInfo directoryInfo)
            => directoryInfo.GetFiles().Where(x => Constants.ExportedAudioFileExtensions.Contains(x.Extension)).Any();
    }
}
