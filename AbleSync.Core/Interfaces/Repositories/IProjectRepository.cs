using AbleSync.Core.Entities;
using AbleSync.Core.Types;
using System;
using System.Collections.Generic;
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
        /// <returns>The created <see cref="Project"/>.</returns>
        Task<Project> CreateAsync(Project project);

        /// <summary>
        ///     Deletes a project from our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        ///     Checks if a given <see cref="Project"/> exists in our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <returns><c>true</c> if the project exists.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        ///     Gets all <see cref="Project"/>s from our data store.
        /// </summary>
        /// <returns>Collection of <see cref="Project"/>s.</returns>
        Task<IEnumerable<Project>> GetAllAsync();

        /// <summary>
        ///     Gets a <see cref="Project"/> from our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <returns>The returned <see cref="Project"/>.</returns>
        Task<Project> GetAsync(Guid id);

        /// <summary>
        ///     Marks the status of a <see cref="Project"/>.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="status">The new <see cref="ProjectStatus"/>.</param>
        /// <returns>The <see cref="Project"/> after the status update.</returns>
        Task<Project> MarkProjectAsync(Guid id, ProjectStatus status);

        /// <summary>
        ///     Updates a <see cref="Project"/> in our data store.
        /// </summary>
        /// <param name="project">The new to-update <see cref="Project"/>.</param>
        /// <returns>The updated <see cref="Project"/>.</returns>
        Task<Project> UpdateAsync(Project project);
    }
}
