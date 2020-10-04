using AbleSync.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a service which analyzes a project folder. This will
    ///     determine which <see cref="ProjectTask"/>s have to be executed.
    /// </summary>
    public interface IProjectAnalyzingService
    {
        // TODO Do we really need to return the result?
        // TODO Do we really need the entire project, isn't the id enough?
        /// <summary>
        ///     Analyzes a project and determines which <see cref="ProjectTask"/>
        ///     entities will have to be executed for said project.
        /// </summary>
        /// <param name="directoryInfo">The project directory.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A collection of project tasks for the project.</returns>
        Task<IEnumerable<ProjectTask>> SyncTasksForProjectAsync(DirectoryInfo directoryInfo, CancellationToken token);
    }
}
