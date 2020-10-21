using AbleSync.Core.Entities;
using AbleSync.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository for project tasks.
    /// </summary>
    public interface IProjectTaskRepository
    {
        /// <summary>
        ///     Creates a new <see cref="ProjectTask"/> in our data store.
        /// </summary>
        /// <param name="projectTask">See <see cref="Project"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created <see cref="ProjectTask"/>.</returns>
        Task<ProjectTask> CreateAsync(ProjectTask projectTask, CancellationToken token);

        /// <summary>
        ///     Gets all the <see cref="ProjectTask"/> entities from our data store.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of project tasks.</returns>
        IAsyncEnumerable<ProjectTask> GetAllAsync(CancellationToken token);

        /// <summary>
        ///     Gets all <see cref="ProjectTask"/>s for a given <see cref="Project"/>.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of <see cref="Project"/>s.</returns>
        IAsyncEnumerable<ProjectTask> GetAllForProjectAsync(Guid projectId, CancellationToken token);

        /// <summary>
        ///     Gets a <see cref="ProjectTask"/> from our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The returned <see cref="ProjectTask"/>.</returns>
        Task<ProjectTask> GetAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Marks the status of a <see cref="ProjectTask"/>.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="status">The new <see cref="ProjectTaskStatus"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The <see cref="ProjectTask"/> after the status update.</returns>
        Task<ProjectTask> MarkStatusAsync(Guid id, ProjectTaskStatus status, CancellationToken token);
    }
}
