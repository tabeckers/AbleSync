using RenameMe.Utility.Extensions;
using System;
using Xunit;

namespace RenameMe.Testing.UnitTests.Extensions
{
    /// <summary>
    ///     Testcases for guid extensions.
    /// </summary>
    public class GuidExtensionsTests
    {
        /// <summary>
        ///     Tests a valid guid.
        /// </summary>
        [Fact]
        public void GuidValidDoesntThrow()
        {
            // Arrange.
            var guid = Guid.NewGuid();

            // Act.
            guid.ThrowIfNullOrEmpty();
        }

        /// <summary>
        ///     Tests the exception for an empty guid.
        /// </summary>
        /// <remarks>
        ///     Guids are not nullable, thus the function
        ///     can't be tested for null values.
        /// </remarks>
        [Fact]
        public void GuidEmptyThrows()
        {
            // Arrange.
            var guid = Guid.Empty;

            // Assert.
            Assert.Throws<ArgumentNullException>(() => guid.ThrowIfNullOrEmpty());
        }
    }
}
