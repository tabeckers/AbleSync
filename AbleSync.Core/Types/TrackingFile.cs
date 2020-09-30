﻿using System;

namespace AbleSync.Core.Types
{
    /// <summary>
    ///     Represents a tracking file for a project.
    /// </summary>
    public sealed class TrackingFile
    {
        /// <summary>
        ///     The internal project identifier.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        ///     Represents the last moment this was updated and synced.
        /// </summary>
        public DateTimeOffset DateUpdated { get; set; }
    }
}
