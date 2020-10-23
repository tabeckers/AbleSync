using RenameMe.Utility.Extensions;
using System;

namespace AbleSync.Core.Helpers
{
    /// <summary>
    ///     Contains centralized functionality with regards
    ///     to file storage and file names.
    /// </summary>
    public static class FileStorageHelper
    {
        /// <summary>
        ///     Generates the filename of an audio file based on 
        ///     the audio file id.
        /// </summary>
        /// <param name="audioFileId">The audio file id.</param>
        /// <returns>The storage file name.</returns>
        public static string AudioFileName(Guid audioFileId)
        {
            audioFileId.ThrowIfNullOrEmpty();
            return $"{audioFileId}";
        }

        /// <summary>
        ///     Generates the audio file folder name based on
        ///     a given project id.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <returns>The audio file folder name for the project.</returns>
        public static string AudioFileFolder(Guid projectId)
        {
            projectId.ThrowIfNullOrEmpty();
            return $"{Constants.StorageProjectFolderBase}/{projectId}/{Constants.StorageAudioFilesFolder}";
        }
    }
}
