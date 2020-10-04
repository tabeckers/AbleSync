using System;
using System.Runtime.Serialization;

namespace AbleSync.Core.Exceptions
{
    /// <summary>
    ///     Indicates our configuration is incorrect.
    /// </summary>
    public sealed class ConfigurationException : AbleSyncBaseException
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
