﻿using System;
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

        public NetProxyException()
        {
        }

        public NetProxyException(string message) : base(message)
        {
        }

        public NetProxyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        // This constructor is needed for serialization.
        protected NetProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}