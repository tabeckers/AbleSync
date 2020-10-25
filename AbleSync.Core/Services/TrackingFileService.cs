using AbleSync.Core.Entities;
using AbleSync.Core.Exceptions;
using AbleSync.Core.Helpers;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace AbleSync.Core.Services
{
    /// <summary>
    ///     Service for managing tracking files.
    /// </summary>
    /// <remarks>
    ///     This only operates locally and has no knowledge of any data store.
    /// </remarks>
    public class TrackingFileService : ITrackingFileService
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
                TrackingFileDateCreated = DateTimeOffset.Now,
                ProjectDateScraped = DateTimeOffset.Now,
                TrackingFileStatus = TrackingFileStatus.UpToDate
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
                throw new MultipleTrackingFilesException();
            }

            var trackingFileName = trackingFileCandidates.First();
            var projectId = trackingFileName.Name.Replace(Constants.TrackingFileExtension, "", StringComparison.InvariantCulture);

            var path = $"{directoryInfo.FullName}/{projectId}{Constants.TrackingFileExtension}";
            return FileHelper.GetFile<TrackingFile>(path);
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
                throw new MultipleTrackingFilesException();
            }

            return true;
        }

        /// <summary>
        ///     Marks a tracking file as <see cref="ProjectStatus.Invalid"/>.
        /// </summary>
        /// <param name="directoryInfo">The directory of the file.</param>
        /// <returns><see cref="Task"/></returns>
        public void MarkTrackingFileInvalidLocal(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            var trackingFile = GetTrackingFile(directoryInfo);

            trackingFile.TrackingFileStatus = TrackingFileStatus.InvalidLocal;

            OverwriteFile(directoryInfo, trackingFile);
        }

        /// <summary>
        ///     Marks a tracking file as scraped at the moment of execution.
        /// </summary>
        /// <param name="directoryInfo">The directory of the file.</param>
        public void MarkProjectScraped(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            var trackingFile = GetTrackingFile(directoryInfo);

            trackingFile.ProjectDateScraped = DateTimeOffset.Now;

            OverwriteFile(directoryInfo, trackingFile);
        }

        /// <summary>
        ///     Marks a tracking file as analyzed at the moment of execution.
        /// </summary>
        /// <param name="directoryInfo">The directory of the file.</param>
        public void MarkProjectAnalyzed(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            var trackingFile = GetTrackingFile(directoryInfo);

            trackingFile.ProjectDateAnalyzed = DateTimeOffset.Now;

            OverwriteFile(directoryInfo, trackingFile);
        }

        /// <summary>
        ///     Deletes and creates a tracking file.
        /// </summary>
        /// <param name="directoryInfo">The file directory.</param>
        /// <param name="trackingFile">The new tracking file.</param>
        private void OverwriteFile(DirectoryInfo directoryInfo, TrackingFile trackingFile)
        {
            DeleteTrackingFile(directoryInfo);
            WriteToFile(directoryInfo, trackingFile);
        }

        // TODO Move to helper
        /// <summary>
        ///     Actually writes a <see cref="TrackingFile"/> to file.
        /// </summary>
        /// <param name="directoryInfo">The directory.</param>
        /// <param name="trackingFile">The tracking file to write.</param>
        private static void WriteToFile(DirectoryInfo directoryInfo, TrackingFile trackingFile)
        {
            var path = $"{directoryInfo.FullName}/{trackingFile.ProjectId}{Constants.TrackingFileExtension}";
            using var stream = new FileStream(path, FileMode.Create);

            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, trackingFile);
        }

    }
}
