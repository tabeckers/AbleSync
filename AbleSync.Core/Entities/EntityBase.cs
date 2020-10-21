using System;

namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     Base for an entity. All entities will inherit from this class.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        ///     Entity identifier.
        /// </summary>
        public Guid Id { get; set; }
    }
}
