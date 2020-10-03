using AbleSync.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a service that will process any pending operations for
    ///     projects.
    /// </summary>
    public interface IProjectTaskProcessingService
    {
        Task ProcessProjectTaskAsync(Project project, ProjectTask task);

        Task ProcessProjectTaskAsync(Project project, IEnumerable<ProjectTask> tasks);
    }
}
