using AbleSync.Core.Entities;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Types;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository for <see cref="Project"/> entities.
    /// </summary>
    internal sealed class ProjectRepository : RepositoryBase, IProjectRepository
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectRepository(DbProvider provider)
            : base(provider)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="Project"/> in our database.
        /// </summary>
        /// <param name="project">The project to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created project with id assigned.</returns>
        public async Task<Project> CreateAsync(Project project, CancellationToken token)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            // TODO Implement.
            var sql = @"
                INSERT INTO entities.project (
    
                )
                VALUES (
    
                )
                RETURNING id;
                ";

            await using var connection = await DbProvider.OpenConnectionScopeAsync(token);
            await using var command = DbProvider.CreateCommand(sql, connection);

            MapToWriter(command, project);

            await using var reader = await command.ExecuteNonQueryAsync(token);
            await reader.ReadAsync();

            return reader.GetSafeString(0);
        }

        public Task DeleteAsync(Guid id, CancellationToken token) => throw new NotImplementedException();

        public Task<bool> ExistsAsync(Guid id, CancellationToken token) => throw new NotImplementedException();

        public Task<IEnumerable<Project>> GetAllAsync(CancellationToken token) => throw new NotImplementedException();

        public Task<Project> GetAsync(Guid id, CancellationToken token) => throw new NotImplementedException();

        public Task<Project> MarkProjectAsync(Guid id, ProjectStatus status, CancellationToken token) => throw new NotImplementedException();

        public Task<Project> UpdateAsync(Project project, CancellationToken token) => throw new NotImplementedException();

        // TODO Implement.
        /// <summary>
        ///     Maps from a <see cref="DbDataReader"/> to a <see cref="Project"/>.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped project.</returns>
        private static Project MapFromReader(DbDataReader reader)
           => new Project
           {
               Id = reader.GetGuid(0),
           };

        // TODO Implement.
        /// <summary>
        ///     Maps a <see cref="Project"/> to a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The command to write to.</param>
        /// <param name="project">The project to write to the command.</param>
        private static void MapToWriter(DbCommand command, Project project)
        {
            // command.AddParameterWithValue("my_id", project.MyId);
        }
    }
}
