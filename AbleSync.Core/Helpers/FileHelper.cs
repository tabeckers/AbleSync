using AbleSync.Core.Exceptions;
using RenameMe.Utility.Extensions;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AbleSync.Core.Helpers
{
    // TODO Windows specific.
    /// <summary>
    ///     Contains helper functionality for files.
    /// </summary>
    public static class FileHelper
    {
        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        ///     Gets a file from a path.
        /// </summary>
        /// <typeparam name="TFile">The file type to deserialize to.</typeparam>
        /// <param name="path">The path to the file.</param>
        /// <returns>The deserialized file.</returns>
        public static TFile GetFile<TFile>(string path)
        {
            path.ThrowIfNullOrEmpty();

            try
            {
                using var stream = new FileStream(path, FileMode.Open);
                return (TFile)formatter.Deserialize(stream);
            }
            catch (Exception e)
            {
                throw new FileAccessException($"Could not access file at {path}", e);
            }
        }
    }
}
