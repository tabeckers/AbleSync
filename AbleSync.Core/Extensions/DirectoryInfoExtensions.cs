using System;
using System.IO;

namespace AbleSync.Core.Extensions
{
    /// <summary>
    ///     Extension functionality for <see cref="DirectoryInfo"/>.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        ///     Prints a directory info path in the desired format.
        /// </summary>
        /// <param name="self">The directory info.</param>
        /// <returns>The formatted path string.</returns>
        public static string FullPathFormatted(this DirectoryInfo self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            return self.FullName.Replace("\\", "/", StringComparison.InvariantCulture);
        }
    }
}
