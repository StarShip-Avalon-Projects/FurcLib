using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using static Furcadia.Security.SSL;

namespace Furcadia.Security
{
    public class SecureTcpClient : IDisposable

    {
        #region Private Fields

        private RemoteCertificateValidationCallback certValidationCallback;
        private bool checkCertificateRevocation;
        private TcpClient client;
        private X509CertificateCollection clientCertificates;
        private SecureConnectionResultsCallback connectionCallback;
        private int disposed;
        private AsyncCallback onAuthenticateAsClient;
        private AsyncCallback onConnected;
        private SslProtocols protocols;
        private IPEndPoint remoteEndPoint;

        private string remoteHostName;

        #endregion Private Fields

        #region Public Constructors

        public SecureTcpClient(SecureConnectionResultsCallback callback)

            : this(callback, null, SslProtocols.Default)

        {
        }

        public SecureTcpClient(SecureConnectionResultsCallback callback,

            RemoteCertificateValidationCallback certValidationCallback)

            : this(callback, certValidationCallback, SslProtocols.Default)

        {
        }

        public SecureTcpClient(SecureConnectionResultsCallback callback,

            RemoteCertificateValidationCallback certValidationCallback,

            SslProtocols sslProtocols)

        {
            onConnected = new AsyncCallback(OnConnected);

            onAuthenticateAsClient = new AsyncCallback(OnAuthenticateAsClient);

            this.certValidationCallback = certValidationCallback;

            this.connectionCallback = callback ?? throw new ArgumentNullException("callback");

            protocols = sslProtocols;

            this.disposed = 0;
        }

        #endregion Public Constructors

        #region Private Destructors

        ~SecureTcpClient()

        {
            Dispose();
        }

        #endregion Private Destructors

        #region Public Properties

        public bool CheckCertificateRevocation

        {
            get { return checkCertificateRevocation; }

            set { checkCertificateRevocation = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public void Close()

        {
            Dispose();
        }

        public void Dispose()

        {
            if (System.Threading.Interlocked.Increment(ref disposed) == 1)

            {
                if (client != null)

                {
                    client.Close();

                    client = null;
                }

                GC.SuppressFinalize(this);
            }
        }

        public void StartConnecting(string remoteHostName, IPEndPoint remoteEndPoint)

        {
            StartConnecting(remoteHostName, remoteEndPoint, null);
        }

        public void StartConnecting(string remoteHostName, IPEndPoint remoteEndPoint,

            X509CertificateCollection clientCertificates)

        {
            if (string.IsNullOrEmpty(remoteHostName))

                throw new ArgumentException("Value cannot be null or empty", "remoteHostName");

            if (remoteEndPoint == null)

                throw new ArgumentNullException("remoteEndPoint");

            //Console.WriteLine(string.Format("Client connecting to: { 0}", remoteEndPoint.Address));

            // this.clientCertificates = clientCertificates;

            this.remoteHostName = remoteHostName;

            this.remoteEndPoint = remoteEndPoint;

            if (client != null)

                client.Close();

            client = new TcpClient(remoteEndPoint.AddressFamily);

            client.BeginConnect(remoteEndPoint.Address,

                remoteEndPoint.Port,

                this.onConnected, null);
        }

        #endregion Public Methods

        #region Private Methods

        private void OnAuthenticateAsClient(IAsyncResult result)

        {
            SslStream sslStream = null;

            try

            {
                sslStream = result.AsyncState as SslStream;

                sslStream.EndAuthenticateAsClient(result);

                this.connectionCallback(this, new SecureConnectionResults(sslStream));
            }
            catch (Exception ex)

            {
                if (sslStream != null)

                {
                    sslStream.Dispose();

                    sslStream = null;
                }

                this.connectionCallback(this, new SecureConnectionResults(ex));
            }
        }

        private void OnConnected(IAsyncResult result)

        {
            SslStream sslStream = null;

            try

            {
                bool leaveStreamOpen = false;//close the socket when done

                if (this.certValidationCallback != null)

                    sslStream = new SslStream(client.GetStream(), leaveStreamOpen, this.certValidationCallback);
                else

                    sslStream = new SslStream(client.GetStream(), leaveStreamOpen);

                sslStream.BeginAuthenticateAsClient(this.remoteHostName,

                        this.clientCertificates,

                        this.protocols,

                        this.checkCertificateRevocation,

                        this.onAuthenticateAsClient,

                        sslStream);
            }
            catch (Exception ex)

            {
                if (sslStream != null)

                {
                    sslStream.Dispose();

                    sslStream = null;
                }

                this.connectionCallback(this, new SecureConnectionResults(ex));
            }
        }

        #endregion Private Methods
    }
}