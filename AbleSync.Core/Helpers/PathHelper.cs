using System;

namespace AbleSync.Core.Helpers
{
    /// <summary>
    ///     Contains helper functionality for dealing with paths.
    /// </summary>
    public static class PathHelper
    {
        // FUTURE This can throw a uri format exception, might not be optimal.
        /// <summary>
        ///     Returns a relative path string from a full path based on a base path
        ///     provided. This returns no trailing slash.
        /// </summary>
        /// <param name="fullPath">The directory path to convert.</param>
        /// <param name="basePath">The base directory path.</param>
        /// <returns>String of the relative path.</returns>
        public static string GetRelativePath(string fullPath, string basePath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                throw new ArgumentNullException(nameof(fullPath));
            }
            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            // Remove all trailing slashes
            while (fullPath.EndsWith("\\", StringComparison.InvariantCulture)
                || fullPath.EndsWith("/", StringComparison.InvariantCulture))
            {
                fullPath = fullPath[0..^1];
            }
            while (basePath.EndsWith("\\", StringComparison.InvariantCulture)
                || basePath.EndsWith("/", StringComparison.InvariantCulture))
            {
                basePath = basePath[0..^1];
            }

            // Add trailing slashes
            basePath += "/";

            var baseUri = new Uri(basePath);
            var fullUri = new Uri(fullPath);

            var relativeUri = baseUri.MakeRelativeUri(fullUri);

            return relativeUri.ToString();
        }
    }
}
