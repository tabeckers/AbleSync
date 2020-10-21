﻿using AbleSync.Core.Entities;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Services
{
    /// <summary>
    ///     Service for processing project tasks.
    /// </summary>
    public class ProjectTaskProcessingService : IProjectTaskProcessingService
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<ProjectTaskProcessingService> _logger;
        private readonly AbleSyncOptions _options;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectTaskProcessingService(IProjectTaskRepository projectTaskRepository,
            IBlobStorageService blobStorageService,
            ILogger<ProjectTaskProcessingService> logger,
            IOptions<AbleSyncOptions> options)
        {
            _projectTaskRepository = projectTaskRepository ?? throw new ArgumentNullException(nameof(projectTaskRepository));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        ///     Processes a single project task.
        /// </summary>
        /// <param name="project">The project to which the project task belongs.</param>
        /// <param name="task">The project task.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        public Task ProcessProjectTaskAsync(Project project, ProjectTask task, CancellationToken token)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return task.ProjectTaskType switch
            {
                Types.ProjectTaskType.UploadAudio => ExecuteUploadAudioAsync(project, task, token),
                Types.ProjectTaskType.BackupFull => ExecuteBackupFullAsync(project, task, token),
                _ => throw new InvalidOperationException(nameof(task.ProjectTaskType)),
            };

        }

        /// <summary>
        ///     Processes a collection of tasks.
        /// </summary>
        /// <remarks>
        ///     All <paramref name="tasks"/> should belong to the same
        ///     <paramref name="project"/>.
        /// </remarks>
        /// <param name="project">The project to which the tasks belong.</param>
        /// <param name="tasks">The tasks to process.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task ProcessProjectTasksAsync(Project project, IEnumerable<ProjectTask> tasks, CancellationToken token)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (tasks == null || !tasks.Any())
            {
                throw new ArgumentNullException(nameof(tasks));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            if (tasks.Where(x => x.ProjectId != project.Id).Any())
            {
                throw new InvalidOperationException("All tasks should belong to the same project.");
            }

            // FUTURE AsyncEnumerable
            foreach (var task in tasks)
            {
                await ProcessProjectTaskAsync(project, task, token);
            }
        }

        // TODO Make this into a separate service, https://github.com/tabeckers/AbleSync/issues/29
        // TODO Task isnt used?
        /// <summary>
        ///     Execute the upload for an audio file.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="task">The task.</param>
        /// <param name="token">The cancellation token.</param>
        private async Task ExecuteUploadAudioAsync(Project project, ProjectTask task, CancellationToken token)
        {
            // TODO Beun, see https://github.com/tabeckers/AbleSync/issues/19
            var parsedPath = project.RelativePath.Replace("\\", "/", StringComparison.InvariantCulture);
            var path = $"{_options.RootDirectoryPath.AbsolutePath}/{parsedPath}";
            path = path.Replace("%20", " ", StringComparison.InvariantCulture);

            var directoryInfo = new DirectoryInfo(path);

            var audioFiles = directoryInfo.GetFiles().Where(x => Constants.ExportedAudioFileExtensions.Contains(x.Extension));

            // TODO Do elegantly.
            var audioFile = null as FileInfo;
            var contentType = "";
            if (audioFiles.Where(x => x.Extension == Constants.AudioMp3FileExtension).Any())
            {
                audioFile = audioFiles.Where(x => x.Extension == Constants.AudioMp3FileExtension).First();
                contentType = Constants.ContentTypeMp3;
            }
            else if (audioFiles.Where(x => x.Extension == Constants.AudioWavFileExtension).Any())
            {
                audioFile = audioFiles.Where(x => x.Extension == Constants.AudioWavFileExtension).First();
                contentType = Constants.ContentTypeWav;
            }
            else if (audioFiles.Where(x => x.Extension == Constants.AudioFlacFileExtension).Any())
            {
                audioFile = audioFiles.Where(x => x.Extension == Constants.AudioFlacFileExtension).First();
                contentType = Constants.ContentTypeFlac;
            }
            else
            {
                throw new InvalidOperationException("Could not get proper audio file.");
            }

            // This has the extension in it as well.
            // TODO This is windows coupled. See https://github.com/tabeckers/AbleSync/issues/19 and https://github.com/tabeckers/AbleSync/issues/28
            var fileName = $"{audioFile.Name}";
            var directoryName = $"{Constants.StorageProjectFolderBase}/{project.Id}/{project.Name}.";
            var fullFileName = $"{path}/{audioFile.Name}";

            using var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read);
            await _blobStorageService.StoreFileAsync(directoryName, fileName, contentType, fileStream, token);

            // TODO Here? https://github.com/tabeckers/AbleSync/issues/31
            _logger.LogTrace($"Processed task {task.Id} {task.ProjectTaskType} - uploaded {fileName} to blob storage"); 
        }

        // FUTURE: Implement.
        private Task ExecuteBackupFullAsync(Project project, ProjectTask task, CancellationToken token)
            => throw new NotImplementedException();
    }
}
