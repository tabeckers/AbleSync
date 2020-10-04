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
    }
}
