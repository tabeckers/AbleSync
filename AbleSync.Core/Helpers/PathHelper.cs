using System;

namespace AbleSync.Core.Helpers
{
    /// <summary>
    ///     Contains helper functionality for dealing with paths.
    /// </summary>
    public static class PathHelper
    {
        // TODO This was copied from internet and could use some work.
        /// <summary>
        ///     Returns a relative path string from a full path based on a base path
        ///     provided.
        /// </summary>
        /// <param name="fullPath">The path to convert. Can be either a file or a directory</param>
        /// <param name="basePath">The base path on which relative processing is based. Should be a directory.</param>
        /// <returns>
        ///     String of the relative path.
        /// 
        ///     Examples of returned values:
        ///     test.txt, ..\test.txt, ..\..\..\test.txt, ., .., subdir\test.txt
        /// </returns>
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

            // Require trailing backslash for path
            if (!basePath.EndsWith("\\", StringComparison.InvariantCulture))
            {
                basePath += "\\";
            }

            var baseUri = new Uri(basePath);
            var fullUri = new Uri(fullPath);

            var relativeUri = baseUri.MakeRelativeUri(fullUri);

            // Uri's use forward slashes so convert back to backward slashes
            return relativeUri.ToString().Replace("/", "\\", StringComparison.InvariantCulture);
        }
    }
}
