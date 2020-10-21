using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates something went wrong with regards to file access.
    /// </summary>
    public sealed class FileAccessException : AbleSyncBaseException
    {
        public FileAccessException()
        {
        }

        public FileAccessException(string message) : base(message)
        {
        }

        public FileAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FileAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
