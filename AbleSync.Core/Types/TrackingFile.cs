using System;

namespace AbleSync.Core.Types
{
    /// <summary>
    ///     Represents a tracking file for a project.
    /// </summary>
    // TODO Do we want this in here?
    [Serializable]
    public sealed class TrackingFile
    {
        /// <summary>
        ///     The internal project identifier.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        ///     The creation date of this tracking file.
        /// </summary>
        public DateTimeOffset TrackingFileDateCreated { get; set; }

        /// <summary>
        ///     The update date of this tracking file.
        /// </summary>
        public DateTimeOffset? TrackingFileDateUpdated { get; set; }

        /// <summary>
        ///     The last moment this project was scraped.
        /// </summary>
        public DateTimeOffset ProjectDateScraped { get; set; }

        /// <summary>
        ///     The last moment this project was analyzed
        ///     for which tasks should be executed.
        /// </summary>
        public DateTimeOffset? ProjectDateAnalyzed { get; set; }

        /// <summary>
        ///     Indicates the status of this tracking file.
        /// </summary>
        public TrackingFileStatus TrackingFileStatus { get; set; }
    }
}
