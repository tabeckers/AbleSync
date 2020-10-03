using System;

namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     Abstract base class that represents a processing task for a given
    ///     <see cref="Project"/>.
    /// </summary>
    public abstract class ProjectTask : EntityBase, IEntityAudit
    {
        /// <summary>
        ///     The referenced <see cref="Project"/> to which this task belongs.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        ///     Represents whether or not this item has been completed.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        ///     Creation time.
        /// </summary>
        public DateTimeOffset? DateCreated { get; set; }

        /// <summary>
        ///     Latest update time.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; set; }

        /// <summary>
        ///     Task completion time.
        /// </summary>
        public DateTimeOffset? DateCompleted { get; set; }
    }
}
