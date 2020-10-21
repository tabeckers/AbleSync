using AbleSync.Core.Entities;
using AbleSync.Core.Helpers;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
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
    // TODO This is a work in progress.
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
        protected readonly IFileTrackingService _fileTrackingService;
        protected readonly AbleSyncOptions _options;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectAnalyzingService(IProjectRepository projectRepository,
            IFileTrackingService fileTrackingService,
            IOptions<AbleSyncOptions> options)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _fileTrackingService = fileTrackingService ?? throw new ArgumentNullException(nameof(fileTrackingService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        ///     Analyze a project, determine which project tasks have
        ///     to be exectued and return all these project tasks.
        /// </summary>
        /// <remarks>
        ///     This does not sync any project tasks with the data store.
        /// </remarks>
        /// <param name="projectId">The project to analyze.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>All project tasks that should be executed.</returns>
        public async Task<IEnumerable<ProjectTask>> AnalyzeProjectAsync(Guid projectId, CancellationToken token)
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
                    ProjectId = trackingFile.ProjectId,
                    ProjectTaskType = ProjectTaskType.UploadAudio,
                });
            }

            // FUTURE Here we will determin other types of tasks as well.

            return result;
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
