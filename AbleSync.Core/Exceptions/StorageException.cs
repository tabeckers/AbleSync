using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates an error with regards to storage.
    /// </summary>
    public sealed class StorageException : AbleSyncBaseException
    {
        public StorageException()
        {
        }

        public StorageException(string message) : base(message)
        {
        }

        public StorageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StorageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
