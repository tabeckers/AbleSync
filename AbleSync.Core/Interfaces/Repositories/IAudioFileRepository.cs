using AbleSync.Core.Entities;
using AbleSync.Core.Types;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository for audio files.
    /// </summary>
    public interface IAudioFileRepository : IRepositoryBaseCRUD<AudioFile>
    {
        /// <summary>
        ///     Checks if any audio files exist for a given 
        ///     project id.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><c>True</c> if any audio file exists.</returns>
        Task<bool> ExistsAnyForProjectAsync(Guid projectId, CancellationToken token);

        /// <summary>
        ///     Checks if an audio file in a specific audio format
        ///     exists for a given project id.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="format">The audio format to check for.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><c>True</c> if an audio file exists.</returns>
        Task<bool> ExistsForProjectAsync(Guid projectId, AudioFormat format, CancellationToken token);

        /// <summary>
        ///     Gets an audio file for a given project and a
        ///     given audio format.
        /// </summary>
        /// <remarks>
        ///     The audio file should exist or this will throw
        ///     an exception.
        /// </remarks>
        /// <param name="projectId">The project id.</param>
        /// <param name="format">The format to get.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The audio file for the project and format.</returns>
        Task<AudioFile> GetFromProjectAsync(Guid projectId, AudioFormat format, CancellationToken token);

        /// <summary>
        ///     Marks the sync date of an audio file as now.
        /// </summary>
        /// <param name="id">The audio file id.</param>
        /// <param name="token">The cancellation token.</param>
        Task MarkSyncedAsync(Guid id, CancellationToken token);
    }
}
