using System;
using System.Runtime.Serialization;

namespace DevTools.Core.Storage.File
{
    [Serializable]
    internal class ContainerAllreadyExistsException : Exception
    {
        public ContainerAllreadyExistsException()
        {
        }

        public ContainerAllreadyExistsException(string message) : base(message)
        {
        }

        public ContainerAllreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContainerAllreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}