using System;
using System.Runtime.Serialization;

namespace Furcadia.Net
{
    /// <summary>
    /// Desctiption of NetProxyException.
    /// </summary>
    [Serializable]
    public class NetProxyException : Exception, ISerializable
    {
        #region Public Constructors

        /// <summary>
        /// </summary>
        public NetProxyException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public NetProxyException(string message) : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="innerException">
        /// </param>
        public NetProxyException(string message, Exception innerException) : base(message, innerException)
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
        protected NetProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}