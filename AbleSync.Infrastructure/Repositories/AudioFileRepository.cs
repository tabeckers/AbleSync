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
    ///     Repository for audio files.
    /// </summary>
    internal sealed class AudioFileRepository : RepositoryBase<AudioFile>, IAudioFileRepository
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AudioFileRepository(DbProvider provider)
            : base(provider)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="AudioFile"/> in our database.
        /// </summary>
        /// <param name="audioFile">The audioFile to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created audioFile with id assigned.</returns>
        public override async Task<Guid> CreateAsync(AudioFile audioFile, CancellationToken token)
        {
            if (audioFile == null)
            {
                throw new ArgumentNullException(nameof(audioFile));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                INSERT INTO entities.audio_file (
                            project_id,
                            name,
                            audio_format
                )
                VALUES (    @project_id,
                            @name,
                            @audio_format
                )
                RETURNING   id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            MapToWriter(command, audioFile);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();
            await reader.ReadAsync(token);

            return reader.GetGuid(0);
        }

        // TODO Implement. Do we even want this functionality?
        public override Task DeleteAsync(Guid id, CancellationToken token) => throw new NotImplementedException();

        /// <summary>
        ///     Checks if any audio files exist for a given 
        ///     project id.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><c>True</c> if any audio file exists.</returns>
        public async Task<bool> ExistsAnyForProjectAsync(Guid projectId, CancellationToken token)
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
                SELECT  count(*)
                FROM    entities.audio_file
                WHERE   project_id = @project_id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("project_id", projectId);

            var count = await command.ExecuteScalarUnsignedIntAsync(token);

            return count > 0;
        }

        /// <summary>
        ///     Checks if a <see cref="AudioFile"/> by a given <paramref name="id"/>
        ///     exists in our database.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns><c>true</c> if the audioFile exists.</returns>
        public override async Task<bool> ExistsAsync(Guid id, CancellationToken token)
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
                FROM    entities.audio_file
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

        /// <summary>
        ///     Checks if an audio file in a specific audio format
        ///     exists for a given project id.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="audioFormat">The audio format to check for.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><c>True</c> if an audio file exists.</returns>
        public async Task<bool> ExistsForProjectAsync(Guid projectId, AudioFormat audioFormat, CancellationToken token)
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
                SELECT  count(*)
                FROM    entities.audio_file
                WHERE   project_id = @project_id
                AND     audio_format = @audio_format";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("project_id", projectId);
            command.AddParameterWithValue("audio_format", audioFormat);

            var count = await command.ExecuteScalarUnsignedIntAsync(token);

            return count > 0;
        }

        // TODO Pagination?
        /// <summary>
        ///     Gets all projects from our database.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of projects.</returns>
        public override async IAsyncEnumerable<AudioFile> GetAllAsync([EnumeratorCancellation] CancellationToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                SELECT  id,
                        project_id,
                        name,
                        audio_format,
                        date_created,
                        date_updated,
                        date_synced
                FROM    entities.audio_file";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();

            while (await reader.ReadAsync(token))
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Gets a single <see cref="AudioFile"/> from our database.
        /// </summary>
        /// <param name="id">Internal audioFile id.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The returned audioFile.</returns>
        public override async Task<AudioFile> GetAsync(Guid id, CancellationToken token)
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
                        name,
                        audio_format,
                        date_created,
                        date_updated,
                        date_synced
                FROM    entities.audio_file
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
        ///     Gets an audio file for a given project and a
        ///     given audio format.
        /// </summary>
        /// <remarks>
        ///     The audio file should exist or this will throw
        ///     an exception.
        /// </remarks>
        /// <param name="projectId">The project id.</param>
        /// <param name="audioFormat">The format to get.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The audio file for the project and format.</returns>
        public async Task<AudioFile> GetFromProjectAsync(Guid projectId, AudioFormat audioFormat, CancellationToken token)
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
                        name,
                        audio_format,
                        date_created,
                        date_updated,
                        date_synced
                FROM    entities.audio_file
                WHERE   project_id = @project_id
                AND     audio_format = @audio_format
                LIMIT   1";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("project_id", projectId);
            command.AddParameterWithValue("audio_format", audioFormat);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();
            await reader.ReadAsync(token);

            return MapFromReader(reader);
        }

        /// <summary>
        ///     Gets the audiofiles from our data store ordered 
        ///     by most recent update date.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Latest audio file collection.</returns>
        public async IAsyncEnumerable<AudioFile> GetLatestAsync([EnumeratorCancellation] CancellationToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                SELECT      id,
                            project_id,
                            name,
                            audio_format,
                            date_created,
                            date_updated,
                            date_synced
                FROM        entities.audio_file
                ORDER BY    date_updated DESC";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            await using var reader = await command.ExecuteReaderAsyncEnsureRowAsync();

            while (await reader.ReadAsync(token))
            {
                yield return MapFromReader(reader);
            }
        }

        /// <summary>
        ///     Marks the sync date of an audio file as now.
        /// </summary>
        /// <param name="id">The audio file id.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task MarkSyncedAsync(Guid id, CancellationToken token)
        {
            id.ThrowIfNullOrEmpty();
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var sql = @"
                UPDATE  entities.audio_file
                SET     date_synced = now()
                WHERE   id = @id";

            await using var connection = await _provider.OpenConnectionScopeAsync(token);
            await using var command = _provider.CreateCommand(sql, connection);

            command.AddParameterWithValue("id", id);

            var affected = await command.ExecuteNonQueryAsync(token);
            if (affected == 0)
            {
                throw new EntityNotFoundException(nameof(AudioFile));
            }
        }

        public override Task UpdateAsync(AudioFile audioFile, CancellationToken token) => throw new NotImplementedException();

        /// <summary>
        ///     Maps from a <see cref="DbDataReader"/> to a <see cref="AudioFile"/>.
        /// </summary>
        /// <param name="reader">The reader to map from.</param>
        /// <returns>The mapped audioFile.</returns>
        private static AudioFile MapFromReader(DbDataReader reader)
           => new AudioFile
           {
               Id = reader.GetGuid(0),
               ProjectId = reader.GetGuid(1),
               Name = reader.GetString(2),
               AudioFormat = reader.GetFieldValue<AudioFormat>(3),
               DateCreated = reader.GetDateTime(4),
               DateUpdated = reader.GetSafeDateTime(5),
               DateSynced = reader.GetDateTime(6)
           };

        /// <summary>
        ///     Maps a <see cref="AudioFile"/> to a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The command to write to.</param>
        /// <param name="audioFile">The audioFile to write to the command.</param>
        private static void MapToWriter(DbCommand command, AudioFile audioFile)
        {
            command.AddParameterWithValue("project_id", audioFile.ProjectId);
            command.AddParameterWithValue("name", audioFile.Name);
            command.AddParameterWithValue("audio_format", audioFile.AudioFormat);
        }
    }
}
