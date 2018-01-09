/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (?,2007) Kylix, Initial Coder and SimpleProxy project manager
 * (Oct 27,2009) Squizzle, Added NetMessage, delegates, and NetProxy wrapper class.
 * (July 26, 2011) Gerolkae, added setting.ini switch for proxy.ini
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path
 */

using Furcadia.Logging;
using Furcadia.Net.Options;
using System;
using System.Diagnostics;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Furcadia.Net.Utils.Utilities;

namespace Furcadia.Net
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
    public class NetProxy : IDisposable
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
        private static TcpClient client;

        /// <summary>
        /// Furcadia Game server TCP Client
        /// </summary>
        private static TcpClient LightBringer;

        private byte[] clientBuffer = new byte[BUFFER_CAP];
        private byte[] serverBuffer = new byte[BUFFER_CAP];

        private object ClientBufferLock = new object();

        /// <summary>
        /// </summary>
        private int ENCODE_PAGE = 1252;

        /// <summary>
        /// Furcadia Client Process
        /// </summary>
        private static Process furcProcess;

        /// <summary>
        /// Allow Furcadia Client to connect to us
        /// </summary>
        private TcpListener listen;

        private ProxyOptions options;

        private object ServerBufferLock = new object();

        private Text.Settings settings;

        private object ClientDataObject = new object();
        private byte[] ClientLeftOvers = new byte[BUFFER_CAP];

        private int ClientLeftOversSize = 0;

        private byte[] ServerLeftOvers = new byte[BUFFER_CAP];

        private int ServerLeftOversSize = 0;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Connect to game server with default settings
        /// </summary>
        public NetProxy()
        {
            options = new ProxyOptions
            {
                LocalhostPort = 6700,
                GameServerPort = 6500
            };
            Initialize();
        }

        /// <summary>
        /// </summary>
        /// <param name="LocalPort">
        /// </param>
        public NetProxy(int LocalPort)
        {
            options = new ProxyOptions
            {
                LocalhostPort = LocalPort,
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
        public NetProxy(int port, int lport)
        {
            options = new ProxyOptions
            {
                LocalhostPort = lport,
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
        public NetProxy(string host, int port)
        {
            options = new ProxyOptions
            {
                GameServerPort = port,
                LocalhostPort = 6700,
                GameServerHost = host
            };
            Initialize();
        }

        /// <summary>
        /// Connect to Furcadia with Proxy Options
        /// </summary>
        /// <param name="Options">
        /// </param>
        public NetProxy(ProxyOptions Options)
        {
            options = Options;
            Initialize();
        }

        /// <summary>
        /// Connect to the Game serer by Host name to IP address
        /// </summary>
        /// <param name="host">
        /// GameServer Ip address or hostname
        /// </param>
        /// <param name="port">
        /// Game server port
        /// </param>
        /// <param name="lport">
        /// Localhost port
        /// </param>
        public NetProxy(string host, int port, int lport)
        {
            options = new ProxyOptions
            {
                LocalhostPort = lport,
                GameServerHost = host,
                GameServerPort = port
            };
            Initialize();
        }

        private void Initialize()
        {
            client = new TcpClient();
#if DEBUG
            if (!Debugger.IsAttached)
                Logger.Disable<NetProxy>();
#else
            Logger.Disable<NetProxy>();
#endif
            FurcadiaUtilities = new Utils.Utilities();
            ClientLaunched += () => LaunchFurcadia();
            SettingsRestore += () =>
            {
                DateTime end = DateTime.Now + TimeSpan.FromSeconds(10);
                while (true)
                {
                    Thread.Sleep(100);
                    if (end < DateTime.Now) break;
                }
                settings.RestoreFurcadiaSettings();
            };
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
        /// <param name="n">The n.</param>
        public delegate void ErrorEventHandler(Exception e, Object o);

        #endregion Public Delegates

        #region Public Events

        /// <summary>
        /// This is triggered when the Client sends data to the server.
        /// Expects a return value.
        /// </summary>
        public virtual event DataEventHandler2 ClientData2;

        /// <summary>
        /// This is triggered when the Server sends data to the client.
        /// Doesn't expect a return value.
        /// </summary>
        public virtual event DataEventHandler2 ServerData2;

        /// <summary>
        ///This is triggered when the Server Disconnects
        /// </summary>
        public event ActionDelegate ServerDisconnected;

        #endregion Public Events

        #region Protected Internal Events

        /// <summary>
        ///This is triggered when the Client Disconnects
        /// </summary>
        protected internal event ActionDelegate ClientDisconnected;

        /// <summary>
        /// Occurs when the furcadia client exits.
        /// </summary>
        protected internal event EventHandler ClientExited;

        private Func<int> ClientLaunched;
        private Action SettingsRestore;

        protected internal event ActionDelegate ClientConnected;

        protected internal event ActionDelegate ServerConnected;

        #endregion Protected Internal Events

        #region Protected Events

        /// <summary>
        /// This is triggered when a handled Exception is thrown.
        /// </summary>
        public event ErrorEventHandler Error;

        #endregion Protected Events

        #region Public Properties

        /// <summary>
        /// Gets the current connection attempt.
        /// </summary>
        /// <value>
        /// The current connection attempt.
        /// </value>
        public int CurrentConnectionAttempt
        { get; private set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public virtual ProxyOptions Options
        {
            get
            { return options; }
            set
            {
                //if (IsServerSocketConnected)
                //    throw new InvalidOperationException("NetProxy is alread connected");
                options = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [the Furcadia lient is a running process].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [furcadia client is  a running process]; otherwise, <c>false</c>.
        /// </value>
        public bool FurcadiaClientIsRunning
        {
            get
            {
                if (furcProcess == null)
                    return false;
                if (furcID == -1)
                    return false;
                try
                {
                    furcProcess = Process.GetProcessById(furcID);
                    return !furcProcess.HasExited;
                }
                catch { }
                return false;
            }
        }

        /// <summary>
        /// Gets the furcadia process identifier.
        /// </summary>
        /// <value>
        /// The furcadia process identifier.
        /// </value>
        public int FurcadiaProcessID
        {
            get
            {
                return furcID;
            }
        }

        private static int furcID;

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
        /// Gets a value indicating whether this instance is client socket connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is client socket connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsClientSocketConnected
        {
            get
            {
                try
                {
                    if (client != null)
                    {
                        if (client.Client != null)
                            return client.Client.Connected;
                    }
                }
                catch { }
                return false;
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
                    if (LightBringer != null)
                    {
                        return LightBringer.Connected;
                    }
                }
                catch { }
                return false;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// send errors to the error handler
        /// </summary>
        /// <param name="e"></param>
        /// <param name="o"></param>
        protected virtual void SendError(Exception e, object o)
        {
            Logger.Error<NetProxy>($"{e} {o}");
            if (Error != null)
            {
                Error?.Invoke(e, o);
            }
            else
                throw e;
        }

        /// <summary>
        /// Disconnect from the Furcadia client
        /// </summary>
        public void ClientDisconnect()
        {
            if (listen != null)
            {
                listen.Stop();
            }
            if (client != null && client.Client != null)
            {
                if (client.Connected)
                {
                    client.Close();
                }
                if (FurcadiaClientIsRunning)
                    CloseClient();
                ClientDisconnected?.Invoke();
            }
        }

        /// <summary>
        ///  Disconnects the furcadia client and Closes the application
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void CloseClient()
        {
            if (FurcadiaProcessID > 0)
            {
                furcProcess = Process.GetProcessById(furcID);
                furcProcess.CloseMainWindow();
                try
                {
                    furcProcess.Close();
                }
                finally
                {
                    furcProcess.Dispose();
                    furcProcess = null;
                }
            }
        }

        private class State
        {
            public State()
            {
                Success = true;
            }

            public TcpClient Client { get; set; }
            public bool Success { get; set; }
        }

        private void EndConnect(IAsyncResult ar)
        {
            var state = (State)ar.AsyncState;
            TcpClient ThisClient = state.Client;

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
        /// Connects to the Furcadia Server and starts the mini proxy.
        /// </summary>
        /// <exception cref="NetProxyException">
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
            settings = new Text.Settings(options);
            CurrentConnectionAttempt = 1;
            LightBringer = new TcpClient();
            var state = new State { Client = LightBringer, Success = true };
            var sucess = false;
            var MiliSecondTime = (int)TimeSpan.FromSeconds(options.ConnectionTimeOut).TotalMilliseconds;
            try
            {
                try
                {
                    listen = new TcpListener(IPAddress.Any, options.LocalhostPort);
                    listen.Start();

                    //when the connection completes before the timeout it will cause a race
                    //we want EndConnect to always treat the connection as successful if it wins

                    Logger.Info($"Starting connection to Furcadia gameserver.");
                    while (!sucess)
                    {
                        LightBringer = new TcpClient
                        {
                            ReceiveTimeout = MiliSecondTime,
                            SendTimeout = MiliSecondTime
                        };

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
                            throw new NetProxyException($"Faile to connect, Aborting");
                        }

                        CurrentConnectionAttempt++;
                        Thread.Sleep(MiliSecondTime);
                    }

                    listen.BeginAcceptTcpClient(new AsyncCallback(AsyncListener), listen);
                }
                catch (Exception ne)
                {
                    listen.Stop();
                    ServerDisconnected?.Invoke();
                    throw ne;
                }

                //Run
                // Set the Proxy/Firewall Settings
                settings.InitializeFurcadiaSettings(options.FurcadiaFilePaths.SettingsPath);
                Logger.Debug<NetProxy>("Start Furcadia");
                // LaunchFurcadia
                var FurcThread = new Thread(() => furcID = ClientLaunched());
                FurcThread.Start();
                SettingsRestore();
            }
            catch (Exception e)
            {
                SendError(e, this);
            }
        }

        private int LaunchFurcadia()
        {
            var furcadiaProcessInfo = new ProcessStartInfo
            {
                FileName = Options.FurcadiaProcess,
                Arguments = Options.CharacterIniFile,
                WorkingDirectory = Options.FurcadiaInstallPath
            };
            furcProcess = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = furcadiaProcessInfo
            };

            furcProcess.Exited += ClientExited;
            furcProcess.Exited += (s, e) =>
            {
                Logger.Debug<NetProxy>($"Furcadia Process Exited ");

                if (IsClientSocketConnected)
                {
                    client.Close();
                }
                settings.RestoreFurcadiaSettings();
                furcID = -1;
                furcProcess = null;
            };
            if (furcProcess.Start())
            {
                Logger.Debug<NetProxy>($"Furcadia Process has Started ");
                return furcProcess.Id;
            }
            return -1;
        }

        /// <summary>
        /// Disconnect from the Furcadia gameserver and Furcadia client
        /// </summary>
        public virtual void Disconnect()
        {
            ClientDisconnect();

            if (LightBringer.Connected)
            {
                LightBringer.Close();
                ServerDisconnected?.Invoke();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public virtual void SendToClient(string message)
        {
            if (!message.EndsWith(@"\n"))
                message += '\n';
            try
            {
                if (client.Client != null && client.GetStream().CanWrite == true && client.Connected == true)
                    client.GetStream().Write(System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message), 0, System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message).Length);
            }
            catch (Exception e) { SendError(e, this); }
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public virtual void SendToClient(INetMessage message)
        {
            SendToClient(message.GetString());
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
            string replaceWith = "";
            string removedBreaks = message.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            message += '\n';

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
                if (IsClientSocketConnected)
                {
                    client.Close();
                    ClientDisconnected?.Invoke();
                }
                SendError(e, this);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Asynchronouses the listener.
        /// </summary>
        /// <param name="ar">The ar.</param>
        /// <exception cref="NetProxyException">There was a problem connecting to the server.</exception>
        private void AsyncListener(IAsyncResult ar)
        {
            try
            {
                listen = (TcpListener)ar.AsyncState;

                try
                {
                    client = listen.EndAcceptTcpClient(ar);
                }
                catch (SocketException se)
                {
                    if (se.ErrorCode > 0) throw se;
                }

                try
                {
                    client.GetStream().BeginRead(clientBuffer, 0, clientBuffer.Length, new AsyncCallback(GetClientData), client);
                    ClientConnected?.Invoke();
                }
                catch (Exception ex)
                {
                    SendError(ex, this);
                }

                try
                {
                    LightBringer.GetStream().BeginRead(serverBuffer, 0, serverBuffer.Length, new AsyncCallback(GetServerData), LightBringer);
                    ServerConnected?.Invoke();
                }
                catch (Exception ex)
                {
                    SendError(ex, this);
                }
            }
            catch (Exception e)
            {
                SendError(e, this);
            }
        }

        /// <summary>
        /// handle the raw data coming from he Furcadia client
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void GetClientData(IAsyncResult ar)
        {
            client = (TcpClient)ar.AsyncState;

            if (!IsClientSocketConnected)
            {
                ClientDisconnected?.Invoke();
                return;
            }
            try
            {
                int read = 0;

                read = client.GetStream().EndRead(ar);
                int currStart = 0;
                int currEnd = -1;

                for (int i = 0; i < read; i++)
                {
                    if (i < BUFFER_CAP && clientBuffer[i] == '\n')//10
                    {
                        // Set the end of the data
                        currEnd = i;

                        // If we have left overs from previous runs:
                        if (ClientLeftOversSize != 0)
                        {
                            // Allocate enough space for the joined buffer
                            byte[] joinedData = new byte[ClientLeftOversSize + (currEnd - currStart + 1)];

                            // And add the current read as well
                            Array.Copy(ClientLeftOvers, 0, joinedData, 0, ClientLeftOversSize);

                            // Get the leftover from the previous read
                            Array.Copy(clientBuffer, currStart, joinedData, ClientLeftOversSize, (currEnd - currStart + 1));

                            ClientData2?.Invoke(System.Text.Encoding.GetEncoding(GetEncoding).GetString(joinedData,
                           0, joinedData.Length));

                            // Mark that we don't have any leftovers anymore
                            ClientLeftOversSize = 0;
                        }
                        else
                        {
                            ClientData2?.Invoke(System.Text.Encoding.GetEncoding(GetEncoding).GetString(clientBuffer,
                            currStart, currEnd - currStart));
                        }

                        // Set the new start - after our delimiter
                        currStart = i + 1;
                    }
                }
                // See if we still have any leftovers
                if (currStart < read)
                {
                    Array.Copy(clientBuffer, currStart, ClientLeftOvers, 0, read - currStart);
                    ClientLeftOversSize = read - currStart;
                }

                if (IsClientSocketConnected)
                {
                    client.GetStream().BeginRead(clientBuffer, 0, BUFFER_CAP, new AsyncCallback(GetClientData), client);
                }
                else
                    ClientDisconnected?.Invoke();
            }
            catch (Exception ex) //Catch any unknown exception and close the connection gracefully
            {
                ClientDisconnected?.Invoke();
                SendError(ex, this);
            }
        }

        /// <summary>
        /// Handle the raw server data
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void GetServerData(IAsyncResult ar)
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

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (client != null) client.Dispose();
                    if (LightBringer != null) LightBringer.Dispose();
                    if (furcProcess != null) furcProcess.Dispose();
                    if (listen != null)
                    {
                        listen.Stop();
                        listen = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public virtual void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support

        #endregion Private Methods
    }
}