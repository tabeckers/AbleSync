using AbleSync.Core.Types;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for handling blob file storage.
    /// </summary>
    public interface IBlobStorageService
    {
        /// <summary>
        ///     Checks if a file exists in our blob storage.
        /// </summary>
        /// <param name="directoryName">The directory in which the file should exist.</param>
        /// <param name="fileName">The name of the file itself.</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>True if the file exists.</returns>
        Task<bool> FileExistsAsync(string directoryName, string fileName, CancellationToken token);

        /// <summary>
        ///     Gets an access uri to upload or access a file.
        /// </summary>
        /// <param name="directoryName">The directory in which the file should exist.</param>
        /// <param name="fileName">The name of the file to access.</param>
        /// <param name="hoursValid">How long the uri should be valid.</param>
        /// <param name="fileAccessType">What we want to do with the file.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<Uri> GetAccessUriAsync(string directoryName, string fileName, double hoursValid, FileAccessType fileAccessType, CancellationToken token);

        /// <summary>
        ///     Gets an access uri to upload or access a file. This will then
        ///     change the name of the downloaded file to <paramref name="overrideFileName"/>.
        /// </summary>
        /// <param name="directoryName">The directory in which the file should exist.</param>
        /// <param name="fileName">The name of the file to access.</param>
        /// <param name="overrideFileName">The overrided file name.</param>
        /// <param name="hoursValid">How long the uri should be valid.</param>
        /// <param name="fileAccessType">What we want to do with the file.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<Uri> GetAccessUriOverrideFilenameAsync(string directoryName, string fileName, string overrideFileName, double hoursValid, FileAccessType fileAccessType, CancellationToken token);

        /// <summary>
        ///     Stores a file to the blob storage from a stream.
        /// </summary>
        /// <param name="directoryName">The directory in which the file should be stored.</param>
        /// <param name="fileName">The name of the file itself.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <param name="stream">The stream to the file.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task StoreFileAsync(string directoryName, string fileName, string contentType, Stream stream, CancellationToken token);
    }
}
