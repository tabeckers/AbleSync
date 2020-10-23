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
    public interface IProjectRepository
    {
        /// <summary>
        ///     Creates a new <see cref="Project"/> in our data store.
        /// </summary>
        /// <param name="project">See <see cref="Project"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created <see cref="Project"/>.</returns>
        Task<Project> CreateAsync(Project project, CancellationToken token);

        /// <summary>
        ///     Deletes a project from our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        Task DeleteAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Checks if a given <see cref="Project"/> exists in our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><c>true</c> if the project exists.</returns>
        Task<bool> ExistsAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Gets all <see cref="Project"/>s from our data store.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of <see cref="Project"/>s.</returns>
        IAsyncEnumerable<Project> GetAllAsync(CancellationToken token);

        /// <summary>
        ///     Gets a <see cref="Project"/> from our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The returned <see cref="Project"/>.</returns>
        Task<Project> GetAsync(Guid id, CancellationToken token);

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
        ///     Updates a <see cref="Project"/> in our data store.
        /// </summary>
        /// <param name="project">The new to-update <see cref="Project"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated <see cref="Project"/>.</returns>
        Task<Project> UpdateAsync(Project project, CancellationToken token);
    }
}
