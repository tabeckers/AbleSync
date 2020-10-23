using AbleSync.Core.Entities;
using AbleSync.Core.Exceptions;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Types;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository for <see cref="ProjectTask"/> entities.
    /// </summary>
    internal sealed class ProjectTaskRepository : RepositoryBase<ProjectTask>, IProjectTaskRepository
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectTaskRepository(DbProvider provider)
            : base(provider)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="ProjectTask"/> in our data store.
        /// </summary>
        /// <param name="projectTask">See <see cref="Project"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created <see cref="ProjectTask"/>.</returns>
        public override async Task<Guid> CreateAsync(ProjectTask projectTask, CancellationToken token)
        {
            if (projectTask == null)
            {
                throw new ArgumentNullException(nameof(projectTask));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            // TODO Pick a format.
            var sql = @"
                INSERT INTO entities.project_task (
                            project_id,
                            project_task_type,
                            task_parameter
                )
                VALUES (    @project_id,
                            @project_task_type,
                            @task_parameter
                )
                RETURNING   id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("project_id", projectTask.ProjectId);
            command.AddParameterWithValue("project_task_type", projectTask.ProjectTaskType);
            command.AddParameterWithValue("task_parameter", projectTask.TaskParameter);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();
            await reader.ReadAsync(token);

            return reader.GetGuid(0);
        }

        public override Task DeleteAsync(Guid id, CancellationToken token) => throw new NotImplementedException();

        public override Task<bool> ExistsAsync(Guid id, CancellationToken token) => throw new NotImplementedException();

        // TODO Make async enumerable?
        /// <summary>
        ///     Gets all <see cref="ProjectTask"/>s for a given <see cref="Project"/>.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of <see cref="Project"/>s.</returns>
        public async IAsyncEnumerable<ProjectTask> GetAllForProjectAsync(Guid projectId, [EnumeratorCancellation] CancellationToken token)
        {
            if (projectId == null || projectId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(projectId));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                SELECT  id,
                        project_id,
                        date_created,
                        date_updated,
                        date_completed,
                        project_task_status,
                        project_task_type,
                        task_parameter
                FROM    entities.project_task
                WHERE   project_id = @project_id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("project_id", projectId);

            await using var reader = await command.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a <see cref="ProjectTask"/> from our data store.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The returned <see cref="ProjectTask"/>.</returns>
        public override async Task<ProjectTask> GetAsync(Guid id, CancellationToken token)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                SELECT  id,
                        project_id,
                        date_created,
                        date_updated,
                        date_completed,
                        project_task_status,
                        project_task_type,
                        task_parameter
                FROM    entities.project_task
                WHERE   id = @id
                LIMIT   1";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();
            await reader.ReadAsync(token);

            return MapFromReader(reader);
        }

        /// <summary>
        ///     Marks the status of a <see cref="ProjectTask"/>.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="status">The new <see cref="ProjectTaskStatus"/>.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The <see cref="ProjectTask"/> after the status update.</returns>
        public async Task<ProjectTask> MarkStatusAsync(Guid id, ProjectTaskStatus status, CancellationToken token)
        {
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                UPDATE  entities.project_task
                SET     project_task_status = @project_task_status
                WHERE   id = @id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("id", id);
            command.AddParameterWithValue("project_task_status", status);

            var affected = await command.ExecuteNonQueryAsync(token);
            if (affected == 0)
            {
                throw new EntityNotFoundException(nameof(Project));
            }

            // Get the updated object from the datastore.
            return await GetAsync(id, token);
        }

        /// <summary>
        ///     Gets all project tasks from our database.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of project tasks.</returns>
        public override async IAsyncEnumerable<ProjectTask> GetAllAsync([EnumeratorCancellation] CancellationToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                SELECT  id,
                        project_id,
                        date_created,
                        date_updated,
                        date_completed,
                        project_task_status,
                        project_task_type,
                        task_parameter
                FROM    entities.project_task";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            await using var reader = await command.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                yield return MapFromReader(reader);
            }
        }

        public override Task UpdateAsync(ProjectTask entity, CancellationToken token) => throw new NotImplementedException();

        /// <summary>
        ///     Maps from a <see cref="DbDataReader"/> to a <see cref="ProjectTask"/>.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped project.</returns>
        private static ProjectTask MapFromReader(DbDataReader reader)
           => new ProjectTask
           {
               Id = reader.GetGuid(0),
               ProjectId = reader.GetGuid(1),
               DateCreated = reader.GetDateTime(2),
               DateUpdated = reader.GetSafeDateTime(3),
               DateCompleted = reader.GetSafeDateTime(4),
               ProjectTaskStatus = reader.GetFieldValue<ProjectTaskStatus>(5),
               ProjectTaskType = reader.GetFieldValue<ProjectTaskType>(6),
               TaskParameter = reader.GetSafeString(7),
           };

    }
}
