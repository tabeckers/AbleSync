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
    ///     This does nothing with project tasks.
    /// </remarks>
    public interface IProjectScrapingService
    {
        /// <summary>
        ///     Process a given Ableton project folder.
        /// </summary>
        /// <remarks>
        ///     This does nothing with project tasks.
        /// </remarks>
        /// <param name="token">The cancellation token.</param>
        /// <param name="directoryInfo">The directory to process.</param>
        Task ProcessAbletonProjectFolderAsync(DirectoryInfo directoryInfo, CancellationToken token);

        /// <summary>
        ///     This will call <see cref="ProcessAbletonProjectFolderAsync(Uri)"/> recursively
        ///     for a given relative path.
        /// </summary>
        /// <param name="directoryInfo">The directory to process recursively.</param>
        /// <param name="token">The cancellation token.</param>
        Task ProcessDirectoryRecursivelyAsync(DirectoryInfo directoryInfo, CancellationToken token);

        /// <summary>
        ///     Calls <see cref="ProcessDirectoryRecursivelyAsync(DirectoryInfo, CancellationToken)"/>
        ///     with the root directory from the options file as the directory info param.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        Task ProcessRootDirectoryRecursivelyAsync(CancellationToken token);
    }
}
