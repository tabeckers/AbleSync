using AbleSync.Core.Entities;
using AbleSync.Core.Exceptions;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
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
    /// <remarks>
    ///     This will first create the project task in the
    ///     data store, then execute, then sync execution
    ///     outcome (success/failure) with the data store.
    /// </remarks>
    public class ProjectTaskProcessingService : IProjectTaskExecuterService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<ProjectTaskProcessingService> _logger;
        private readonly AbleSyncOptions _options;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectTaskProcessingService(IProjectRepository projectRepository,
            IProjectTaskRepository projectTaskRepository,
            IBlobStorageService blobStorageService,
            ILogger<ProjectTaskProcessingService> logger,
            IOptions<AbleSyncOptions> options)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _projectTaskRepository = projectTaskRepository ?? throw new ArgumentNullException(nameof(projectTaskRepository));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
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

            await _projectTaskRepository.CreateAsync(task, token);

            try
            {
                await (task.ProjectTaskType switch
                {
                    ProjectTaskType.UploadAudio => ExecuteUploadAudioAsync(task, token),
                    ProjectTaskType.BackupFull => ExecuteBackupFullAsync(task, token),
                    _ => throw new InvalidOperationException(nameof(task.ProjectTaskType)),
                });

                await _projectTaskRepository.MarkStatusAsync(task.Id, ProjectTaskStatus.Done, token);
            } 
            catch (AbleSyncBaseException e)
            {
                _logger.LogError($"Could not process project task {task.Id}", e);
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

        // TODO Make this into a separate service, https://github.com/tabeckers/AbleSync/issues/29
        // TODO Task isnt used?
        /// <summary>
        ///     Execute the upload for an audio file.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="token">The cancellation token.</param>
        private async Task ExecuteUploadAudioAsync(ProjectTask task, CancellationToken token)
        {
            var project = await _projectRepository.GetAsync(task.ProjectId, token);

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
        private Task ExecuteBackupFullAsync(ProjectTask task, CancellationToken token)
            => throw new NotImplementedException();
    }
}
