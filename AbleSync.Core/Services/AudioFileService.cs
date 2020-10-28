using AbleSync.Core.Entities;
using AbleSync.Core.Helpers;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using RenameMe.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Services
{
    /// <summary>
    ///     Service for audio file related operations.
    /// </summary>
    public class AudioFileService : IAudioFileService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IAudioFileRepository _audioFileRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AudioFileService(IBlobStorageService blobStorageService,
            IAudioFileRepository audioFileRepository)
        {
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _audioFileRepository = audioFileRepository ?? throw new ArgumentNullException(nameof(audioFileRepository));
        }

        /// <summary>
        ///     Gets an access uri to download an audio file.
        /// </summary>
        /// <param name="audioFileId">The audio file to download.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The access uri to download the file.</returns>
        public async Task<Uri> GetAccessUriAsync(Guid audioFileId, CancellationToken token)
        {
            audioFileId.ThrowIfNullOrEmpty();
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var audioFile = await _audioFileRepository.GetAsync(audioFileId, token);
            var downloadFileName = $"{audioFileId}{FileNameHelper.ToExtension(audioFile.AudioFormat)}";

            var uri = await _blobStorageService.GetAccessUriOverrideFilenameAsync(
                FileStorageHelper.AudioFileFolder(audioFile.ProjectId),
                audioFileId.ToString(),
                downloadFileName,
                1, // TODO From config
                Types.FileAccessType.Read, 
                token);

            return uri;
        }

        /// <summary>
        ///     Gets all audio files.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of all audio files.</returns>
        public IAsyncEnumerable<AudioFile> GetAllAsync(Pagination pagination, CancellationToken token)
            => _audioFileRepository.GetAllAsync(pagination, token);

        /// <summary>
        ///     Gets an audio file from our data store.
        /// </summary>
        /// <param name="audioFileId">The audio file id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The fetched audio file.</returns>
        public Task<AudioFile> GetAsync(Guid audioFileId, CancellationToken token)
            => _audioFileRepository.GetAsync(audioFileId, token);

        // TODO This gets a single item.
        // TODO Audio format.
        // TODO ToAsyncEnumerable?
        /// <summary>
        ///     Gets all audio files for a given project.
        /// </summary>
        /// <param name="projectId">The respective project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>All audio files that belong to the project id.</returns>
        public IAsyncEnumerable<AudioFile> GetForProjectAsync(Guid projectId, Pagination pagination, CancellationToken token)
            => _audioFileRepository.GetFromProjectAsync(projectId, AudioFormat.Mp3, token).ToAsyncEnumerable();

        /// <summary>
        ///     Gets all latest updated audio files.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of the latest updated audio files.</returns>
        public IAsyncEnumerable<AudioFile> GetLastestAsync(Pagination pagination, CancellationToken token)
            => _audioFileRepository.GetLatestAsync(pagination, token);

        /// <summary>
        ///     Search by a query in our data store for audio files.
        /// </summary>
        /// <param name="query">The search term.</param>
        /// <param name="pagination">The pagination.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Search result audio file collection.</returns>
        public IAsyncEnumerable<AudioFile> SearchAsync(string query, Pagination pagination, CancellationToken token)
        {
            query.ThrowIfNullOrEmpty();
            if (query.Length < 3) {
                throw new ArgumentException("Search query must be at least 3 characters long");
            }

            return _audioFileRepository.SearchAsync(query, pagination, token);
        }
    }
}
