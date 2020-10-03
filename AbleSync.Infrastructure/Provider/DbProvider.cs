using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Infrastructure.Provider
{
    /// <summary>
    ///     Base class for a database connection provider.
    /// </summary>
    internal abstract class DbProvider
    {
        /// <summary>
        ///     Contains our database connection string.
        /// </summary>
        protected string ConnectionString { get; }

        public DbProvider(IOptions<DbProviderOptions> options, IConfiguration configuration)
        {
            if (options == null || options.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // TODO Format error.
            ConnectionString = configuration.GetConnectionString(options.Value.ConnectionStringName) ?? throw new ArgumentNullException("Can't find connection string");
        }

        /// <summary>
        ///     Opens or gets a database connection.
        /// </summary>
        /// <returns>The <see cref="DbConnection"/>.</returns>
        public abstract DbConnection GetConnectionScope();

        /// <summary>
        ///     Opens a new connection scope with cancellation ability.
        /// </summary>
        /// <param name="token">See <see cref="CancellationToken"/>.</param>
        /// <returns>The opened <see cref="DbConnection"/>.</returns>
        public virtual async Task<DbConnection> OpenConnectionScopeAsync(CancellationToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var connection = GetConnectionScope();
            await connection.OpenAsync(token);
            return connection;
        }

        /// <summary>
        ///     Create command on the database connection.
        /// </summary>
        /// <param name="commandText">The text of the query.</param>
        /// <param name="connection">Database connection, see <see cref="DbConnection"/>.</param>
        /// <returns>See <see cref="DbCommand"/>.</returns>
        public virtual DbCommand CreateCommand(string commandText, DbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var cmd = connection.CreateCommand();

            // TODO SQL injection.
            cmd.CommandText = commandText;
            return cmd;
        }
    }
}
