using AbleSync.Core.Helpers;
using System;
using Xunit;

namespace AbleSync.Testing.UnitTests.Helpers
{
    /// <summary>
    ///     Contains unit tests for <see cref="PathHelper"/>.
    /// </summary>
    public class PathHelperTests
    {
        private const string ValidPath = "C:/valid/path";

        [Fact]
        public void GetRelativePathCorrectly()
        {
            // Arrange.
            var fullPath = "C:/root/folders/mypath";
            var basePath = "C:/root/";

            // Act.
            var relativePath = PathHelper.GetRelativePath(fullPath, basePath);

            // Assert.
            Assert.Equal("folders/mypath", relativePath);
        }

        [Theory]
        [InlineData("c:/root/folders/mypath", "c:/root")]
        [InlineData("c:/root/folders/mypath/", "c:/root")]
        [InlineData("c:/root/folders/mypath", "c:/root/")]
        [InlineData("c:/root/folders/mypath/", "c:/root/")]
        [InlineData("c:\\root\\folders\\mypath", "c:\\root")]
        [InlineData("c:\\root\\folders\\mypath\\", "c:\\root")]
        [InlineData("c:\\root\\folders\\mypath", "c:\\root\\")]
        [InlineData("c:\\root\\folders\\mypath\\", "c:\\root\\")]
        public void GetRelativePathDifferentValidFormats(string fullPath, string basePath)
        {
            // Act.
            var relativePath = PathHelper.GetRelativePath(fullPath, basePath);

            // Assert.
            Assert.Equal("folders/mypath", relativePath);
        }

        [Theory]
        [InlineData("", ValidPath)]
        [InlineData(null, ValidPath)]
        [InlineData(ValidPath, "")]
        [InlineData(ValidPath, null)]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(null, "")]
        public void GetRelativePathInvalidStringsThrows(string fullPath, string basePath) =>
            // Assert.
            Assert.Throws<ArgumentNullException>(() => PathHelper.GetRelativePath(fullPath, basePath));

        [Theory]
        [InlineData(ValidPath, "no/rootdir")]
        [InlineData(ValidPath, "n@#$%")]
        [InlineData(ValidPath, "singleword")]
        [InlineData(ValidPath, "double//slash")]
        [InlineData(ValidPath, "!/!")]
        [InlineData(ValidPath, ";")]
        public void GetRelativePathMalformattedInputThrows(string fullPath, string basePath) =>
            // Assert
            Assert.Throws<UriFormatException>(() => PathHelper.GetRelativePath(fullPath, basePath));
    }
}
