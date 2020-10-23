using AbleSync.Core.Entities;
using AbleSync.Core.Helpers;
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
        private readonly IAudioFileRepository _audioFileRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly AbleSyncOptions _options;
        private readonly ILogger<UploadAudioExecuter> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UploadAudioExecuter(IAudioFileRepository audioFileRepository,
            IProjectRepository projectRepository,
            IBlobStorageService blobStorageService,
            IOptions<AbleSyncOptions> options,
            ILogger<UploadAudioExecuter> logger)
        {
            _audioFileRepository = audioFileRepository ?? throw new ArgumentNullException(nameof(audioFileRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // FUTURE Also support other audio formats.
        // TODO This is windows coupled. See https://github.com/tabeckers/AbleSync/issues/19 and https://github.com/tabeckers/AbleSync/issues/28
        // TODO Pick correct audio file, there may be multiple mp3 files.
        /// <summary>
        ///     Execute the upload for an audio file.
        /// </summary>
        /// <remarks>
        ///     This only handles mp3.
        /// </remarks>
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

            _logger.LogTrace($"Starting mp3 audio upload for project {task.ProjectId}");

            var project = await _projectRepository.GetAsync(task.ProjectId, token);
            var directoryInfo = DirectoryInfoHelper.GetFromProject(_options.RootDirectoryPath, project);
            var audioFiles = directoryInfo.GetFiles().Where(x => Constants.ExportedAudioFileExtensions.Contains(x.Extension));

            // TODO Do elegantly.
            if (!audioFiles.Where(x => x.Extension == Constants.AudioMp3FileExtension).Any())
            {
                // TODO Custom exception.
                throw new InvalidOperationException("Could not get mp3 file in project folder");
            }

            // Get the actual physical file because we need its name.
            var audioFilePhysical = audioFiles.Where(x => x.Extension == Constants.AudioMp3FileExtension).First();

            // Get or create the audio file.
            var audioFileEntity = await _audioFileRepository.ExistsForProjectAsync(task.ProjectId, AudioFormat.Mp3, token)
                ? await _audioFileRepository.GetFromProjectAsync(task.ProjectId, AudioFormat.Mp3, token)
                : await _audioFileRepository.CreateGetAsync(new AudioFile
                {
                    AudioFormat = AudioFormat.Mp3,
                    Name = audioFilePhysical.Name,
                    ProjectId = project.Id
                }, token);

            // TODO Beun
            var parsedPath = project.RelativePath.Replace("\\", "/", StringComparison.InvariantCulture);
            var path = $"{_options.RootDirectoryPath.AbsolutePath}/{parsedPath}";
            path = path.Replace("%20", " ", StringComparison.InvariantCulture);

            var fileNamePhysical = audioFilePhysical.Name;
            var folderNamePhysical = $"{path}";
            using var fileStream = new FileStream($"{folderNamePhysical}/{fileNamePhysical}", FileMode.Open, FileAccess.Read);

            var fileNameStorage = FileStorageHelper.AudioFileName(audioFileEntity.Id);
            var folderNameStorage = FileStorageHelper.AudioFileFolder(project.Id);
            await _blobStorageService.StoreFileAsync(folderNameStorage, fileNameStorage, Constants.ContentTypeMp3, fileStream, token);

            // Mark our synchronization
            await _audioFileRepository.MarkSyncedAsync(audioFileEntity.Id, token);
            
            _logger.LogTrace($"Finished mp3 audio upload for project {task.ProjectId} - uploaded {fileNamePhysical} as {fileNameStorage}");
        }
    }
}
