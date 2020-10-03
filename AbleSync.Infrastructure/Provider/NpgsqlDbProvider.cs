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

        /// <summary>
        ///     Static initializer.
        /// </summary>
        static NpgsqlDbProvider()
        {
            // TODO Add mapping here.
        }

        /// <summary>
        ///     Opens a new <see cref="NpgsqlConnection"/>.
        /// </summary>
        /// <returns>The opened <see cref="NpgsqlConnection"/>.</returns>
        public override DbConnection GetConnectionScope() => new NpgsqlConnection(ConnectionString);

        /// <summary>
        ///     Create a new Npgsql command on the database connection.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">Database connection, see <see cref="DbConnection"/>.</param>
        /// <returns>See <see cref="DbCommand"/>.</returns>
        public override DbCommand CreateCommand(string cmdText, DbConnection connection)
            => new NpgsqlCommand(cmdText, connection as NpgsqlConnection);
    }
}
