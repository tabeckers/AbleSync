using AbleSync.Core.Exceptions;
using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates a full queue.
    /// </summary>
    public sealed class QueueFullException : AbleSyncBaseException
    {
        public QueueFullException()
        {
        }

        public QueueFullException(string message) : base(message)
        {
        }

        public QueueFullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public QueueFullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
