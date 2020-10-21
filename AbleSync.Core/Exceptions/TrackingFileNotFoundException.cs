using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates a trackign file was not found.
    /// </summary>
    public sealed class TrackingFileNotFoundException : AbleSyncBaseException
    {
        public TrackingFileNotFoundException()
        {
        }

        public TrackingFileNotFoundException(string message) : base(message)
        {
        }

        public TrackingFileNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TrackingFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
