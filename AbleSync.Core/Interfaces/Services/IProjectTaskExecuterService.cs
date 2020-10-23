using AbleSync.Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a service that will process any 
    ///     pending operations for projects.
    /// </summary>
    /// <remarks>
    ///     This will first create the project task in the
    ///     data store, then execute, then sync execution
    ///     outcome (success/failure) with the data store.
    /// </remarks>
    public interface IProjectTaskExecuterService
    {
        /// <summary>
        ///     Process a project task.
        /// </summary>
        /// <param name="task">The task to process.</param>
        /// <param name="token">The cancellation token.</param>
        Task ProcessProjectTaskAsync(ProjectTask task, CancellationToken token);

        /// <summary>
        ///     Process a collection of project tasks.
        /// </summary>
        /// <param name="tasks">All tasks to process.</param>
        /// <param name="token">The cancellation token.</param>
        Task ProcessProjectTasksAsync(IEnumerable<ProjectTask> tasks, CancellationToken token);
    }
}
