using AbleSync.Core.Types;
using System;

namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     Represents a single audio file.
    /// </summary>
    public class AudioFile : EntityBase, IEntityAudit
    {
        /// <summary>
        ///     Full file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Identifier of the <see cref="Project"/> this belongs to.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        ///     Represents the format of this audio file.
        /// </summary>
        public AudioFormat AudioFormat { get; set; }

        /// <summary>
        ///     Creation time.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        ///     Latest update time.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; set; }

        /// <summary>
        ///     Latest sync time.
        /// </summary>
        public DateTimeOffset DateSynced { get; set; }
    }
}
