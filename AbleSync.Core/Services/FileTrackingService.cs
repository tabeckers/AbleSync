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
                throw new InvalidOperationException("Can't have more than one tracking file");
            }

            return true;
        }

        /// <summary>
        ///     Marks a tracking file as <see cref="ProjectStatus.Invalid"/>.
        /// </summary>
        /// <param name="directoryInfo">The directory of the file.</param>
        /// <returns><see cref="Task"/></returns>
        public TrackingFile MarkTrackingFileInvalid(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            var trackingFile = GetTrackingFile(directoryInfo);
            trackingFile.ProjectStatus = ProjectStatus.Invalid;
            OverwriteFile(directoryInfo, trackingFile);

            return trackingFile;
        }

        /// <summary>
        ///     Updates a <see cref="TrackingFile"/> in a project folder based
        ///     on the current state of the file and its <see cref="Project"/>.
        /// </summary>
        /// <param name="directoryInfo">The respective directory.</param>
        /// <param name="project">The tracking files project.</param>
        /// <returns>The updated <see cref="TrackingFile"/>.</returns>
        public TrackingFile UpdateTrackingFile(DirectoryInfo directoryInfo, Project project)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var trackingFile = GetTrackingFile(directoryInfo);
            trackingFile.ProjectStatus = project.ProjectStatus;
            trackingFile.ProjectDateCreated = project.DateCreated;
            trackingFile.DateUpdated = project.DateUpdated;

            OverwriteFile(directoryInfo, trackingFile);

            return trackingFile;
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
