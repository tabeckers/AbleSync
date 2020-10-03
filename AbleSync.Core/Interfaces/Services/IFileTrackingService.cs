using AbleSync.Core.Types;
using System;
using System.IO;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a file tracking service. This performs tracking file 
    ///     creation, updating and deleting on single project folders.
    /// </summary>
    public interface IFileTrackingService
    {
        bool HasTrackingFile(DirectoryInfo directoryInfo);

        TrackingFile GetTrackingFile(DirectoryInfo directoryInfo);

        /// <summary>
        ///     Creates a new <see cref="TrackingFile"/> in a project folder.
        /// </summary>
        /// <param name="projectId">The internal project id.</param>
        /// <param name="directoryInfo">The respective directory.</param>
        /// <returns>The created <see cref="TrackingFile"/>.</returns>
        TrackingFile CreateTrackingFile(Guid projectId, DirectoryInfo directoryInfo);

        /// <summary>
        ///     Updates a <see cref="TrackingFile"/> in a project folder.
        /// </summary>
        /// <param name="directoryInfo">The respective directory.</param>
        /// <param name="trackingFile"><see cref="TrackingFile"/> containing all updates.</param>
        /// <returns>The updated <see cref="TrackingFile"/>.</returns>
        TrackingFile UpdateTrackingFile(DirectoryInfo directoryInfo, TrackingFile trackingFile);

        /// <summary>
        ///     Deletes a <see cref="TrackingFile"/> in a project folder.
        /// </summary>
        /// <param name="directoryInfo">The respective directory.</param>
        void DeleteTrackingFile(DirectoryInfo directoryInfo);
    }
}
