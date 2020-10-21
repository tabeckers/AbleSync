using AbleSync.Core.Entities;
using System;
using System.Collections.Generic;
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
        // FUTURE Make IAsyncEnumerable
        /// <summary>
        ///     Analyze a project and determine all project tasks that have
        ///     to be executed for it.
        /// </summary>
        /// <param name="projectId">The project to analyze.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>All project tasks that should be executed.</returns>
        public Task<IEnumerable<ProjectTask>> AnalyzeProjectAsync(Guid projectId, CancellationToken token);
    }
}
