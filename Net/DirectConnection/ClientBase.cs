/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (?,2007) Kylix, Initial Coder and SimpleProxy project manager
 * (Oct 27,2009) Squizzle, Added NetMessage, delegates, and ClientBase wrapper class.
 * (July 26, 2011) Gerolkae, added setting.ini switch for proxy.ini
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path
 */

using Furcadia.Logging;
using Furcadia.Net.Options;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using static Furcadia.Net.Utils.Utilities;

namespace Furcadia.Net.DirectConnection
{
    /// <summary>
    /// Furcadia base proxy connect between Client and Server. This is a low
    /// level class that handles the raw connections and furcadia
    /// proxy/firewall settings.
    /// <para>
    /// We don't have TLS/SSL handling here, so therefore, Furcadia Settings
    /// for this are disabled
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ClientBase : IDisposable
    {
        #region Protected Internal Fields

        /// <summary>
        /// Furcadia Utilities
        /// </summary>
        protected internal Utils.Utilities FurcadiaUtilities;

        #endregion Protected Internal Fields

        #region Private Fields

        /// <summary>
        /// Max buffer size
        /// </summary>
        private static int BUFFER_CAP = 4096;

        /// <summary>
        /// Furcadia Client TCP Client
        /// </summary>
        private static TcpClient LightBringer;

        private bool disposedValue = false;

        /// <summary>
        /// </summary>
        private int ENCODE_PAGE = 1252;

        private ClientOptions options;
        internal byte[] serverBuffer = new byte[BUFFER_CAP];
        // private object ServerBufferLock = new object();

        internal byte[] ServerLeftOvers = new byte[BUFFER_CAP];
        internal int ServerLeftOversSize = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Connect to game server with default settings
        /// </summary>
        public ClientBase()
        {
            options = new ClientOptions
            {
                GameServerPort = 6500
            };
            Initialize();
        }

        /// <summary>
        /// </summary>
        /// <param name="port">
        /// gameserver port
        /// </param>
        /// <param name="lport">
        /// localhost port
        /// </param>
        public ClientBase(int port)
        {
            options = new ClientOptions
            {
                GameServerPort = port
            };
            Initialize();
        }

        /// <summary>
        /// </summary>
        /// <param name="host">
        /// Game server host
        /// </param>
        /// <param name="port">
        /// </param>
        public ClientBase(string host, int port)
        {
            options = new ClientOptions
            {
                GameServerPort = port,
                GameServerHost = host
            };
            Initialize();
        }

        /// <summary>
        /// Connect to Furcadia with Proxy Options
        /// </summary>
        /// <param name="Options">
        /// </param>
        public ClientBase(ClientOptions Options)
        {
            options = Options;
            Initialize();
        }

        #endregion Public Constructors

        #region Public Delegates

        /// <summary>
        /// </summary>
        public delegate void ActionDelegate();

        /// <summary>
        /// </summary>
        public delegate string DataEventHandler(string data);

        /// <summary>
        /// </summary>
        public delegate void DataEventHandler2(string data);

        /// <summary>
        ///
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="o">The o.</param>
        public delegate void ErrorEventHandler(Exception e, Object o);

        #endregion Public Delegates

        #region Public Events

        /// <summary>
        /// This is triggered when a handled Exception is thrown.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// This is triggered when the Server sends data to the client.
        /// Doesn't expect a return value.
        /// </summary>
        public virtual event DataEventHandler2 ServerData2;

        /// <summary>
        ///This is triggered when the Server Disconnects
        /// </summary>
        public virtual event ActionDelegate ServerDisconnected;

        #endregion Public Events

        #region Protected Internal Events

        /// <summary>
        /// Occurs when [server connected].
        /// </summary>
        public virtual event ActionDelegate ServerConnected;

        #endregion Protected Internal Events

        #region Public Properties

        /// <summary>
        /// Gets the buffer capacity.
        /// </summary>
        /// <value>
        /// The buffer capacity.
        /// </value>
        public int BufferCapacity
        {
            get
            {
                return BUFFER_CAP;
            }
        }

        /// <summary>
        /// Gets the current connection attempt.
        /// </summary>
        /// <value>
        /// The current connection attempt.
        /// </value>
        public int CurrentConnectionAttempt
        { get; internal set; }

        /// <summary>
        /// Encodig
        /// <para/>
        /// DEFAULT: Windows 1252
        /// </summary>
        public int EncoderPage
        {
            get
            {
                return ENCODE_PAGE;
            }
        }

        /// <summary>
        /// Check our connection status to the game server
        /// </summary>
        public bool IsServerSocketConnected
        {
            get
            {
                try
                {
                    if (LightBringer != null && LightBringer.Client != null)
                    {
                        return LightBringer.Connected;
                    }
                }
                catch { }
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public virtual ClientOptions Options
        {
            get
            { return options; }
            set
            {
                options = value;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Connects to the Furcadia Server and starts the mini proxy.
        /// </summary>
        /// Could not fine available localhost port
        /// or
        /// there is a problem with the Proxy server
        /// or
        /// Process path not found.
        /// or
        /// Client executable '" + options.FurcadiaProcess + "' not found.
        /// </exception>
        public virtual void Connect()
        {
            var MiliSecondTime = (int)TimeSpan.FromSeconds(options.ConnectionTimeOut).TotalMilliseconds;
            CurrentConnectionAttempt = 1;

            LightBringer = new TcpClient
            {
                ReceiveTimeout = MiliSecondTime,
                SendTimeout = MiliSecondTime
            };
            var state = new State { Client = LightBringer, Success = true };
            var sucess = false;

            try
            {
                //when the connection completes before the timeout it will cause a race
                //we want EndConnect to always treat the connection as successful if it wins

                Logger.Info($"Starting connection to Furcadia gameserver.");
                while (!sucess)
                {
                    IAsyncResult ar = LightBringer.BeginConnect(options.GameServerHost, options.GameServerPort, EndConnect, state);
                    state.Success = ar.AsyncWaitHandle.WaitOne(MiliSecondTime, false);

                    sucess = (state.Success && LightBringer.Connected);
                    if (sucess)
                        break;
                    if (!sucess && CurrentConnectionAttempt < options.ConnectionRetries)
                    {
                        Logger.Warn($"Connect attempt {CurrentConnectionAttempt}/{options.ConnectionRetries} Has Failed, Trying again in {options.ConnectionTimeOut} seconds");
                        ServerDisconnected?.Invoke();
                    }
                    if (!sucess && CurrentConnectionAttempt == options.ConnectionRetries)
                    {
                        throw new NetConnectionException($"Faile to connect, Aborting");
                    }

                    CurrentConnectionAttempt++;
                    Thread.Sleep(MiliSecondTime);
                }
            }
            catch (Exception e)
            {
                SendError(e, this);
            }
        }

        /// <summary>
        /// Disconnect from the Furcadia gameserver and Furcadia client
        /// </summary>
        public virtual void Disconnect()
        {
            if (LightBringer.Connected)
            {
                LightBringer.Close();
                LightBringer.Dispose();
                ServerDisconnected?.Invoke();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public virtual void SendToServer(INetMessage message)
        {
            SendToServer(message.GetString());
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public virtual void SendToServer(string message)
        {
            SanatizeProtocolStrinng(ref message);

            if (!IsServerSocketConnected)
                return;
            try
            {
                if (LightBringer.GetStream().CanWrite)
                    LightBringer.GetStream().Write(System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message), 0, System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message).Length);
                else ServerDisconnected?.Invoke();
            }
            catch (Exception e)
            {
                ServerDisconnected?.Invoke();
                SendError(e, this);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// send errors to the error handler
        /// </summary>
        /// <param name="e"></param>
        /// <param name="o"></param>
        protected virtual void SendError(Exception e, object o)
        {
            if (Error != null)
            {
                Error?.Invoke(e, o);
            }
            else
            {
                throw e;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Asynchronouses the listener.
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void AsyncListener(IAsyncResult ar)
        {
            try
            {
                LightBringer.GetStream().BeginRead(serverBuffer, 0, serverBuffer.Length, new AsyncCallback(GetServerData), LightBringer);
                ServerConnected?.Invoke();
            }
            catch (Exception e)
            {
                SendError(e, this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (LightBringer != null) LightBringer.Dispose();
                }

                disposedValue = true;
            }
        }

        internal void EndConnect(IAsyncResult ar)
        {
            var state = (State)ar.AsyncState;
            TcpClient ThisClient = state.Client;
            if (ThisClient == null)
                return;
            try
            {
                ThisClient.EndConnect(ar);
            }
            catch { }

            if (ThisClient.Connected && state.Success)
                return;

            ThisClient.Close();
        }

        /// <summary>
        /// Handle the raw server data
        /// </summary>
        /// <param name="ar">
        /// </param>
        internal void GetServerData(IAsyncResult ar)
        {
            LightBringer = (TcpClient)ar.AsyncState;

            if (LightBringer.Client.Connected)
            {
                try
                {
                    int read = 0;
                    // TcpClient GameServer = listener.EndAcceptTcpClient(ar);
                    read = LightBringer.GetStream().EndRead(ar);
                    int currStart = 0;
                    int currEnd = -1;

                    for (int i = 0; i < read; i++)
                    {
                        if (i < BUFFER_CAP && serverBuffer[i] == 10)
                        {
                            // Set the end of the data
                            currEnd = i;

                            // If we have left overs from previous runs:
                            if (ServerLeftOversSize != 0)
                            {
                                // Allocate enough space for the joined buffer
                                byte[] joinedData = new byte[ServerLeftOversSize + (currEnd - currStart + 1)];

                                // And add the current read as well
                                Array.Copy(ServerLeftOvers, 0, joinedData, 0, ServerLeftOversSize);

                                // Get the leftover from the previous read
                                Array.Copy(serverBuffer, currStart, joinedData, ServerLeftOversSize, (currEnd - currStart + 1));

                                // Now handle it string test =
                                ServerData2?.Invoke(System.Text.Encoding.GetEncoding(GetEncoding).GetString(joinedData,
                                             currStart, joinedData.Length));
                                ServerLeftOversSize = 0;
                            }
                            else
                            {
                                ServerData2?.Invoke(System.Text.Encoding.GetEncoding(GetEncoding).GetString(serverBuffer,
                                 currStart, currEnd - currStart));

                                // Handle the data, from the start to the end,
                                // between delimiter '\n'
                            }
                            // Set the new start - after our delimiter
                            currStart = i + 1;
                        }
                    }

                    // See if we still have any leftovers
                    if (currStart < read)
                    {
                        // ServerLeftOvers = new byte[read - currStart];

                        Array.Copy(serverBuffer, currStart, ServerLeftOvers, 0, read - currStart);
                        ServerLeftOversSize = read - currStart;
                    }

                    if (IsServerSocketConnected)
                        LightBringer.GetStream().BeginRead(serverBuffer, 0, BUFFER_CAP, new AsyncCallback(GetServerData), LightBringer);
                }
                catch (Exception ex) //Catch any unknown exception and close the connection gracefully
                {
                    ServerDisconnected?.Invoke();
                    SendError(ex, this);
                }
            }
            else
                ServerDisconnected?.Invoke();
        }

        private void Initialize()
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Logger.Disable<ClientBase>();
#else
            Logger.Disable<ClientBase>();
#endif
            FurcadiaUtilities = new Utils.Utilities();
        }

        /// <summary>
        /// Sanatizes the protocol strinng.
        /// </summary>
        /// <param name="message">The message.</param>
        // TODO: Move this to utils?
        internal void SanatizeProtocolStrinng(ref string message)
        {
            string replaceWith = "";
            string removedBreaks = message.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            message = removedBreaks + '\n';
        }

        #endregion Private Methods

        #region Private Classes

        internal class State
        {
            #region Public Constructors

            public State()
            {
                Success = true;
            }

            #endregion Public Constructors

            #region Public Properties

            public TcpClient Client { get; set; }
            public bool Success { get; set; }

            #endregion Public Properties
        }

        #endregion Private Classes

        // To detect redundant calls
    }
}