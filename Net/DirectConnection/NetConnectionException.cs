using System;
using System.Runtime.Serialization;

namespace Furcadia.Net.DirectConnection
{
    /// <summary>
    /// Desctiption of NetConnectionException.
    /// </summary>
    [Serializable]
    public class NetConnectionException : Exception, ISerializable
    {
        #region Public Constructors

        /// <summary>
        /// </summary>
        public NetConnectionException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public NetConnectionException(string message) : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="innerException">
        /// </param>
        public NetConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        // This constructor is needed for serialization.
        /// <summary>
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
        protected NetConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}