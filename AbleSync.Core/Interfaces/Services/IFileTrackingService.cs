using AbleSync.Core.Entities;
using AbleSync.Core.Types;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a file tracking service. This performs tracking file 
    ///     creation, updating and deleting on single project folders.
    /// </summary>
    public interface IFileTrackingService
    {
        /// <summary>
        ///     Creates a new <see cref="TrackingFile"/> in a project folder.
        /// </summary>
        /// <param name="projectId">The internal project id.</param>
        /// <param name="directoryInfo">The respective directory.</param>
        /// <returns>The created <see cref="TrackingFile"/>.</returns>
        TrackingFile CreateTrackingFile(Guid projectId, DirectoryInfo directoryInfo);

        /// <summary>
        ///     Deletes a <see cref="TrackingFile"/> in a project folder.
        /// </summary>
        /// <param name="directoryInfo">The respective directory.</param>
        void DeleteTrackingFile(DirectoryInfo directoryInfo);

        /// <summary>
        ///     Gets a tracking file from a directory.
        /// </summary>
        /// <param name="directoryInfo">The directory to check.</param>
        /// <returns>The fetched tracking file.</returns>
        TrackingFile GetTrackingFile(DirectoryInfo directoryInfo);

        /// <summary>
        ///     Checks if a directory has a tracking file.
        /// </summary>
        /// <param name="directoryInfo">The directory to check.</param>
        /// <returns><c>true</c> if a tracking file exists.</returns>
        bool HasTrackingFile(DirectoryInfo directoryInfo);

        /// <summary>
        ///     Marks a tracking file as <see cref="ProjectStatus.Invalid"/>.
        /// </summary>
        /// <param name="directoryInfo">The directory of the file.</param>
        /// <returns><see cref="Task"/></returns>
        TrackingFile MarkTrackingFileInvalid(DirectoryInfo directoryInfo);

        /// <summary>
        ///     Updates a <see cref="TrackingFile"/> in a project folder based
        ///     on the current state of the file and its <see cref="Project"/>.
        /// </summary>
        /// <param name="directoryInfo">The respective directory.</param>
        /// <param name="project">The tracking files project.</param>
        /// <returns>The updated <see cref="TrackingFile"/>.</returns>
        TrackingFile UpdateTrackingFile(DirectoryInfo directoryInfo, Project project);
    }
}
