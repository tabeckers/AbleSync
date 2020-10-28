using AbleSync.Core.Entities;
using AbleSync.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository for Ableton projects.
    /// </summary>
    public interface IProjectRepository : IRepositoryBaseCRUD<Project>
    {
        /// <summary>
        ///     Gets the projects from our data store ordered 
        ///     by most recent update date.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Latest project collection.</returns>
        IAsyncEnumerable<Project> GetLatestAsync(Pagination pagination, CancellationToken token);

        /// <summary>
        ///     Marks a project as scraped by setting it's update date.
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated project as fetched from the data store.</returns>
        Task<Project> MarkProjectAsScrapedAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Marks the status of a <see cref="Project"/>.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="status">The new <see cref="ProjectStatus"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The <see cref="Project"/> after the status update.</returns>
        Task<Project> MarkProjectAsync(Guid id, ProjectStatus status, CancellationToken token);

        /// <summary>
        ///     Search by a query in our data store for projects.
        /// </summary>
        /// <param name="query">The search term.</param>
        /// <param name="pagination">The pagination.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Search result project collection.</returns>
        IAsyncEnumerable<Project> SearchAsync(string query, Pagination pagination, CancellationToken token);
    }
}
