/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (?,2007) Kylix, Initial Coder and SimpleProxy project manager
 * (Oct 27,2009) Squizzle, Added NetMessage, delegates, and NetProxy wrapper class.
 * (July 26, 2011) Gerolkae, added setting.ini switch for proxy.ini
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path
 */

using Furcadia.Logging;
using Furcadia.Net.DirectConnection;
using Furcadia.Net.Options;
using Furcadia.Net.Utils.ServerObjects;
using System;
using System.Diagnostics;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Furcadia.Net.Utils.Utilities;

namespace Furcadia.Net.Proxy
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
    public class NetProxy : ClientBase, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Max buffer size
        /// </summary>
        private static int BUFFER_CAP = 4096;

        /// <summary>
        /// Furcadia Client TCP Client
        /// </summary>
        private static TcpClient client;

        private static int furcID;

        /// <summary>
        /// Furcadia Game server TCP Client
        /// </summary>
        private static TcpClient LightBringer;

        private byte[] clientBuffer = new byte[BUFFER_CAP];
        private object ClientBufferLock = new object();
        private object ClientDataObject = new object();
        private byte[] ClientLeftOvers = new byte[BUFFER_CAP];
        private int ClientLeftOversSize = 0;
        private bool disposedValue = false;

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

        private Text.Settings settings;
        private Action SettingsRestore;

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

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// This is triggered when the Client sends data to the server.
        /// Expects a return value.
        /// </summary>
        public virtual event DataEventHandler2 ClientData2;

        #endregion Public Events

        #region Protected Internal Events

        /// <summary>
        /// Occurs when [client connected].
        /// </summary>
        protected internal event ActionDelegate ClientConnected;

        /// <summary>
        ///This is triggered when the Client Disconnects
        /// </summary>
        protected internal event ActionDelegate ClientDisconnected;

        /// <summary>
        /// Occurs when the furcadia client exits.
        /// </summary>
        protected internal event EventHandler ClientExited;

        #endregion Protected Internal Events

        /// <summary>
        ///This is triggered when the Server Disconnects
        /// </summary>
        public override event ActionDelegate ServerDisconnected;

        /// <summary>
        /// Occurs when [server connected].
        /// </summary>
        public override event ActionDelegate ServerConnected;

        #region Public Properties

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
                if (furcID == -1 || furcProcess == null)
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
                    if (client != null && client.Client != null)
                    {
                        return client.Connected;
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
        public ProxyOptions Options
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

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///  Disconnects the furcadia client and Closes the application
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static void CloseFurcadiaClient()
        {
            if (furcProcess != null)
            {
                try
                {
                    //while (!furcProcess.HasExited)
                    //{
                    //if (furcProcess == null)
                    //    break;
                    var Furc = Process.GetProcessById(furcProcess.Id, furcProcess.MachineName);
                    Furc.CloseMainWindow();
                    //}
                }
                finally
                {
                    if (furcProcess != null)
                    {
                        furcProcess.Dispose();
                        furcProcess = null;
                        furcID = -1;
                    }
                }
            }
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
        public override void Connect()
        {
            settings = new Text.Settings(options);
            var MiliSecondTime = (int)TimeSpan.FromSeconds(options.ConnectionTimeOut).TotalMilliseconds;
            CurrentConnectionAttempt = 1;

            LightBringer = new TcpClient
            {
                ReceiveTimeout = MiliSecondTime,
                SendTimeout = MiliSecondTime
            };
            var state = new State { Client = LightBringer, Success = true };
            var sucess = false;

            listen = new TcpListener(IPAddress.Any, options.LocalhostPort)
            {
                ExclusiveAddressUse = true
            };

            try
            {
                listen.Start(0);
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
                            this.ServerDisconnected?.Invoke();
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
                catch (SocketException se)
                {
                    throw se;
                }

                //Run
                // Set the Proxy/Firewall Settings
                settings.InitializeFurcadiaSettings(options.FurcadiaFilePaths.SettingsPath);
                Logger.Debug<NetProxy>("Start Furcadia");
                // LaunchFurcadia
                furcID = LaunchFurcadiaClient();

                SettingsRestore();
            }
            catch (Exception e)
            {
                SendError(e, this);
            }
            finally
            {
                // if (listen.Pending())
                // {
                //
                // listen.Server.Disconnect(true);
                // listen = null;
                listen.Stop();
                //}
            }
        }

        /// <summary>
        /// Disconnect from the Furcadia client
        /// </summary>
        public void DisconnectClientStream()
        {
            if (client != null && client.Client != null)
            {
                if (client.Connected)
                {
                    client.Close();
                    client.Dispose();
                }
                ClientDisconnected?.Invoke();
            }
            CloseFurcadiaClient();
        }

        /// <summary>
        /// Disconnect from the Furcadia gameserver and Furcadia client
        /// </summary>
        public virtual void DisconnectServerAndClientStreams()
        {
            DisconnectClientStream();

            base.Disconnect();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public virtual void SendToClient(string message)
        {
            string replaceWith = "";
            string removedBreaks = message.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
            message = removedBreaks + '\n';
            Logger.Debug<NetProxy>($"{message}");
            try
            {
                if (client.Client != null && client.GetStream().CanWrite && client.Connected)
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
                client = listen.EndAcceptTcpClient(ar);

                client.GetStream().BeginRead(clientBuffer, 0, clientBuffer.Length, new AsyncCallback(GetClientData), client);
                ClientConnected?.Invoke();
                //  listen.Stop();
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
                base.Dispose();
                if (disposing)
                {
                    if (client != null) client.Dispose();
                    if (LightBringer != null) LightBringer.Dispose();
                    if (furcProcess != null) furcProcess.Dispose();
                    if (listen != null)
                    {
                        listen.Stop();
                        //   listen = null;
                    }
                }

                disposedValue = true;
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

            if (client.Client.Connected)
            {
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
                        client.GetStream().BeginRead(clientBuffer, 0, BUFFER_CAP, new AsyncCallback(GetClientData), client);
                    else
                        ClientDisconnected?.Invoke();
                }
                catch (Exception ex) //Catch any unknown exception and close the connection gracefully
                {
                    SendToClient("[");
                    //  DisconnectClientStream();
                    SendError(ex, this);
                }
            }
        }

        private void Initialize()
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Logger.Disable<NetProxy>();
#else
            Logger.Disable<NetProxy>();
#endif
            FurcadiaUtilities = new Utils.Utilities();
            SettingsRestore += () =>
            {
                DateTime end = DateTime.Now + TimeSpan.FromSeconds(Options.ResetSettingTime);
                while (true)
                {
                    Thread.Sleep(100);
                    if (end < DateTime.Now) break;
                }
                settings.RestoreFurcadiaSettings();
            };
        }

        private int LaunchFurcadiaClient()
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
                //      CloseFurcadiaClient();
                settings.RestoreFurcadiaSettings();
            };
            if (furcProcess.Start())
            {
                Logger.Debug<NetProxy>($"Furcadia Process has Started ");
                return furcProcess.Id;
            }
            return -1;
        }

        #endregion Private Methods

        // To detect redundant calls
    }
}