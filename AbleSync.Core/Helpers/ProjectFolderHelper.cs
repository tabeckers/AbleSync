using AbleSync.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AbleSync.Core.Helpers
{
    // TODO This is Windows specific at the moment.
    // TODO This should look at file/contenttype instead of extensions.
    /// <summary>
    ///     Helper class for recognizing Ableton project folders.
    /// </summary>
    public static class ProjectFolderHelper
    {
        /// <summary>
        ///     Checks if a directory is an Ableton project directory.
        /// </summary>
        /// <param name="directoryInfo">The directory to check.</param>
        /// <returns><c>true</c> if the directory is an Ableton project directory.</returns>
        public static bool IsAbletonProjectFolder(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            try
            {
                var subDirectories = directoryInfo.GetDirectories();
                var files = directoryInfo.GetFiles();

                // Check for an actual Ableton project file.
                if (!files.Where(x => x.Extension == AbletonConstants.ProjectFileExtension).Any())
                {
                    return false;
                }

                // Check for the project info directory.
                if (!DoesDirectoryContainDirectory(subDirectories, AbletonConstants.ProjectInfoDirectoryName))
                {
                    return false;
                }
                else
                {
                    var projectInfoDirectory = subDirectories.Where(x => x.Name == AbletonConstants.ProjectInfoDirectoryName).First();
                    if (!DoesDirectoryContainFile(projectInfoDirectory, AbletonConstants.ProjectInfoFileName, AbletonConstants.ProjectInfoFileExtension) ||
                        !DoesDirectoryContainFile(projectInfoDirectory, AbletonConstants.ProjectInfoIconFileName, AbletonConstants.ProjectInfoIconFileExtension))
                    {
                        return false;
                    }
                }

                // If we have passed all checks the directory must be a project folder.
                return true;
            }
            catch (IOException e)
            {
                throw new FileAccessException("Could not determine if directory was Ableton project", e);
            }
        }

        /// <summary>
        ///     Checks if a collection of directories contains a specific directory
        ///     with a given name.
        /// </summary>
        /// <remarks>
        ///     This does not check for any duplicates.
        /// </remarks>
        /// <param name="subDirectories">The directory collection.</param>
        /// <param name="directoryName">The directory name to check for.</param>
        /// <returns><c>true</c> if it exists in the collection.</returns>
        private static bool DoesDirectoryContainDirectory(IEnumerable<DirectoryInfo> subDirectories, string directoryName)
            => subDirectories.Where(x => x.Name == directoryName).Any();

        /// <summary>
        ///     Checks if a given directory contains a specific file.
        /// </summary>
        /// <remarks>
        ///     This does not check for any duplicates.
        /// </remarks>
        /// <param name="directory">The directory to check.</param>
        /// <param name="fileName">The file name to check.</param>
        /// <param name="fileExtension">The file extension to check.</param>
        /// <returns><c>true</c> if the directory contains the file.</returns>
        private static bool DoesDirectoryContainFile(DirectoryInfo directory, string fileName, string fileExtension)
            => directory.GetFiles().Where(x => x.Name == $"{fileName}{fileExtension}").Any();
    }
}
