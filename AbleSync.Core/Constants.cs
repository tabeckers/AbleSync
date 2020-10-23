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

        /// <summary>
        ///     Project folders storage folder name.
        /// </summary>
        public const string StorageProjectFolderBase = "projects";

        /// <summary>
        ///     Audio file storage folder name.
        /// </summary>
        public const string StorageAudioFilesFolder = "audiofiles";

        public const string ContentTypeMp3 = "audio/mpeg3";

        public const string ContentTypeWav = "audio/wav";

        public const string ContentTypeFlac = "audio/flac";
    }
}
