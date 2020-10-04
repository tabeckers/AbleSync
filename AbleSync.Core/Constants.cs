namespace AbleSync.Core
{
    /// <summary>
    ///     Contains application global constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     The name for the tracking file.
        /// </summary>
        public const string TrackingFileExtension = ".asi";

        /// <summary>
        ///     All known Ableton audio file extensions for exported files.
        /// </summary>
        public static readonly string[] ExportedAudioFileExtensions = { ".mp3", ".wav", ".flac" };
    }
}
