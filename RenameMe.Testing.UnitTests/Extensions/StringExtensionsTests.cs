using RenameMe.Utility.Extensions;
using System;
using Xunit;

namespace RenameMe.Testing.UnitTests.Extensions
{
    /// <summary>
    ///     Testcases for string extensions.
    /// </summary>
    public class StringExtensionsTests
    {
        /// <summary>
        ///     Tests a nonemtpy string.
        /// </summary>
        [Fact]
        public void ValidStringDoesntThrow()
        {
            // Arrange.
            var s = "thisismystring";

            // Act.
            s.ThrowIfNullOrEmpty();
        }

        /// <summary>
        ///     Tests a whitespace character string.
        /// </summary>
        [Fact]
        public void WhitespaceStringDoesntThrow()
        {
            // Arrange.
            var s = " ";

            // Act.
            s.ThrowIfNullOrEmpty();
        }

        /// <summary>
        ///     Tests if an empty string generates an exception.
        /// </summary>
        [Fact]
        public void EmptyStringThrows()
        {
            // Arrange.
            var s = "";

            // Assert.
            Assert.Throws<ArgumentNullException>(() => s.ThrowIfNullOrEmpty());
        }

        /// <summary>
        ///     Tests if a null string generates an exception.
        /// </summary>
        [Fact]
        public void NullStringThrows()
        {
            // Arrange.
            string s = null;

            // Assert.
            Assert.Throws<ArgumentNullException>(() => s.ThrowIfNullOrEmpty());
        }
    }
}
