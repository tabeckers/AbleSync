using AbleSync.Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a service which analyzes a project folder. This will
    ///     determine which <see cref="ProjectTask"/>s have to be executed.
    /// </summary>
    /// <remarks>
    ///     This does not sync any project tasks with the data store.
    /// </remarks>
    public interface IProjectAnalyzingService
    {
        /// <summary>
        ///     Analyze a project and determine all project tasks that have
        ///     to be executed for it. All tasks are then enqueued.
        /// </summary>
        /// <param name="projectId">The project to analyze.</param>
        /// <param name="token">The cancellation token.</param>
        public Task AnalyzeProjectEnqueueTasksAsync(Guid projectId, CancellationToken token);

        /// <summary>
        ///     Analyze all projects and determine all project tasks that
        ///     have to be executed for them. All tasks are then enqueued.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public Task AnalyzeAllProjectsEnqueueTasksAsync(CancellationToken token);
    }
}
