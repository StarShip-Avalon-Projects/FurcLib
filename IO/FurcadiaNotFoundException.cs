/*
 * Created by SharpDevelop.
 * User: Gerolkae
 * Date: 5/7/2016
 * Time: 10:03 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Runtime.Serialization;

namespace Furcadia.IO
{
    /// <summary>
    /// Desctiption of FurcadiaNotFoundException.
    /// </summary>
    public class FurcadiaNotFoundException : Exception, ISerializable
    {
        #region Public Constructors

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException()'

        public FurcadiaNotFoundException()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException()'
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException(string)'

        public FurcadiaNotFoundException(string message) : base(message)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException(string)'
        {
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException(string, Exception)'

        public FurcadiaNotFoundException(string message, Exception innerException) : base(message, innerException)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException(string, Exception)'
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        // This constructor is needed for serialization.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException(SerializationInfo, StreamingContext)'

        protected FurcadiaNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'FurcadiaNotFoundException.FurcadiaNotFoundException(SerializationInfo, StreamingContext)'
        {
        }

        #endregion Protected Constructors
    }
}