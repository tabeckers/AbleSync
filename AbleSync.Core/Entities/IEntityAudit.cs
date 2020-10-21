using System;

namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     Interface for audit metadata for an entity.
    /// </summary>
    public interface IEntityAudit
    {
        /// <summary>
        ///     Creation time.
        /// </summary>
        DateTimeOffset DateCreated { get; set; }

        /// <summary>
        ///     Latest update time.
        /// </summary>
        DateTimeOffset? DateUpdated { get; set; }
    }
}
