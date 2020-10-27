using System;

namespace AbleSync.Api.DataTransferObjects
{
    /// <summary>
    ///     Ableton project DTO.
    /// </summary>
    public sealed class ProjectDTO
    {
        /// <summary>
        ///     Internal project id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Project name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The internal id of the <see cref="Artist"/> that created this project.
        /// </summary>
        public Guid? ArtistId { get; set; }

        /// <summary>
        ///     Creation time.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        ///     Latest update time.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; set; }
    }
}
