using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a scraping service which is capable of fully processing
    ///     a single project or all projects in a folder.
    /// </summary>
    public interface IProjectScrapingService
    {
        /// <summary>
        ///     This gets or creates the tracking file for a given project. Then
        ///     this will analyze the project, store the changes in our datastore
        ///     and write these changes to the tracking file.
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
