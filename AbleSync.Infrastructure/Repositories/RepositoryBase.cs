using AbleSync.Infrastructure.Provider;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbleSync.Infrastructure.Repositories
{
    /// <summary>
    ///     Base class for all repositories.
    /// </summary>
    internal abstract class RepositoryBase
    {
        protected DbProvider _provider;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public RepositoryBase(DbProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
    }
}
