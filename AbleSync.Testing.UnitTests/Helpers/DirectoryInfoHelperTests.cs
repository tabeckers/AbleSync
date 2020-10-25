using AbleSync.Core.Entities;
using AbleSync.Core.Helpers;
using System;
using Xunit;

namespace AbleSync.Testing.UnitTests.Helpers
{
    /// <summary>
    ///     Contains tests cases for <see cref="DirectoryInfoHelper"/>.
    /// </summary>
    public class DirectoryInfoHelperTests
    {
        // TODO This is coupled with windows.
        /// <summary>
        ///     Checks for valid path escaping.
        /// </summary>
        [Fact]
        public void ParseProjectPath()
        {
            // Arrange.
            var rootDirectory = new Uri("C:/root/my folder/my-subfolder");
            var project = new Project
            {
                RelativePath = "projects/myproject"
            };

            // Act.
            var result = DirectoryInfoHelper.ParsePathFromProject(rootDirectory, project);

            // Assert.
            Assert.Equal("C:/root/my folder/my-subfolder/projects/myproject", result);
        }
    }
}
