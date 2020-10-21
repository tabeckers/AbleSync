using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates an invalid tracking file.
    /// </summary>
    public sealed class TrackingFileInvalidException : AbleSyncBaseException
    {
        public TrackingFileInvalidException()
        {
        }

        public TrackingFileInvalidException(string message) : base(message)
        {
        }

        public TrackingFileInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TrackingFileInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
