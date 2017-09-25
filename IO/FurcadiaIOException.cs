/*
 * Created by SharpDevelop.
 * User: Gerolkae
 * Date: 5/7/2016
 * Time: 9:16 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Runtime.Serialization;

namespace Furcadia.IO
{
    /// <summary>
    /// Desctiption of FurcadiaIOException.
    /// </summary>
    public class FurcadiaIOException : Exception, ISerializable
    {
        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException()'

        public FurcadiaIOException()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException()'
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException(string)'

        public FurcadiaIOException(string message) : base(message)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException(string)'
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException(string, Exception)'

        public FurcadiaIOException(string message, Exception innerException) : base(message, innerException)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException(string, Exception)'
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        // This constructor is needed for serialization.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException(SerializationInfo, StreamingContext)'

        protected FurcadiaIOException(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaIOException.FurcadiaIOException(SerializationInfo, StreamingContext)'
        {
        }

        #endregion Protected Constructors
    }
}