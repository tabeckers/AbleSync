using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AbleSync.Core.Entities;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a service which analyzes a project folder. This will
    ///     determine which <see cref="ProjectTask"/>s have to be executed.
    /// </summary>
    public interface IProjectAnalyzingService
    {
        Task<IEnumerable<ProjectTask>> GetTasksForProject(Project project);
    }
}
