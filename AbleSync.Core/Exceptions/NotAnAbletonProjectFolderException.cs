using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates we try to process a directory which is not an Ableton
    ///     project directory as if it were one.
    /// </summary>
    public sealed class NotAnAbletonProjectFolderException : AbleSyncBaseException
    {
        public NotAnAbletonProjectFolderException()
        {
        }

        public NotAnAbletonProjectFolderException(string message) : base(message)
        {
        }

        public NotAnAbletonProjectFolderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotAnAbletonProjectFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
