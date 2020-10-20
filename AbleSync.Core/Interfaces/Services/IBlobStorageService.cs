using AbleSync.Core.Types;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Interfaces.Services
{
    // TODO Docs.
    /// <summary>
    ///     Contract for handling blob file storage.
    /// </summary>
    public interface IBlobStorageService
    {
        Task<bool> FileExistsAsync(string directoryName, string fileName, CancellationToken token);

        Task<Uri> GetAccessUriAsync(string directoryName, string fileName, double hoursValid, FileAccessType fileAccessType, CancellationToken token);

        Task StoreFileAsync(string directoryName, string fileName, string contentType, Stream stream, CancellationToken token);
    }
}
