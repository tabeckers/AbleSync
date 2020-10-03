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
        Task<Project> CreateAsync(Project project);

        Task DeleteAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);

        Task<IEnumerable<Project>> GetAllAsync(Guid id);

        Task<Project> GetAsync(Guid id);

        Task<Project> MarkProjectAsync(Guid id, ProjectStatus status);

        Task<Project> UpdateAsync(Project project);
    }
}
