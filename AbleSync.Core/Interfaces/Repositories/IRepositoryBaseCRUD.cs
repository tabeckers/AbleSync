using AbleSync.Core.Entities;
using AbleSync.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Base interface for an entity repository.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepositoryBaseCRUD<TEntity>
        where TEntity : EntityBase
    {
        /// <summary>
        ///     Creates an entity in our data store and returns
        ///     the id of the created entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created entity id.</returns>
        Task<Guid> CreateAsync(TEntity entity, CancellationToken token);

        /// <summary>
        ///     Creates an entity in our data store and gets
        ///     it from the data store right after creation.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created entity.</returns>
        Task<TEntity> CreateGetAsync(TEntity entity, CancellationToken token);

        /// <summary>
        ///     Deletes an entity from our data store.
        /// </summary>
        /// <param name="id">Internal entity id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        Task DeleteAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Checks if a given entity exists in our data store.
        /// </summary>
        /// <param name="id">Internal enitity id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><c>true</c> if the entity exists.</returns>
        Task<bool> ExistsAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Gets all entities from our data store.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Collection of entities.</returns>
        IAsyncEnumerable<TEntity> GetAllAsync(Pagination pagination, CancellationToken token);

        /// <summary>
        ///     Gets an entity from our data store.
        /// </summary>
        /// <param name="id">Internal entity id.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The returned entity.</returns>
        Task<TEntity> GetAsync(Guid id, CancellationToken token);

        /// <summary>
        ///     Updates an entity in our data store.
        /// </summary>
        /// <param name="entity">The new to-update entity.</param>
        /// <param name="token">The cancellation token.</param>
        Task UpdateAsync(TEntity entity, CancellationToken token);

        /// <summary>
        ///     Updates an entity in our data store and returns 
        ///     the enity from our data store right after.
        /// </summary>
        /// <param name="entity">The new to-update entity.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated entity as fetched from the data store.</returns>
        Task<TEntity> UpdateGetAsync(TEntity entity, CancellationToken token);
    }
}
