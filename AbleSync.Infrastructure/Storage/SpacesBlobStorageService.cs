using AbleSync.Core.Exceptions;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RenameMe.Utility.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
namespace AbleSync.Infrastructure.Storage
{
    /// <summary>
    ///     Digital Ocean Spaces implementation of <see cref="IBlobStorageService"/>.
    /// </summary>
    /// <remarks>
    ///     This creates an <see cref="IAmazonS3"/> client once in its constructor.
    ///     Register this service as a singleton if dependency injection is used.
    /// </remarks>
    internal class SpacesBlobStorageService : IBlobStorageService, IDisposable
    {
        private readonly BlobStorageOptions _options;
        private readonly IAmazonS3 client;
        private readonly ILogger<SpacesBlobStorageService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public SpacesBlobStorageService(IOptions<BlobStorageOptions> options,
            ILogger<SpacesBlobStorageService> logger)
        {
            if (options == null || options.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.Value.AccessKey))
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.Value.SecretKey))
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Value.ServiceUri == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create client once.
            var config = new AmazonS3Config
            {
                ServiceURL = _options.ServiceUri.AbsoluteUri
            };
            var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
            client = new AmazonS3Client(credentials, config);
        }

        /// <summary>
        ///     Called on graceful shutdown.
        /// </summary>
        public void Dispose() => client.Dispose();

        // TODO Amazon has no clean way to check for object existence.
        /// <summary>
        ///     Checks if a file exists or not.
        /// </summary>
        /// <param name="directoryName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>Boolean result.</returns>
        public async Task<bool> FileExistsAsync(string directoryName, string fileName, CancellationToken token)
        { 
            fileName.ThrowIfNullOrEmpty();

            try
            {
                // TODO Maybe use list keys with a filter?

                var result = await client.GetObjectAsync(new Amazon.S3.Model.GetObjectRequest
                {
                    BucketName = _options.BlobStorageName,
                    Key = string.IsNullOrEmpty(directoryName) ? fileName : $"{WithTrailingSlash(directoryName)}{fileName}"
                }, token);

                return true;
            }
            catch (Exception e)
            {
                // This type of exception indicates that the file does not exist.
                if (e is AmazonS3Exception exception && exception.ErrorCode == "NoSuchKey")
                {
                    return false;
                }

                _logger.LogError(e, "Could not check file existence in Spaces using S3");
                // TODO QUESTION: Inner exception or not? I don't think so because we already log it.
                throw new StorageException("Could not check file existence", e);
            }
        }

        /// <summary>
        ///     Gets an access uri for a given file.
        /// </summary>
        /// <remarks>
        ///     The default <paramref name="accessType"/> is read.
        /// </remarks>
        /// <param name="directoryName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="hoursValid">How many hours the link should be valid.</param>
        /// <param name="accessType">Indicates what we want to do with the link.</param>
        /// <returns>Access <see cref="Uri"/>.</returns>
        public Task<Uri> GetAccessUriAsync(string directoryName, string fileName, double hoursValid, FileAccessType fileAccessType, CancellationToken token)
            => GetAccessUriOverrideFilenameAsync(directoryName, fileName, fileName, hoursValid, fileAccessType, token);

        /// <summary>
        ///     Gets an access uri to upload or access a file. This will then
        ///     change the name of the downloaded file to <paramref name="overrideFileName"/>.
        /// </summary>
        /// <remarks>
        ///     This can be used to add the file extension.
        /// </remarks>
        /// <param name="directoryName">The directory in which the file should exist.</param>
        /// <param name="fileName">The name of the file to access.</param>
        /// <param name="overrideFileName">The overrided file name.</param>
        /// <param name="hoursValid">How long the uri should be valid.</param>
        /// <param name="fileAccessType">What we want to do with the file.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        public Task<Uri> GetAccessUriOverrideFilenameAsync(string directoryName, string fileName, string overrideFileName, double hoursValid, FileAccessType fileAccessType, CancellationToken token)
        {
            directoryName.ThrowIfNullOrEmpty();
            fileName.ThrowIfNullOrEmpty();
            overrideFileName.ThrowIfNullOrEmpty();
            if (hoursValid <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hoursValid));
            }

            // TODO Doesn't use token anywhere.

            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _options.BlobStorageName,
                    Key = string.IsNullOrEmpty(directoryName) ? fileName : $"{WithTrailingSlash(directoryName)}{fileName}",
                    Expires = DateTime.UtcNow.AddHours(hoursValid),
                    Verb = fileAccessType switch
                    {
                        FileAccessType.Read => HttpVerb.GET,
                        FileAccessType.Write => HttpVerb.PUT,
                        _ => throw new InvalidOperationException(nameof(fileAccessType))
                    },

                };

                // Override the download file name.
                request.ResponseHeaderOverrides.ContentDisposition = $"attachment; filename={overrideFileName}";

                var uri = new Uri(client.GetPreSignedURL(request));

                return Task.FromResult(uri);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, "Could not get access link from Spaces using S3");
                throw new StorageException("Could not get access link", e);
            }
        }

        /// <summary>
        ///     Stores a file in a Digital Ocean Space.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="stream">See <see cref="Stream"/>.</param>
        /// <returns>See <see cref="ValueTask"/>.</returns>
        public Task StoreFileAsync(string containerName, string fileName, string contentType, Stream stream, CancellationToken token)
        {
            fileName.ThrowIfNullOrEmpty();
            contentType.ThrowIfNullOrEmpty();
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            try
            {
                using var transferUtility = new TransferUtility(client);
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _options.BlobStorageName,
                    ContentType = contentType,
                    Key = string.IsNullOrEmpty(containerName) ? fileName : $"{WithTrailingSlash(containerName)}{fileName}",
                    InputStream = stream
                };

                return transferUtility.UploadAsync(request, token);
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError(e, $"Could not store file with content type {contentType} to Spaces using S3");
                throw new StorageException($"Could not upload file with content type {contentType}", e);
            }
        }

        /// <summary>
        ///     Removes a / or \ from the end of a string if present.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <returns>The string without trailing slash.</returns>
        private static string WithoutTrailingSlash(string input)
        {
            if (input.EndsWith("\\", StringComparison.InvariantCulture))
            {
                return input[0..^1];
            }

            if (input.EndsWith("/", StringComparison.InvariantCulture))
            {
                return input[0..^1];
            }

            return input;
        }

        /// <summary>
        ///     Ensures a / at the end of a string.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <returns>The string with trailing slash.</returns>
        private static string WithTrailingSlash(string input)
            => $"{WithoutTrailingSlash(input)}/";
    }
}
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
