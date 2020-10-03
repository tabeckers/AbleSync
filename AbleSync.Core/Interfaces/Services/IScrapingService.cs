using System;
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
        /// <param name="relativePath"></param>
        /// <returns></returns>
        Task ProcessProjectAsync(Uri relativePath);

        /// <summary>
        ///     This will call <see cref="ProcessProjectAsync(Uri)"/> recursively
        ///     for a given relative path.
        /// </summary>
        /// <remarks>
        ///     The <paramref name="relativePath"/> may be null to process the root.
        /// </remarks>
        /// <param name="relativePath">Relative path with regards to the root uri.</param>
        /// <returns></returns>
        Task ProcessFolderRecursivelyAsync(Uri relativePath);
    }
}
