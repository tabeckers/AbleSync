using AbleSync.Core.Entities;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Services
{
    /// <summary>
    ///     Analyzer class for <see cref="Project"/> entities to determine
    ///     which <see cref="ProjectTask"/>s should be created.
    /// </summary>
    internal class ProjectAnalyzingService : IProjectAnalyzingService
    {
        protected readonly IProjectTaskRepository _projectTaskRepository;
        protected readonly IFileTrackingService _fileTrackingService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectAnalyzingService(IProjectTaskRepository projectTaskRepository,
            IFileTrackingService fileTrackingService)
        {
            _projectTaskRepository = projectTaskRepository ?? throw new ArgumentNullException(nameof(projectTaskRepository));
            _fileTrackingService = fileTrackingService ?? throw new ArgumentNullException(nameof(fileTrackingService));
        }

        // TODO Make IAsyncEnumerable
        /// <summary>
        ///     Analyzes a project and determines which <see cref="ProjectTask"/>
        ///     entities will have to be executed for said project.
        /// </summary>
        /// <remarks>
        ///     Any tasks that should be done for this project will be stored
        ///     in the data store. Any existing tasks will be compared to the 
        ///     result of the task analysis.
        /// </remarks>
        /// <param name="directoryInfo">The project directory.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A collection of project tasks for the project.</returns>
        public async Task<IEnumerable<ProjectTask>> SyncTasksForProjectAsync(DirectoryInfo directoryInfo, CancellationToken token)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var trackingFile = _fileTrackingService.GetTrackingFile(directoryInfo);

            var existingTasks = await _projectTaskRepository.GetAllForProjectAsync(trackingFile.ProjectId, token).ToListAsync(token);

            // TODO Check for changes in audio file.

            // If we don't have a sync audio task, create one. If we already have
            // one, the background worker will simply sync the latest audio file.
            if (!existingTasks.Where(x => x.ProjectTaskType == ProjectTaskType.UploadAudio).Any() &&
                DoesDirectoryContainAudioFiles(directoryInfo))
            {
                await _projectTaskRepository.CreateAsync(new ProjectTask
                {
                    ProjectId = trackingFile.ProjectId,
                    ProjectTaskType = ProjectTaskType.UploadAudio,
                }, token);
            }

            // FUTURE Here we will determin other types of tasks as well.

            return await _projectTaskRepository.GetAllForProjectAsync(trackingFile.ProjectId, token).ToListAsync(token);
        }

        /// <summary>
        ///     Checks if a directory contains one or more audio files.
        /// </summary>
        /// <param name="directoryInfo">The directory to analyze.</param>
        /// <returns><c>true</c> if there is one or more audio file.</returns>
        private static bool DoesDirectoryContainAudioFiles(DirectoryInfo directoryInfo)
            => directoryInfo.GetFiles().Where(x => Constants.ExportedAudioFileExtensions.Contains(x.Extension)).Any();
    }
}
