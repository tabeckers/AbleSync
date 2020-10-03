using AbleSync.Core.Types;
using System;

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
        /// <param name="relativePath">Project path relative to the base Uri.</param>
        /// <returns>The created <see cref="TrackingFile"/>.</returns>
        TrackingFile CreateTrackingFile(Uri relativePath);

        /// <summary>
        ///     Updates a <see cref="TrackingFile"/> in a project folder.
        /// </summary>
        /// <param name="relativePath">Project path relative to the base Uri.</param>
        /// <param name="trackingFile"><see cref="TrackingFile"/> containing all updates.</param>
        /// <returns>The updated <see cref="TrackingFile"/>.</returns>
        TrackingFile UpdateTrackingFile(Uri relativePath, TrackingFile trackingFile);

        /// <summary>
        ///     Deletes a <see cref="TrackingFile"/> in a project folder.
        /// </summary>
        /// <param name="relativePath">Project path relative to the base Uri.</param>
        void DeleteTrackingFile(Uri relativePath);
    }
}
