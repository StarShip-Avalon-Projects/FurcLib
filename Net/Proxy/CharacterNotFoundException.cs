using System;
using System.Runtime.Serialization;

namespace Furcadia.Net.Proxy
{
    /// <summary>
    /// Furcadia Character Not found Exception
    /// </summary>
    internal class CharacterNotFoundException : Exception, ISerializable
    {
        #region Public Constructors

        public CharacterNotFoundException()
        {
        }

        public CharacterNotFoundException(string message) : base(message)
        {
        }

        public CharacterNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        // This constructor is needed for serialization.
        protected CharacterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}