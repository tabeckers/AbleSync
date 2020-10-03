using AbleSync.Core.Exceptions;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace AbleSync.Core.Services
{
    // TODO Custom exceptions.
    /// <summary>
    ///     Service for managing tracking files.
    /// </summary>
    /// <remarks>
    ///     This only operates locally and has no knowledge of any data store.
    /// </remarks>
    public class FileTrackingService : IFileTrackingService
    {
        /// <summary>
        ///     Creates a new tracking file.
        /// </summary>
        /// <remarks>
        ///     If the project already contains a <see cref="TrackingFile"/>
        ///     this will throw an <see cref="InvalidOperationException"/>.
        /// </remarks>
        /// <param name="projectId">The internal project id.</param>
        /// <param name="directoryInfo">The directory of the project.</param>
        /// <returns>The created <see cref="TrackingFile"/>.</returns>
        public TrackingFile CreateTrackingFile(Guid projectId, DirectoryInfo directoryInfo)
        {
            if (projectId == null || projectId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(projectId));
            }
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            if (HasTrackingFile(directoryInfo))
            {
                throw new InvalidOperationException("Project already has a tracking file");
            }

            // TODO Pass required properties to this file.
            var trackingFile = new TrackingFile
            {
                ProjectId = projectId,
            };

            WriteToFile(directoryInfo, trackingFile);
            return trackingFile;
        }

        /// <summary>
        ///     Deletes a <see cref="TrackingFile"/>.
        /// </summary>
        /// <param name="directoryInfo"></param>
        public void DeleteTrackingFile(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            if (!HasTrackingFile(directoryInfo))
            {
                throw new TrackingFileNotFoundException();
            }

            var trackingFile = GetTrackingFile(directoryInfo);
            File.Delete($"{directoryInfo.FullName}/{trackingFile.ProjectId}{Constants.TrackingFileExtension}");
        }

        /// <summary>
        ///     Gets a <see cref="TrackingFile"/> from a project directory.
        /// </summary>
        /// <remarks>
        ///     This throws a <see cref="TrackingFileNotFoundException"/> if the
        ///     directory does not contain a <see cref="TrackingFile"/>.
        /// </remarks>
        /// <param name="directoryInfo">The directory to check.</param>
        /// <returns>The retrieved <see cref="TrackingFile"/>.</returns>
        public TrackingFile GetTrackingFile(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            var files = directoryInfo.GetFiles();
            var trackingFileCandidates = files.Where(x => x.Extension == Constants.TrackingFileExtension);

            if (!trackingFileCandidates.Any())
            {
                throw new TrackingFileNotFoundException();
            }
            if (trackingFileCandidates.Count() > 1)
            {
                throw new InvalidOperationException("Can't have more than one tracking file");
            }

            var trackingFileName = trackingFileCandidates.First();
            var projectId = trackingFileName.Name.Replace(Constants.TrackingFileExtension, "", StringComparison.InvariantCulture);

            var path = $"{directoryInfo.FullName}/{projectId}{Constants.TrackingFileExtension}";
            using var stream = new FileStream(path, FileMode.Open);

            var formatter = new BinaryFormatter();
            return (TrackingFile)formatter.Deserialize(stream);
        }

        /// <summary>
        ///     Checks if a directory contains a tracking file.
        /// </summary>
        /// <param name="directoryInfo">The directory to check.</param>
        /// <returns><c>true</c> if it contains a tracking file.</returns>
        public bool HasTrackingFile(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            var files = directoryInfo.GetFiles();
            var trackingFileCandidates = files.Where(x => x.Extension == Constants.TrackingFileExtension);

            if (!trackingFileCandidates.Any())
            {
                return false;
            }
            if (trackingFileCandidates.Count() > 1)
            {
                throw new InvalidOperationException("Can't have more than one tracking file");
            }

            return true;
        }

        // TODO Duplicate code.
        /// <summary>
        ///     This updates the <see cref="TrackingFile"/> by overwriting it.
        /// </summary>
        /// <remarks>
        ///     Currently this simply deletes and re-creates the file.
        /// </remarks>
        /// <param name="directoryInfo">The directory.</param>
        /// <param name="trackingFile">The new tracking file.</param>
        /// <returns>The written tracking file.</returns>
        public TrackingFile UpdateTrackingFile(DirectoryInfo directoryInfo, TrackingFile trackingFile)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }
            if (trackingFile == null)
            {
                throw new ArgumentNullException(nameof(trackingFile));
            }

            if (!HasTrackingFile(directoryInfo))
            {
                throw new InvalidOperationException("Tracking file did not exist during update call");
            }

            DeleteTrackingFile(directoryInfo);
            WriteToFile(directoryInfo, trackingFile);
            return trackingFile;
        }

        /// <summary>
        ///     Actually writes a <see cref="TrackingFile"/> to file.
        /// </summary>
        /// <param name="directoryInfo">The directory.</param>
        /// <param name="trackingFile">The tracking file to write.</param>
        private void WriteToFile(DirectoryInfo directoryInfo, TrackingFile trackingFile)
        {
            var path = $"{directoryInfo.FullName}/{trackingFile.ProjectId}{Constants.TrackingFileExtension}";
            using var stream = new FileStream(path, FileMode.Create);

            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, trackingFile);
        }
    }
}
