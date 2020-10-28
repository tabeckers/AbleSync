using AbleSync.Core.Entities;
using AbleSync.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for audio file related operations.
    /// </summary>
    public interface IAudioFileService
    {
        /// <summary>
        ///     Gets an access uri to download an audio file.
        /// </summary>
        /// <param name="audioFileId">The audio file to download.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The access uri to download the file.</returns>
        Task<Uri> GetAccessUriAsync(Guid audioFileId, CancellationToken token);

        /// <summary>
        ///     Gets all audio files.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of all audio files.</returns>
        IAsyncEnumerable<AudioFile> GetAllAsync(Pagination pagination, CancellationToken token);

        /// <summary>
        ///     Gets an audio file from our data store.
        /// </summary>
        /// <param name="audioFileId">The audio file id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The fetched audio file.</returns>
        Task<AudioFile> GetAsync(Guid audioFileId, CancellationToken token);

        /// <summary>
        ///     Gets all audio files for a given project.
        /// </summary>
        /// <param name="projectId">The respective project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>All audio files that belong to the project id.</returns>
        IAsyncEnumerable<AudioFile> GetForProjectAsync(Guid projectId, Pagination pagination, CancellationToken token);

        /// <summary>
        ///     Gets all latest updated audio files.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of the latest updated audio files.</returns>
        IAsyncEnumerable<AudioFile> GetLastestAsync(Pagination pagination, CancellationToken token);

        /// <summary>
        ///     Search by a query in our data store for audio files.
        /// </summary>
        /// <param name="query">The search term.</param>
        /// <param name="pagination">The pagination.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Search result audio file collection.</returns>
        IAsyncEnumerable<AudioFile> SearchAsync(string query, Pagination pagination, CancellationToken token);
    }
}
