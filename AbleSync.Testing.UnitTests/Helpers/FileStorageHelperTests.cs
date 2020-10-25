using AbleSync.Core;
using AbleSync.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AbleSync.Testing.UnitTests.Helpers
{
    /// <summary>
    ///     Test cases for <see cref="FileStorageHelper"/>.
    /// </summary>
    public class FileStorageHelperTests
    {
        /// <summary>
        ///     Checks if we correctly generate our audio file name.
        /// </summary>
        [Fact]
        public void GenerateAudioFileName()
        {
            // Arrange.
            var audioFileId = Guid.NewGuid();

            // Act.
            var result = FileStorageHelper.AudioFileName(audioFileId);

            // Assert.
            Assert.Equal($"{audioFileId}", result);
        }

        /// <summary>
        ///     Checks if we correctly generate our audio file folder
        ///     according to the set constant values.
        /// </summary>
        [Fact]
        public void GenerateAudioFileFolder()
        {
            // Arrange.
            var projectId = Guid.NewGuid();

            // Act.
            var result = FileStorageHelper.AudioFileFolder(projectId);

            // Assert.
            Assert.Equal($"{Constants.StorageProjectFolderBase}/{projectId}/{Constants.StorageAudioFilesFolder}", result);
        }

        /// <summary>
        ///     Checks if an invalid id causes the generation to throw.
        /// </summary>
        [Fact]
        public void GenerateAudioFileNameInvalidAudioFileIdThrows()
        {
            // Arrange
            var audioFileId = Guid.Empty;

            // Assert
            Assert.Throws<ArgumentNullException>(() => FileStorageHelper.AudioFileName(audioFileId));
        }

        /// <summary>
        ///     Checks if an invalid id causes the generation to throw.
        /// </summary>
        [Fact]
        public void GenerateAudioFileFolderInvalidProjectIdThrows()
        {
            // Arrange
            var projectId = Guid.Empty;

            // Assert
            Assert.Throws<ArgumentNullException>(() => FileStorageHelper.AudioFileFolder(projectId));
        }
    }
}
