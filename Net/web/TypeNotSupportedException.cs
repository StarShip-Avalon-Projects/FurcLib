using System;
using System.Runtime.Serialization;

namespace Furcadia.Net.Web
{
    [Serializable]
    internal class TypeNotSupportedException : Exception
    {
        public TypeNotSupportedException()
        {
        }

        public TypeNotSupportedException(string message) : base(message)
        {
        }

        public TypeNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TypeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}