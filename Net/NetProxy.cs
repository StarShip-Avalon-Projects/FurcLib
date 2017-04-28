/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (?,2007) Kylix, Initial Coder and SimpleProxy project manager
 * (Oct 27,2009) Squizzle, Added NetMessage, delegates, and NetProxy wrapper class.
 * (July 26, 2011) Gerolkae, added setting.ini switch for proxy.ini
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path
 */

using Furcadia.Net.Utils;
using Furcadia.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

//using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Timers;

namespace Furcadia.Net
{
    // [Obsolete("Use Net.Proxy Classes instead", false)]
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
    public class NetProxy
    {
        #region Private Fields

        private Options.ProxyOptions options;
        private Process proc;
        private Text.Settings settings;

        #endregion Private Fields

        #region Event Handling

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
        /// </summary>
        public delegate void ErrorEventHandler(Exception e, Object o, String n);

        /// <summary>
        /// This is triggered when the Client sends data to the server.
        /// </summary>
        public event DataEventHandler ClientData;

        /// <summary>
        /// This is triggered when the Client sends data to the server.
        /// Expects a return value.
        /// </summary>
        public event DataEventHandler2 ClientData2;

        /// <summary>
        /// This is triggered when the Server sends data to the client.
        /// Doesn't expect a return value.
        /// </summary>
        public event DataEventHandler2 ServerData2;

        /// <summary>
        ///This is triggered when the Server Disconnects
        /// </summary>
        public event ActionDelegate ServerDisConnected;

        /// <summary>
        ///This is triggered when the Client Disconnects
        /// </summary>
        protected internal event ActionDelegate ClientDisConnected;

        /// <summary>
        /// This is triggered when the user has exited/logoff Furcadia and
        /// the Furcadia client is closed.
        /// </summary>
        protected internal event ActionDelegate ClientExited;

        //public delegate void ErrorEventHandler(Exception e);
        /// <summary>
        ///This is triggered when the
        /// </summary>
        protected internal event ActionDelegate Connected;

        /// <summary>
        /// This is triggered when a handled Exception is thrown.
        /// </summary>
        protected internal event ErrorEventHandler Error;

        /// <summary>
        /// This is triggered when t client is closed.
        /// </summary>
        protected internal event ActionDelegate FurcSettingsRestored;

        /// <summary>
        /// This is triggered when the Server sends data to the client.
        /// Expects a return Value
        /// </summary>
        protected internal event DataEventHandler ServerData;

        #endregion Event Handling

        #region Private Declarations

        private static string[] BackupSettings;

        /// <summary>
        /// Max buffer size
        /// </summary>
        private static int BUFFER_CAP = 2048;

        /// <summary>
        /// </summary>
        private static int ENCODE_PAGE = 1252;

        private static System.Timers.Timer NewsTimer;
        private IPEndPoint _endpoint;

        /// <summary>
        /// Process IP for Furcadia.exe
        /// </summary>
        private int _procID;

        private string _ServerLeftOvers;

        /// <summary>
        /// Furcadia Client Connection
        /// </summary>
        private TcpClient client = new TcpClient();

        private byte[] clientBuffer = new byte[BUFFER_CAP], serverBuffer = new byte[BUFFER_CAP];
        private string clientBuild, serverBuild;
        private TcpListener listen;

        /// <summary>
        /// </summary>
        private TcpClient server;

        #endregion Private Declarations

        #region Constructors

        /// <summary>
        /// Connect to game servver with default settings
        /// </summary>
        public NetProxy()
        {
            options = new Options.ProxyOptions();
            settings = new Text.Settings(options.LocalhostPort);
            string SetPath = options.FurcadiaFilePaths.GetLocalSettingsPath();
            string SetFile = "settings.ini";
            string[] sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
            options.GameServerPort = Convert.ToInt32(FurcIni.GetUserSetting("PreferredServerPort", sett));
            _endpoint = ConverHostToIP(Utilities.GameServerHost, options.GameServerPort);
        }

        /// <summary>
        /// </summary>
        /// <param name="port">
        /// </param>
        public NetProxy(int port)
        {
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            try
            {
                _endpoint = new IPEndPoint(Dns.GetHostEntry(options.GameServerHost).AddressList[0], options.LocalhostPort);
            }
            catch { }
        }

        /// <summary>
        /// </summary>
        /// <param name="port">
        /// </param>
        /// <param name="lport">
        /// </param>
        public NetProxy(int port, int lport)
        {
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            settings = new Text.Settings(options.LocalhostPort);
            _endpoint = ConverHostToIP(options.GameServerHost, options.GameServerPort);
        }

        /// <summary>
        /// </summary>
        /// <param name="host">
        /// </param>
        /// <param name="port">
        /// </param>
        public NetProxy(string host, int port)
        {
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            settings = new Text.Settings(options.LocalhostPort);
            _endpoint = ConverHostToIP(options.GameServerHost, options.GameServerPort);
        }

        /// <summary>
        /// Connect to Furcadia with Proxy Options
        /// </summary>
        /// <param name="Options">
        /// </param>
        public NetProxy(Options.ProxyOptions Options)
        {
            options = Options;
            settings = new Text.Settings(options.LocalhostPort);
            _endpoint = ConverHostToIP(options.GameServerHost, options.GameServerPort);
        }

        /// <summary>
        /// Connect to the Game serer by Host name ot IP address
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
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            settings = new Text.Settings(options.LocalhostPort);
            _endpoint = ConverHostToIP(options.GameServerHost, options.GameServerPort);
        }

        /// <summary>
        /// </summary>
        /// <param name="endpoint">
        /// </param>
        /// <param name="lport">
        /// </param>
        public NetProxy(IPEndPoint endpoint, int lport)
        {
            options = new Options.ProxyOptions();
            options.LocalhostPort = lport;
            settings = new Text.Settings(options.LocalhostPort);
            try
            {
                _endpoint = endpoint;
            }
            catch { }
        }

        private IPEndPoint ConverHostToIP(string HostName, int ServerPort)
        {
            IPAddress IP;
            if (IPAddress.TryParse(HostName, out IP))
                try
                {
                    return new IPEndPoint(IP, ServerPort);
                }
                catch { }
            else
            {
                try
                {
                    return new IPEndPoint(Dns.GetHostEntry(HostName).AddressList[0], ServerPort);
                }
                catch
                {
                    return new IPEndPoint(Utilities.GameServerIp, ServerPort);
                }
            }
            return null;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Check the connection Status of the Furcadia Client
        /// </summary>
        public bool IsClientConnected
        {
            get
            {
                if (client != null)
                    return client.Connected;
                else
                    return false;
            }
        }

        /// <summary>
        /// Check our connection status to the game server
        /// </summary>
        public bool IsServerConnected
        {
            get
            {
                if (server != null)
                    return server.Connected;
                else
                    return false;
            }
        }

        /// <summary>
        /// Process ID for closing Furcadia.exe
        /// </summary>
        public int ProcID
        {
            get { return _procID; }
            set { _procID = value; }
        }

        #endregion Properties

        #region Public Static Properties

        /// <summary>
        /// </summary>
        public static int BufferCapacity
        {
            get
            {
                return BUFFER_CAP;
            }
        }

        /// <summary>
        /// </summary>
        public static int EncoderPage
        {
            get
            {
                return ENCODE_PAGE;
            }
        }

        #endregion Public Static Properties

        #region Public Methods

        /// <summary>
        /// Disconnect from the Furcadia client
        /// </summary>
        public void ClientDisconnect()
        {
            if (listen != null) listen.Stop();

            if (client != null && client.Connected == true)
            {
                NetworkStream clientStream = client.GetStream();
                if (clientStream != null)
                {
                    clientStream.Flush();
                    clientStream.Close();
                }

                client.Close();
            }
        }

        /// <summary>
        /// Closes the Client Connection
        /// </summary>
        public void CloseClient()
        {
            try
            {
                if (listen != null) listen.Stop();
                if (client != null && client.Connected == true)
                {
                    NetworkStream clientStream = client.GetStream();
                    if (clientStream != null)
                    {
                        clientStream.Flush();
                        clientStream.Dispose();
                        clientStream.Close();
                    }

                    client.Close();
                }
            }
            catch (Exception e) { Error?.Invoke(e, this, "CloseClient()"); }
        }

        /// <summary>
        /// Connects to the Furcadia Server and starts the mini proxy.
        /// </summary>
        public void Connect()
        {
            if (string.IsNullOrEmpty(options.CharacterIniFile))
                throw new Proxy.CharacterNotFoundException("Character.ini not specified");
            try
            {
                if (listen != null)
                {
                    listen.Start();
                    listen.BeginAcceptTcpClient(new AsyncCallback(AsyncListener), listen);
                }
                else
                {
                    bool isAvailable = true;
                    IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                    TcpConnectionInformation[] tcpInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                    foreach (TcpConnectionInformation tcpi in tcpInfoArray)
                    {
                        if (tcpi.LocalEndPoint.Port == _endpoint.Port)
                        {
                            isAvailable = false;
                            break;
                        }
                    }
                    if (isAvailable)
                    {
                        try
                        {
                            listen = new TcpListener(IPAddress.Any, options.LocalhostPort);
                            listen.Start();
                            listen.BeginAcceptTcpClient(new AsyncCallback(AsyncListener), listen);
                        }
                        catch (SocketException)
                        {
                            options.LocalhostPort++;
                            listen = new TcpListener(IPAddress.Any, options.LocalhostPort);
                            listen.Start();
                            listen.BeginAcceptTcpClient(new AsyncCallback(AsyncListener), listen);
                        }
                    }
                    else throw new NetProxyException("Port " + options.LocalhostPort.ToString() + " is being used.");
                }
                // UAC Perms Needed to Create proxy.ini Win 7 Change your
                // UAC Level or add file create Permissions to [%program
                // files%/furcadia] Maybe Do this at install
                BackupSettings = settings.InitializeFurcadiaSettings(options.FurcadiaFilePaths.GetLocalSettingsPath());
                //Run

                //check ProcessPath is not a directory
                if (!Directory.Exists(options.FurcadiaInstallPath)) throw new NetProxyException("Process path not found.");
                if (!File.Exists(Path.Combine(options.FurcadiaInstallPath, options.FurcadiaProcess))) throw new NetProxyException("Client executable '" + options.FurcadiaProcess + "' not found.");
                proc = new System.Diagnostics.Process(); //= System.Diagnostics.Process.Start(Process,ProcessCMD );
                proc.EnableRaisingEvents = true;
                proc.StartInfo.FileName = options.FurcadiaProcess;
                proc.StartInfo.Arguments = options.CharacterIniFile;
                proc.StartInfo.WorkingDirectory = options.FurcadiaInstallPath;
                proc.Start();
                proc.Exited += delegate
                {
                    ClientExited?.Invoke();
                };
                ProcID = proc.Id;
            }
            catch (NetProxyException e)
            {
                throw e;
            }
            catch (Exception e) { if (Error != null) Error(e, this, "Connect()"); else throw e; }
        }

        /// <summary>
        /// Disconnect from the Furcadia gameserver and Furcadia client
        /// </summary>
        public virtual void Disconnect()
        {
            if (listen != null) listen.Stop();

            if (client != null && client.Connected == true)
            {
                NetworkStream clientStream = client.GetStream();
                if (clientStream != null)
                {
                    clientStream.Flush();
                    clientStream.Close();
                }

                client.Close();
            }

            if (server != null && server.Connected == true)
            {
                NetworkStream serverStream = server.GetStream();
                if (serverStream != null)
                {
                    serverStream.Flush();
                    serverStream.Close();
                }
                server.Close();
            }
        }

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="message">
        /// </param>
        public virtual void SendToClient(string message)
        {
            if (!message.EndsWith("\n"))
                message += '\n';
            try
            {
                if (client.Client != null && client.GetStream().CanWrite == true && client.Connected == true)
                    client.GetStream().Write(System.Text.Encoding.GetEncoding(EncoderPage).GetBytes(message), 0, System.Text.Encoding.GetEncoding(EncoderPage).GetBytes(message).Length);
            }
            catch (Exception e) { Error?.Invoke(e, this, "SendClient()"); }
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
            if (!message.EndsWith("\n"))
                message += '\n';
            try
            {
                if (server.GetStream().CanWrite)
                    server.GetStream().Write(System.Text.Encoding.GetEncoding(EncoderPage).GetBytes(message), 0, System.Text.Encoding.GetEncoding(EncoderPage).GetBytes(message).Length);
            }
            catch (Exception e)
            {
                Error?.Invoke(e, this, "SendServer");
                ServerDisConnected?.Invoke();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (BackupSettings != null)
                    settings.RestoreFurcadiaSettings(BackupSettings);
                if (listen != null) listen.Stop();

                if (client != null && client.Connected == true)
                {
                    NetworkStream clientStream = client.GetStream();
                    if (clientStream != null)
                    {
                        clientStream.Flush();
                        clientStream.Close();
                        clientStream.Dispose();
                    }

                    client.Close();
                }

                if (server != null && server.Connected == true)
                {
                    NetworkStream serverStream = server.GetStream();
                    if (serverStream != null)
                    {
                        serverStream.Flush();
                        serverStream.Close();
                        serverStream.Dispose();
                    }
                    server.Close();
                }
            }
            // Free other state (managed objects).

            // Free your own state (unmanaged objects). Set large fields to null.
        }

        #endregion Public Methods

        #region Private Methods

        private object lck = new object();

        /// <summary>
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void AsyncListener(IAsyncResult ar)
        {
            try
            {
                try
                {
                    client = listen.EndAcceptTcpClient(ar);
                }
                catch (SocketException se)
                {
                    listen.Stop();
                    if (se.ErrorCode > 0) throw se;
                }
                //listen.Stop();
                // Connects to the server
                server = new TcpClient(Utilities.GameServerHost, _endpoint.Port);
                if (!server.Connected) throw new Exception("There was a problem connecting to the server.");

                client.GetStream().BeginRead(clientBuffer, 0, clientBuffer.Length, new AsyncCallback(GetClientData), client);
                server.GetStream().BeginRead(serverBuffer, 0, serverBuffer.Length, new AsyncCallback(GetServerData), server);

                if (Connected != null)
                {
                    Connected();

                    // reset settings.ini 10second delay timer
                    NewsTimer = new System.Timers.Timer();
                    NewsTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                    NewsTimer.Enabled = true;
                    NewsTimer.Interval = 10000;
                    NewsTimer.AutoReset = false;
                }
            }
            catch (Exception e) { Error?.Invoke(e, this, "AsyncListener()"); }
        }

        /// <summary>
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void GetClientData(IAsyncResult ar)
        {
            lock (lck)
            {
                try
                {
                    if (client.Connected == false)
                    {
                        throw new SocketException((int)SocketError.NotConnected);
                    }
                    List<string> lines = new List<string>();

                    //read = number of bytes read
                    int read = client.GetStream().EndRead(ar);
                    //ClientBuild is a string containing data read off the clientBuffer
                    clientBuild = System.Text.Encoding.GetEncoding(EncoderPage).GetString(clientBuffer, 0, read);
                    while (client.GetStream().DataAvailable)
                    {
                        //clientBuffer.Length = NetProxy.BUFFER_CAP

                        if (clientBuffer.Length <= 0)
                            ClientDisConnected();
                        int pos = client.GetStream().Read(clientBuffer, 0, clientBuffer.Length);
                        clientBuild += System.Text.Encoding.GetEncoding(EncoderPage).GetString(clientBuffer, 0, pos);
                    }

                    //Every line should end with '\n'
                    //Split function removes the last character
                    lines.AddRange(clientBuild.Split('\n'));
                    for (int i = 0; i < lines.Count; i++)
                    {
                        ClientData2(lines[i]);
                        // we want ServerConnected and Check for null data
                        // Application may intentionally return ClientData =
                        // null to Avoid "Throat Tired" Syndrome. Let
                        // Application handle packet routing.
                        if (IsServerConnected == true && ClientData != null)
                        {
                            INetMessage msg = new NetMessage();
                            //Send the line to the ClientData event and write the return value to a new NetMessage
                            msg.Write(ClientData(lines[i]));
                            //If the NetMessage doesn't have '\n' at the end add it
                            //The '\n' separates the server/client protocols
                            if (msg.GetString().EndsWith("\n") == false) msg.Write("\n");
                            //Send it on it's way...
                            SendToServer(msg);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (client.Connected == true) ClientDisConnected();
                    Error?.Invoke(e, this, "GetClientData()");
                } // else throw e;
                if (IsClientConnected && clientBuild.Length < 1 || IsClientConnected == false)
                    ClientDisConnected?.Invoke();
                else
                {
                    if (client.Connected == true && clientBuild.Length >= 1)
                    {
                        client.GetStream().BeginRead(clientBuffer, 0, clientBuffer.Length, new AsyncCallback(GetClientData), client);
                    }
                }
            }
        }

        private void GetServerData(IAsyncResult ar)
        {
            lock (lck)
            {
                try
                {
                    if (IsServerConnected == false)
                    {
                        throw new SocketException((int)SocketError.NotConnected);
                    }
                    else
                    {
                        List<string> lines = new List<string>();
                        int read = server.GetStream().EndRead(ar);

                        if (read < 1)
                        {
                            ServerDisConnected?.Invoke(); return;
                        }
                        //If we have left over data add it to this server build
                        if (!string.IsNullOrEmpty(_ServerLeftOvers) && _ServerLeftOvers.Length > 0)
                            serverBuild += _ServerLeftOvers;
                        serverBuild = System.Text.Encoding.GetEncoding(EncoderPage).GetString(serverBuffer, 0, read);

                        while (server.GetStream().DataAvailable == true)
                        {
                            int pos = server.GetStream().Read(serverBuffer, 0, serverBuffer.Length);
                            serverBuild += System.Text.Encoding.GetEncoding(EncoderPage).GetString(serverBuffer, 0, pos);
                        }
                        lines.AddRange(serverBuild.Split('\n'));
                        if (!serverBuild.EndsWith("\n"))
                        {
                            _ServerLeftOvers += lines[lines.Count - 1];
                            lines.RemoveAt(lines.Count - 1);
                        }

                        for (int i = 0; i < lines.Count; i++)
                        {
                            ServerData2?.Invoke(lines[i]);
                            if (IsClientConnected == true && ServerData != null &&
                                ServerData(lines[i]) != "" && ServerData(lines[i]) != null)
                            {
                                if (i < lines.Count)
                                {
                                    NetMessage msg = new NetMessage();
                                    msg.Write(ServerData(lines[i]) + '\n');
                                    SendToClient(msg);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // if (IsServerConnected == true) ServerDisConnected();
                    Error?.Invoke(e, this, "GetServerData()");
                    ServerDisConnected?.Invoke();
                    return;
                } //else throw e;
                  // Detect if client disconnected
                if (IsServerConnected && serverBuild.Length < 1 || IsServerConnected == false)
                {
                    ServerDisConnected?.Invoke();
                }

                if (IsServerConnected && serverBuild.Length > 0)
                    server.GetStream().BeginRead(serverBuffer, 0, serverBuffer.Length, new AsyncCallback(GetServerData), server);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // reset settings.ini
            settings.RestoreFurcadiaSettings(BackupSettings);
            BackupSettings = null;
            FurcSettingsRestored?.Invoke();
        }

        #endregion Private Methods
    }
}