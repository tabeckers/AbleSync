using AbleSync.Core.Entities;
using AbleSync.Core.Exceptions;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Types;
using AbleSync.Infrastructure.Extensions;
using AbleSync.Infrastructure.Provider;
using RenameMe.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
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
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            // TODO Pick a format.
            var sql = @"
                INSERT INTO entities.project (
                            artist_id,
                            name,
                            relative_path 
                )
                VALUES (    @artist_id,
                            @name,
                            @relative_path
                )
                RETURNING   id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            MapToWriter(command, project);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();
            await reader.ReadAsync(token);

            var id = reader.GetGuid(0);

            return await GetAsync(id, token);
        }

        // TODO Implement. Do we even want this functionality?
        public Task DeleteAsync(Guid id, CancellationToken token) => throw new NotImplementedException();

        /// <summary>
        ///     Checks if a <see cref="Project"/> by a given <paramref name="id"/>
        ///     exists in our database.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns><c>true</c> if the project exists.</returns>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken token)
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
                SELECT  count(*)
                FROM    entities.project
                WHERE   id = @id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("id", id);

            var count = await command.ExecuteScalarUnsignedIntAsync(token);
            if (count > 1)
            {
                // FUTURE Custom exception for this?
                throw new InvalidOperationException("Found more than one entity when looking for a single item");
            }

            return count == 1;
        }

        // TODO Pagination?
        /// <summary>
        ///     Gets all projects from our database.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of projects.</returns>
        public async IAsyncEnumerable<Project> GetAllAsync([EnumeratorCancellation] CancellationToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                SELECT  id,
                        name,
                        artist_id,
                        relative_path,
                        date_created,
                        date_updated,
                        project_status
                FROM    entities.project";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();

            while (await reader.ReadAsync(token))
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a single <see cref="Project"/> from our database.
        /// </summary>
        /// <param name="id">Internal project id.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The returned project.</returns>
        public async Task<Project> GetAsync(Guid id, CancellationToken token)
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
                        name,
                        artist_id,
                        relative_path,
                        date_created,
                        date_updated,
                        project_status
                FROM    entities.project
                WHERE   id = @id
                LIMIT   1";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();
            await reader.ReadAsync(token);

            return MapFromReader(reader);
        }

        // TODO Maybe add a separate column for scrape dates?
        /// <summary>
        ///     Mark a project as scraped by settings its update
        ///     date to now().
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated project as fetched from the data store.</returns>
        public async Task<Project> MarkProjectAsScrapedAsync(Guid id, CancellationToken token)
        {
            id.ThrowIfNullOrEmpty();
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                UPDATE  entities.project
                SET     date_updated = now()
                WHERE   id = @id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("id", id);

            var affected = await command.ExecuteNonQueryAsync(token);
            if (affected == 0)
            {
                throw new EntityNotFoundException(nameof(Project));
            }

            // Get the updated object from the datastore.
            return await GetAsync(id, token);
        }

        /// <summary>
        ///     Marks a <see cref="Project"/> with a given <paramref name="projectStatus"/>.
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <param name="projectStatus">The status to mark the project as.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The updated and marked <see cref="Project"/>.</returns>
        public async Task<Project> MarkProjectAsync(Guid id, ProjectStatus projectStatus, CancellationToken token)
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
                UPDATE  entities.project
                SET     project_status = @project_status
                WHERE   id = @id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("id", id);
            command.AddParameterWithValue("project_status", projectStatus);

            var affected = await command.ExecuteNonQueryAsync(token);
            if (affected == 0)
            {
                throw new EntityNotFoundException(nameof(Project));
            }

            // Get the updated object from the datastore.
            return await GetAsync(id, token);
        }

        public Task<Project> UpdateAsync(Project project, CancellationToken token)
            => throw new NotImplementedException();

        /// <summary>
        ///     Maps from a <see cref="DbDataReader"/> to a <see cref="Project"/>.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped project.</returns>
        private static Project MapFromReader(DbDataReader reader)
           => new Project
           {
               Id = reader.GetGuid(0),
               Name = reader.GetString(1),
               ArtistId = reader.GetSafeGuid(2),
               RelativePath = reader.GetString(3),
               DateCreated = reader.GetDateTime(4),
               DateUpdated = reader.GetSafeDateTime(5),
               ProjectStatus = reader.GetFieldValue<ProjectStatus>(6),
           };

        /// <summary>
        ///     Maps a <see cref="Project"/> to a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The command to write to.</param>
        /// <param name="project">The project to write to the command.</param>
        private static void MapToWriter(DbCommand command, Project project)
        {
            command.AddParameterWithValue("artist_id", project.ArtistId);
            command.AddParameterWithValue("name", project.Name);
            command.AddParameterWithValue("relative_path", project.RelativePath);
        }

    }
}
