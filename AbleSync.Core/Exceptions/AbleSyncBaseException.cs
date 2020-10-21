using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Base exception for library specific exceptions.
    /// </summary>
    public abstract class AbleSyncBaseException : Exception
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AbleSyncBaseException()
        {
        }

        /// <summary>
        ///     Create new instance.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public AbleSyncBaseException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Create new instance.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="innerException">The inner <see cref="Exception"/>.</param>
        public AbleSyncBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        // TODO What does this actually do?
        /// <summary>
        ///     Create new instance.
        /// </summary>
        /// <param name="info">See <see cref="SerializationInfo"/>.</param>
        /// <param name="context">See <see cref="StreamingContext"/>.</param>
        protected AbleSyncBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
