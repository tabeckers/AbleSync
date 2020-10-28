using AbleSync.Core.Exceptions;
using AbleSync.Core.Types;
using RenameMe.Utility.Extensions;
using System;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Infrastructure.Extensions
{
    /// <summary>
    ///     <see cref="DbCommand"/> extensions.
    /// </summary>
    internal static class DbCommandExtensions
    {
        // TODO Add order by as well
        /// <summary>
        ///     Appends pagination to our sql command. Call this after
        ///     creating the entire SQL text.
        /// </summary>
        /// <remarks>
        ///     The <see cref="Pagination"/>
        /// </remarks>
        /// <param name="sql">The sql command text to append to.</param>
        /// <param name="pagination">The pagination.</param>
        public static void AddPagination(ref string sql, Pagination pagination)
        {
            sql.ThrowIfNullOrEmpty();
            if (pagination == null)
            {
                throw new ArgumentNullException(nameof(pagination));
            }

            if (pagination.ItemsPerPage > 0)
            {
                sql += $"\n\tLIMIT {pagination.ItemsPerPage}";
            }

            if (pagination.Page > 0)
            {
                sql += $"\n\tOFFSET {pagination.Page}";
            }
        }

        /// <summary>
        ///     Add parameter with key and value to command.
        /// </summary>
        /// <remarks>
        ///     Will send a null if the value is null.
        /// </remarks>
        /// <param name="command">The command to extend.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        /// <returns>See <see cref="DbParameter"/>.</returns>
        public static DbParameter AddParameterWithValue(this DbCommand command, string parameterName, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            if (value is string && string.IsNullOrEmpty(value as string))
            {
                parameter.Value = DBNull.Value;
            }
            command.Parameters.Add(parameter);
            return parameter;
        }

        // FUTURE: Do not depend on Npgsql. Too npgsql specific.
        public static DbParameter AddJsonParameterWithValue(this DbCommand command, string parameterName, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var parameter = new Npgsql.NpgsqlParameter(parameterName, NpgsqlTypes.NpgsqlDbType.Jsonb)
            {
                Value = value ?? DBNull.Value
            };
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        ///     Executes the query and returns the first column of the first row as unsigned integer.
        /// </summary>
        /// <param name="command">The command to extend.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Unsigned integer.</returns>
        public static async ValueTask<uint> ExecuteScalarUnsignedIntAsync(this DbCommand command, CancellationToken token)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return Convert.ToUInt32(await command.ExecuteScalarEnsureRowAsync(token), CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Execute command and ensure success.
        /// </summary>
        /// <param name="command">The command to extend.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Scalar result.</returns>
        public static async ValueTask<object> ExecuteScalarEnsureRowAsync(this DbCommand command, CancellationToken token)
        {
            var result = await command.ExecuteScalarAsync(token);
            if (result == null)
            {
                throw new EntityNotFoundException();
            }
            return result;
        }

        /// <summary>
        ///     Execute command and ensure success.
        /// </summary>
        /// <param name="command">The command to extend.</param>
        /// <returns><see cref="DbDataReader"/>.</returns>
        public static async ValueTask<DbDataReader> ExecuteReaderAsyncEnsureRowAsync(this DbCommand command)
        {
            DbDataReader reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                throw new EntityNotFoundException();
            }
            return reader;
        }
    }
}
