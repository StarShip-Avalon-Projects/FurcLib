/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (?,2007) Kylix, Initial Coder and SimpleProxy project manager
 * (Oct 27,2009) Squizzle, Added NetMessage, delegates, and NetProxy wrapper class.
 * (July 26, 2011) Gerolkae, added setting.ini switch for proxy.ini
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path
 */

using Furcadia.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

//using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static Furcadia.Net.Utils.Utilities;

namespace Furcadia.Net
{
    // TODO: Redo FurcadiaSettings to a seperate Object with a Settings
    //       restore Event. add Mutex to Lock Furcadia Settings betweenn new
    // sessions connecting... Accunt Login will Bypass News Timer Update.
    // Arg -url=""

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
        #region Protected Internal Fields

        /// <summary>
        /// Furcadia Utilities
        /// </summary>
        protected internal Utils.Utilities FurcadiaUtilities;

        #endregion Protected Internal Fields

        #region Private Fields

        /// <summary>
        /// Furcadia Client Process
        /// </summary>
        private Process furcProcess;

        private Options.ProxyOptions options;
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

        /// <summary>
        /// FurcadiaSettings File
        /// </summary>
        private const string SetFile = "settings.ini";

        /// <summary>
        /// Max buffer size
        /// </summary>
        private static int BUFFER_CAP = 2048;

        private IPEndPoint _endpoint;
        private string _ServerLeftOvers;
        private string[] BackupSettings;

        /// <summary>
        /// Furcadia Client Connection
        /// </summary>
        private TcpClient client = new TcpClient();

        private byte[] clientBuffer = new byte[BUFFER_CAP], serverBuffer = new byte[BUFFER_CAP];

        private string clientBuild, serverBuild;

        /// <summary>
        /// </summary>
        private int ENCODE_PAGE = 1252;

        private Mutex FurcMutex;

        /// <summary>
        /// Allow Furcadia Client to connect to us
        /// </summary>
        private TcpListener listen;

        private System.Threading.Timer NewsTimer;

        /// <summary>
        /// Process IP for Furcadia.exe
        /// </summary>
        private int processID;

        /// <summary>
        /// </summary>
        private TcpClient server;

        /// <summary>
        /// Furcadia Settings File Path
        /// </summary>
        private string SetPath;

        /// <summary>
        /// Furcadia Settings for backup/restore
        /// </summary>
        private string[] sett;

        #endregion Private Declarations

        #region Constructors

        /// <summary>
        /// Connect to game servver with default settings
        /// </summary>
        public NetProxy()
        {
            FurcadiaUtilities = new Utils.Utilities();
            options = new Options.ProxyOptions();
            settings = new Text.Settings(options.LocalhostPort);
            SetPath = options.FurcadiaFilePaths.SettingsPath;
            sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
            options.GameServerPort = Convert.ToInt32(FurcIni.GetUserSetting("PreferredServerPort", sett));
            _endpoint = ConverHostToIP(FurcadiaUtilities.GameServerHost, options.GameServerPort);
        }

        /// <summary>
        /// </summary>
        /// <param name="port">
        /// </param>
        public NetProxy(int port)
        {
            FurcadiaUtilities = new Utils.Utilities();
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            settings = new Text.Settings(options.LocalhostPort);
            SetPath = options.FurcadiaFilePaths.SettingsPath;
            sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
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
            FurcadiaUtilities = new Utils.Utilities();
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            settings = new Text.Settings(options.LocalhostPort);
            SetPath = options.FurcadiaFilePaths.SettingsPath;
            sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
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
            FurcadiaUtilities = new Utils.Utilities();
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            settings = new Text.Settings(options.LocalhostPort);
            SetPath = options.FurcadiaFilePaths.SettingsPath;
            sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
            _endpoint = ConverHostToIP(options.GameServerHost, options.GameServerPort);
        }

        /// <summary>
        /// Connect to Furcadia with Proxy Options
        /// </summary>
        /// <param name="Options">
        /// </param>
        public NetProxy(Options.ProxyOptions Options)
        {
            FurcadiaUtilities = new Utils.Utilities();
            options = Options;
            settings = new Text.Settings(options.LocalhostPort);
            SetPath = options.FurcadiaFilePaths.SettingsPath;
            sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
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
            FurcadiaUtilities = new Utils.Utilities();
            options = new Options.ProxyOptions();
            options.LocalhostPort = port;
            settings = new Text.Settings(options.LocalhostPort);
            SetPath = options.FurcadiaFilePaths.SettingsPath;
            sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
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
            FurcadiaUtilities = new Utils.Utilities();
            options = new Options.ProxyOptions();
            options.LocalhostPort = lport;
            settings = new Text.Settings(options.LocalhostPort);
            SetPath = options.FurcadiaFilePaths.SettingsPath;
            sett = FurcIni.LoadFurcadiaSettings(SetPath, SetFile);
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
                    return new IPEndPoint(FurcadiaUtilities.GameServerIp, ServerPort);
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
        public int ProcessID
        {
            get { return processID; }
        }

        #endregion Properties

        #region Public  Properties

        /// <summary>
        /// </summary>
        public int BufferCapacity
        {
            get
            {
                return BUFFER_CAP;
            }
        }

        /// <summary>
        /// </summary>
        public int EncoderPage
        {
            get
            {
                return ENCODE_PAGE;
            }
        }

        #endregion Public  Properties

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
                    clientStream.Dispose();
                }

                client.Close();
            }
        }

        /// <summary>
        /// Disconnects the furcadia client and Closes the application
        /// </summary>
        public void CloseClient()
        {
            try

            {
                ClientDisconnect();
                if (furcProcess == null)
                    furcProcess = Process.GetProcessById(processID);
                if (furcProcess != null)
                {
                    furcProcess.Kill();
                    furcProcess.Dispose();
                }
            }
            catch (Exception e) { Error?.Invoke(e, this, "CloseClient()"); }
        }

        /// <summary>
        /// Connects to the Furcadia Server and starts the mini proxy.
        /// </summary>
        public void Connect()
        {
            //if (string.IsNullOrEmpty(options.CharacterIniFile))
            //    throw new Proxy.CharacterNotFoundException("Character.ini not specified");
            try
            {
                if (listen != null)
                {
                    listen.Start();
                    listen.BeginAcceptTcpClient(new AsyncCallback(AsyncListener), listen);
                }
                else
                {
                    bool isAvailable = PortOpen(options.LocalhostPort);

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
                BackupSettings = settings.InitializeFurcadiaSettings(options.FurcadiaFilePaths.SettingsPath);
                //Run

                //check ProcessPath is not a directory
                if (!Directory.Exists(options.FurcadiaInstallPath)) throw new NetProxyException("Process path not found.");
                if (!File.Exists(Path.Combine(options.FurcadiaInstallPath, options.FurcadiaProcess))) throw new NetProxyException("Client executable '" + options.FurcadiaProcess + "' not found.");
                furcProcess = new System.Diagnostics.Process(); //= System.Diagnostics.Process.Start(Process,ProcessCMD );
                furcProcess.EnableRaisingEvents = true;
                furcProcess.StartInfo.FileName = options.FurcadiaProcess;
                furcProcess.StartInfo.Arguments = options.CharacterIniFile;
                furcProcess.StartInfo.WorkingDirectory = options.FurcadiaInstallPath;
                furcProcess.Start();
                furcProcess.Exited += delegate
                {
                    ClientExited?.Invoke();
                };
                furcProcess.Exited += delegate
                {
                    ClientDisconnected();
                };
                processID = furcProcess.Id;
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
            if (string.IsNullOrEmpty(message))
                return;
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
            if (string.IsNullOrEmpty(message))
                return;
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
        /// Dispose all our Disposable objects
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (NewsTimer != null)
                    NewsTimer.Dispose();
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

        private void ClientDisconnected()
        {
            if (!options.Standalone)
                Disconnect();
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
                server = new TcpClient(FurcadiaUtilities.GameServerHost, _endpoint.Port);
                if (!server.Connected) throw new Exception("There was a problem connecting to the server.");

                client.GetStream().BeginRead(clientBuffer, 0, clientBuffer.Length, new AsyncCallback(GetClientData), client);
                server.GetStream().BeginRead(serverBuffer, 0, serverBuffer.Length, new AsyncCallback(GetServerData), server);

                if (Connected != null)
                {
                    Connected();
                    // Trigger News timer to restore settings

                    NewsTimer = new System.Threading.Timer(OnTimedEvent, null, TimeSpan.FromSeconds(10), TimeSpan.MaxValue);
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
            //lock (lck)
            //{
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
                    ClientData2?.Invoke(lines[i]);
                    // we want ServerConnected and Check for null data
                    // Application may intentionally return ClientData =
                    // null to Avoid "Throat Tired" Syndrome. Let
                    // Application handle packet routing.
                    if (IsServerConnected == true && ClientData != null)
                        SendToServer(ClientData?.Invoke(lines[i]));
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
                if (IsClientConnected == true && clientBuild.Length > 0)
                {
                    client.GetStream().BeginRead(clientBuffer, 0, clientBuffer.Length, new AsyncCallback(GetClientData), client);
                }
            }
            // }
        }

        private void GetServerData(IAsyncResult ar)
        {
            //lock (lck)
            //{
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
            //}
        }

        private void OnTimedEvent(object source)
        {
            // reset settings.ini
            settings.RestoreFurcadiaSettings(BackupSettings);
            BackupSettings = null;
            FurcSettingsRestored?.Invoke();
            if (NewsTimer != null)
                NewsTimer.Dispose();
        }

        #endregion Private Methods
    }
}