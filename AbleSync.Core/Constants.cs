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
        ///     MP3 extension.
        /// </summary>
        public const string AudioMp3FileExtension = ".mp3";

        /// <summary>
        ///     WAV extension
        /// </summary>
        public const string AudioWavFileExtension = ".wav";

        /// <summary>
        ///     FLAC extension.
        /// </summary>
        public const string AudioFlacFileExtension = ".flac";

        /// <summary>
        ///     All known Ableton audio file extensions for exported files.
        /// </summary>
        public static readonly string[] ExportedAudioFileExtensions = {
                AudioMp3FileExtension,
                AudioWavFileExtension,
                AudioFlacFileExtension
            };

        // TODO Implement structure.
        /// <summary>
        ///     Project folders storage base folder..
        /// </summary>
        public const string StorageProjectFolderBase = "projects";
    }
}
