using AbleSync.Core.Entities;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Types;
using AbleSync.Infrastructure.Provider;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Infrastructure.Repositories
{
    // Implementations of the repo base have a lot of duplicate code.
    /// <summary>
    ///     Base class for all repositories.
    /// </summary>
    internal abstract class RepositoryBase<TEntity> : IRepositoryBaseCRUD<TEntity>
        where TEntity : EntityBase
    {
        protected DbProvider _provider;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public RepositoryBase(DbProvider provider) => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        ///     Creates an entity in our data store and returns
        ///     the id of the created entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created entity id.</returns>
        public abstract Task<Guid> CreateAsync(TEntity entity, CancellationToken token);

        /// <summary>
        ///     Creates an entity and fetched it from the data store afterwards.
        ///     This calls <see cref="CreateGetAsync(TEntity, CancellationToken)"/>,
        ///     then calls <see cref="GetAsync(Guid, CancellationToken)"/>.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created entity fetched from the data store.</returns>
        public virtual async Task<TEntity> CreateGetAsync(TEntity entity, CancellationToken token)
        {
            var id = await CreateAsync(entity, token);
            return await GetAsync(id, token);
        }

        /// <summary>
        ///     Deletes an entity from our data store.
        /// </summary>
        /// <param name="id">Internal entity id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        public abstract Task DeleteAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Checks if a given entity exists in our data store.
        /// </summary>
        /// <param name="id">Internal enitity id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><c>true</c> if the entity exists.</returns>
        public abstract Task<bool> ExistsAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Gets all entities from our data store.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of entities.</returns>
        public abstract IAsyncEnumerable<TEntity> GetAllAsync(Pagination pagination, CancellationToken token);

        /// <summary>
        ///     Gets an entity from our data store.
        /// </summary>
        /// <param name="id">Internal entity id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The returned entity.</returns>
        public abstract Task<TEntity> GetAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Updates an entity in our data store.
        /// </summary>
        /// <param name="entity">The new to-update entity.</param>
        /// <param name="token">The cancellation token.</param>
        public abstract Task UpdateAsync(TEntity entity, CancellationToken token);

        /// <summary>
        ///     Updates an entity and fetches it from the data store afterwards.
        ///     This calls <see cref="UpdateAsync(TEntity, CancellationToken)"/>,
        ///     then calls <see cref="GetAsync(Guid, CancellationToken)"/>.
        /// </summary>
        /// <param name="entity">The entity with updated parameters.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated entity fetched from the data store.</returns>
        public virtual async Task<TEntity> UpdateGetAsync(TEntity entity, CancellationToken token)
        {
            await UpdateAsync(entity, token);
            return await GetAsync(entity.Id, token);
        }
    }
}
