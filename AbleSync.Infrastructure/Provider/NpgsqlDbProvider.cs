using AbleSync.Core.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;

namespace AbleSync.Infrastructure.Provider
{
    /// <summary>
    ///     Postgresql implementation of <see cref="DbProvider"/>.
    /// </summary>
    internal sealed class NpgsqlDbProvider : DbProvider
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NpgsqlDbProvider(IOptions<DbProviderOptions> options, IConfiguration configuration)
            : base(options, configuration)
        {
        }

        // FUTURE: Move somewhere. Too npgsql specific
        /// <summary>
        ///     Static initializer.
        /// </summary>
        static NpgsqlDbProvider()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<AudioFormat>("entities.audio_format");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ProjectStatus>("entities.project_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ProjectTaskStatus>("entities.project_task_status");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<ProjectTaskType>("entities.project_task_type");
        }

        /// <summary>
        ///     Opens a new <see cref="NpgsqlConnection"/>.
        /// </summary>
        /// <returns>The opened <see cref="NpgsqlConnection"/>.</returns>
        public override DbConnection GetConnectionScope() => new NpgsqlConnection(ConnectionString);

        // TODO Check for SQL injection.
        /// <summary>
        ///     Create a new Npgsql command on the database connection.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">Database connection, see <see cref="DbConnection"/>.</param>
        /// <returns>See <see cref="DbCommand"/>.</returns>
        public override DbCommand CreateCommand(string cmdText, DbConnection connection)
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            => new NpgsqlCommand(cmdText, connection as NpgsqlConnection);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
    }
}
