namespace AbleSync.Core.Entities
{
    /// <summary>
    ///     Represents an artist.
    /// </summary>
    public sealed class Artist : EntityBase
    {
        /// <summary>
        ///     The artist name or alias.
        /// </summary>
        public string Name { get; set; }
    }
}
