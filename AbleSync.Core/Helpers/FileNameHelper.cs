using AbleSync.Core.Types;
using System;

namespace AbleSync.Core.Helpers
{
    // TODO Bad name.
    /// <summary>
    ///     Contains generic file name mapping functionality.
    /// </summary>
    public static class FileNameHelper
    {
        /// <summary>
        ///     Translates an audio format to the corresponding
        ///     file extension.
        /// </summary>
        /// <param name="audioFormat">The audio format.</param>
        /// <returns>The file extension, including the '.' dot.</returns>
        public static string ToExtension(AudioFormat audioFormat)
            => audioFormat switch
            {
                AudioFormat.Mp3 => Constants.AudioMp3FileExtension,
                AudioFormat.Wav => Constants.AudioWavFileExtension,
                AudioFormat.Flac => Constants.AudioFlacFileExtension,
                _ => throw new InvalidOperationException(nameof(audioFormat))
            };
    }
}
