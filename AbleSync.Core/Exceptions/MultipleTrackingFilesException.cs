using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates we found multiple tracking files in a folder.
    /// </summary>
    public sealed class MultipleTrackingFilesException : AbleSyncBaseException
    {
        public MultipleTrackingFilesException()
        {
        }

        public MultipleTrackingFilesException(string message) : base(message)
        {
        }

        public MultipleTrackingFilesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MultipleTrackingFilesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
