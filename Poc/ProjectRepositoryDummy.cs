using AbleSync.Core.Entities;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poc
{
    public class ProjectRepositoryMock : IProjectRepository
    {
        public Task<Project> CreateAsync(Project project) => Task.FromResult(new Project
        {
            Id = Guid.NewGuid(),
        });

        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => Task.FromResult(true);
        public Task<IEnumerable<Project>> GetAllAsync(Guid id) => throw new NotImplementedException();
        public Task<Project> GetAsync(Guid id) => Task.FromResult(new Project
        {
            Id = id
        });
        public Task<Project> MarkProjectAsync(Guid id, ProjectStatus status) => Task.FromResult(new Project
        {
            Id = Guid.NewGuid(),
        });
        public Task<Project> UpdateAsync(Project project) => throw new NotImplementedException();
    }
}
