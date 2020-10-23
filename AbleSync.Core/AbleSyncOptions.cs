using System;

namespace AbleSync.Core
{
    /// <summary>
    ///     Options file for the global application.
    /// </summary>
    public sealed class AbleSyncOptions
    {
        /// <summary>
        ///     The root directory.
        /// </summary>
        public Uri RootDirectoryPath { get; set; }

        /// <summary>
        ///     Interval in minutes between two consecutive
        ///     project scraping cycles.
        /// </summary>
        public uint IntervalScrapingMinutes { get; set; }

        /// <summary>
        ///     Interval in minutes between two consecutive
        ///     project analyzing cycles.
        /// </summary>
        public uint IntervalAnalyzingMinutes { get; set; }

        /// <summary>
        ///     Maximum task executing queue.
        /// </summary>
        public uint TaskExecutionQueueSize { get; set; }
    }
}
