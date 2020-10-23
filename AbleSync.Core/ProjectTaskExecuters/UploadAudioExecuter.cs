using AbleSync.Core.Entities;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.ProjectTaskExecuters
{
    // TODO Base abstraction or class for executers?
    /// <summary>
    ///     Project task executor for <see cref="ProjectTaskType.UploadAudio"/>.
    /// </summary>
    public class UploadAudioExecuter
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly AbleSyncOptions _options;
        private readonly ILogger<UploadAudioExecuter> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UploadAudioExecuter(IProjectRepository projectRepository,
            IBlobStorageService blobStorageService,
            IOptions<AbleSyncOptions> options,
            ILogger<UploadAudioExecuter> logger)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // TODO Task isnt used?
        /// <summary>
        ///     Execute the upload for an audio file.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task ExecuteUploadAudioAsync(ProjectTask task, CancellationToken token)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            if (task.ProjectTaskType != ProjectTaskType.UploadAudio)
            {
                throw new InvalidOperationException(nameof(task.ProjectTaskType));
            }

            _logger.LogTrace($"Starting audio upload for project {task.ProjectId}");

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
            _logger.LogTrace($"Finished audio upload for project {task.ProjectId} - uploaded {fileName}");
        }

    }
}
