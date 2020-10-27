using System;

namespace AbleSync.Api.DataTransferObjects
{
    /// <summary>
    ///     Artist DTO.
    /// </summary>
    public sealed class ArtistDTO
    {
        /// <summary>
        ///     Internal identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The artist name or alias.
        /// </summary>
        public string Name { get; set; }
    }
}
