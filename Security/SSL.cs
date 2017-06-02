using System;

using System.Net.Security;

namespace Furcadia.Security
{
    public class SSL
    {
        #region Public Delegates

        public delegate void SecureConnectionResultsCallback(object sender, SecureConnectionResults args);

        #endregion Public Delegates

        #region Public Classes

        public class SecureConnectionResults

        {
            #region Private Fields

            private Exception asyncException;
            private SslStream secureStream;

            #endregion Private Fields

            #region Internal Constructors

            internal SecureConnectionResults(SslStream sslStream)

            {
                this.secureStream = sslStream;
            }

            internal SecureConnectionResults(Exception exception)

            {
                this.asyncException = exception;
            }

            #endregion Internal Constructors

            #region Public Properties

            public Exception AsyncException { get { return asyncException; } }

            public SslStream SecureStream { get { return secureStream; } }

            #endregion Public Properties
        }

        #endregion Public Classes
    }
}