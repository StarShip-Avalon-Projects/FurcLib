/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (?,2007) Kylix, Initial Coder and SimpleProxy project manager
 * (Oct 27,2009) Squizzle, Added NetMessage, delegates, and NetProxy wrapper class.
 * (July 26, 2011) Gerolkae, added setting.ini switch for proxy.ini
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path
 */

using Furcadia.Text;
using System;
using System.Diagnostics;
using System.IO;

//using System.ComponentModel;
using System.Net;
using System.Net.Security;
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
        #region Private Fields

        private SslStream ServerSslStream;

        #endregion Private Fields

        #region Protected Internal Fields

        /// <summary>
        /// Furcadia Utilities
        /// </summary>
        protected internal Utils.Utilities FurcadiaUtilities;

        #endregion Protected Internal Fields



        #region Private Fields

        /// <summary>
        /// FurcadiaSettings File
        /// </summary>
        private const string SetFile = "settings.ini";

        /// <summary>
        /// Max buffer size
        /// </summary>
        private static int BUFFER_CAP = 4096;

        private IPEndPoint _endpoint;

        private string[] BackupSettings;

        /// <summary>
        /// Furcadia Client Connection
        /// </summary>
        private TcpClient client = new TcpClient();

        //NetworkStream stream;

        private byte[] clientBuffer = new byte[BUFFER_CAP], serverBuffer = new byte[BUFFER_CAP];

        private object ClientBufferLock = new object();

        /// <summary>
        /// </summary>
        private int ClientnTaken = 0;

        /// <summary>
        /// </summary>
        private int ENCODE_PAGE = 1252;

        private Mutex FurcMutex;

        /// <summary>
        /// Furcadia Client Process
        /// </summary>
        private Process furcProcess;

        /// <summary>
        /// Allow Furcadia Client to connect to us
        /// </summary>
        private TcpListener listen;

        private System.Threading.Timer NewsTimer;
        private Options.ProxyOptions options;

        /// <summary>
        /// Process IP for Furcadia.exe
        /// </summary>
        private int processID;

        /// <summary>
        /// </summary>
        private TcpClient server;

        private object ServerBufferLock = new object();
        private int ServerTaken = 0;

        /// <summary>
        /// Furcadia Settings File Path
        /// </summary>
        private string SetPath;

        /// <summary>
        /// Furcadia Settings for backup/restore
        /// </summary>
        private string[] sett;

        private Text.Settings settings;

        #endregion Private Fields

        #region Public Constructors

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
        /// </summary>
        public delegate void ErrorEventHandler(Exception e, Object o, String n);

        #endregion Public Delegates

        #region Public Events

        /// <summary>
        /// This is triggered when the Client sends data to the server.
        /// </summary>
        public virtual event DataEventHandler ClientData;

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
        public event ActionDelegate ServerDisConnected;

        #endregion Public Events

        #region Protected Internal Events

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
        /// This is triggered when t client is closed.
        /// </summary>
        protected internal event ActionDelegate FurcSettingsRestored;

        /// <summary>
        /// This is triggered when the Server sends data to the client.
        /// Expects a return Value
        /// </summary>
        protected internal virtual event DataEventHandler ServerData;

        #endregion Protected Internal Events

        #region Protected Events

        /// <summary>
        /// This is triggered when a handled Exception is thrown.
        /// </summary>
        protected event ErrorEventHandler Error;

        #endregion Protected Events

        #region Public Properties

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

        #endregion Public Properties

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
        public virtual void Connect()
        {
            //if (string.IsNullOrEmpty(options.CharacterIniFile))
            //    throw new Proxy.CharacterNotFoundException("Character.ini not specified");
            try
            {
                _endpoint = ConverHostToIP(FurcadiaUtilities.GameServerHost, options.GameServerPort);
                if (listen != null)
                {
                    listen.Start();
                    listen.BeginAcceptTcpClient(new AsyncCallback(AsyncListener), listen);
                }
                else
                {
                    bool isAvailable = false;
                    int counter = 0;
                    while (isAvailable == false)
                    {
                        isAvailable = PortOpen(options.LocalhostPort);
                        if (!isAvailable)
                            options.LocalhostPort++;
                        if (counter == 100)
                            break;
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
            //if (string.IsNullOrEmpty(message))
            //    return;
            if (!message.EndsWith(string.Format("{0}", '\n')))
                message += '\n';
            try
            {
                if (client.Client != null && client.GetStream().CanWrite == true && client.Connected == true)
                    client.GetStream().Write(System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message), 0, System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message).Length);
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
            //if (string.IsNullOrEmpty(message))
            //    return;
            if (!message.EndsWith(string.Format("{0}", '\n')))
                message += '\n';
            if (!IsServerConnected)
                return;
            try
            {
                if (server.GetStream().CanWrite)
                    server.GetStream().Write(System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message), 0, System.Text.Encoding.GetEncoding(GetEncoding).GetBytes(message).Length);
            }
            catch (Exception e)
            {
                Error?.Invoke(e, this, "SendServer");
                ServerDisConnected?.Invoke();
            }
        }

        #endregion Public Methods

        #region Protected Methods

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

        #endregion Protected Methods

        #region Private Methods

        private byte[] ClientLeftOvers = new byte[BUFFER_CAP];

        private int ClientLeftOversSize = 0;

        private byte[] ServerLeftOvers = new byte[BUFFER_CAP];

        private int ServerLeftOversSize = 0;

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
                server = new TcpClient();
                server.Connect(_endpoint);
                if (!server.Connected) throw new Exception("There was a problem connecting to the server.");

                client.GetStream().BeginRead(clientBuffer, 0, clientBuffer.Length, new AsyncCallback(GetClientData), client);
                server.GetStream().BeginRead(serverBuffer, 0, serverBuffer.Length, new AsyncCallback(GetServerData), server);

                Connected?.Invoke();

                // Trigger News timer to restore settings
                NewsTimer = new System.Threading.Timer(OnTimedEvent, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            }
            catch (Exception e) { Error?.Invoke(e, this, "AsyncListener()"); }
        }

        /// <summary>
        /// </summary>
        private void ClientDisconnected()
        {
            if (!options.Standalone)
                Disconnect();
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

        /// <summary>
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void GetClientData(IAsyncResult ar)
        {
            if (client.Connected == false)
            {
                return;
                // throw new SocketException((int)SocketError.NotConnected);
            }

            //read = number of bytes read
            int read = 0;

            read = client.GetStream().EndRead(ar);
            int currStart = 0;
            int currEnd = -1;

            for (int i = 0; i < read; i++)
            {
                if (i < BUFFER_CAP && clientBuffer[i] == 10)//'\n'
                {
                    // Set the end of the data
                    currEnd = i;

                    // If we have left overs from previous runs:
                    if (ClientLeftOversSize != 0) //&& (currEnd - currStart + 1) > 0)
                    {
                        // Allocate enough space for the joined buffer
                        byte[] joinedData = new byte[ClientLeftOversSize + (currEnd - currStart + 1)];

                        // And add the current read as well
                        Array.Copy(ClientLeftOvers, 0, joinedData, 0, ClientLeftOversSize);

                        // Get the leftover from the previous read
                        Array.Copy(clientBuffer, currStart, joinedData, ClientLeftOversSize, (currEnd - currStart + 1));

                        ClientData2?.Invoke(System.Text.Encoding.GetEncoding(GetEncoding).GetString(joinedData,
                       0, joinedData.Length)); //Mark that we don't have any
                                               // leftovers anymore

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
                //ClientLeftOvers = new byte[read - currStart];

                Array.Copy(clientBuffer, currStart, ClientLeftOvers, 0, read - currStart);
                ClientLeftOversSize = read - currStart;
            }

            if (IsClientConnected == false)
            {
                ClientDisConnected?.Invoke();
            }

            if (client.Connected)
                client.GetStream().BeginRead(clientBuffer, 0, BUFFER_CAP, new AsyncCallback(GetClientData), client);

            // }
        }

        /// <summary>
        /// </summary>
        /// <param name="ar">
        /// </param>
        private void GetServerData(IAsyncResult ar)
        {
            if (IsServerConnected == false)
            {
                return;
                //throw new SocketException((int)SocketError.NotConnected);
            }
            //read = number of bytes read
            int read = 0;

            read = server.GetStream().EndRead(ar);
            int currStart = 0;
            int currEnd = -1;

            for (int i = 0; i < read; i++)
            {
                if (i < BUFFER_CAP && serverBuffer[i] == 10)
                {
                    // Set the end of the data
                    currEnd = i;

                    // If we have left overs from previous runs:
                    if (ServerLeftOversSize != 0) //&& (currEnd - currStart + 1) > 0)
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
                        // between delimiter
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

            if (IsServerConnected == false)
            {
                ServerDisConnected?.Invoke();
            }

            if (server.Connected)
                server.GetStream().BeginRead(serverBuffer, 0, BUFFER_CAP, new AsyncCallback(GetServerData), server);
        }

        /// <summary>
        /// Reset Furcadia Settings when NewsTimer expires
        /// </summary>
        /// <param name="source">
        /// </param>
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