using System;

namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     Represents an Ableton project.
    /// </summary>
    public class Project : EntityBase, IEntityAudit
    {
        /// <summary>
        ///     Project name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The internal id of the <see cref="Artist"/> that created this project.
        /// </summary>
        public Guid ArtistId { get; set; }

        /// <summary>
        ///     Relative path with regards to the base path.
        /// </summary>
        public Uri RelativePath { get; set; }

        /// <summary>
        ///     Creation time.
        /// </summary>
        public DateTimeOffset? DateCreated { get; set; }

        /// <summary>
        ///     Latest update time.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; set; }
    }
}
