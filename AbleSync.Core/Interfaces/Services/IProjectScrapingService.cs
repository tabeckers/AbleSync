using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a scraping service which recursively scans a root directory
    ///     and gets all Ableton project directories. Each retrieved item will be
    ///     processed and it's metadata will be synchronized with our data store.
    /// </summary>
    /// <remarks>
    ///     This does not actually perform any operations other than metadata syncs.
    /// </remarks>
    public interface IProjectScrapingService
    {
        /// <summary>
        ///     This gets or creates the tracking file for a given project. Then
        ///     this will analyze the project, store the changes in our datastore
        ///     and write these changes to the tracking file. Any required operations
        ///     on our project will also be created or updated in our data store.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <param name="directoryInfo">The directory to process.</param>
        /// <returns><see cref="Task"/></returns>
        Task ProcessAbletonProjectFolderAsync(DirectoryInfo directoryInfo, CancellationToken token);

        /// <summary>
        ///     This will call <see cref="ProcessAbletonProjectFolderAsync(Uri)"/> recursively
        ///     for a given relative path.
        /// </summary>
        /// <param name="directoryInfo">The directory to process recursively.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task ProcessDirectoryRecursivelyAsync(DirectoryInfo directoryInfo, CancellationToken token);

        /// <summary>
        ///     Calls <see cref="ProcessDirectoryRecursivelyAsync(DirectoryInfo, CancellationToken)"/>
        ///     with the root directory from the options file as the directory info param.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task ProcessRootDirectoryRecursivelyAsync(CancellationToken token);
    }
}
