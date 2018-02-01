using System;
using System.IO;
using static Furcadia.Net.Utils.Utilities;

namespace Furcadia.Net.Options
{
    /// <summary>
    /// Configuration options for NetProxy
    /// </summary>
    public class ProxyOptions : ClientOptions
    {
        #region Private Fields

        private string characterini;
        private string furcinstallpath;

        /// <summary>
        /// Furcadia Client Executable
        /// </summary>
        private string furcprocess;

        private int connectionTimeOut;
        private int connectionRetries;

        /// <summary>
        /// LocalHost port
        /// </summary>
        private int localhostport;

        private string proxyHost;

        #endregion Private Fields

        private bool standalone;

        #region Public Constructors

        /// <summary>
        /// Default settings
        /// <para>
        /// <see cref="localhostport"/> = 6700
        /// </para>
        /// <para>
        /// <see cref="furcprocess"/> = "Furcadia.exe"
        /// </para>
        /// </summary>
        public ProxyOptions() : base()
        {
            connectionTimeOut = 10;
            connectionRetries = 5;
            proxyHost = "localhost";
            LocalhostPort = 6700;
            furcprocess = FurcadiaUtilities.DefaultClient;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Allow the connection to stay open after the client drops?
        /// </summary>
        ///<remarks>
        ///if standalone is enabled.. then we can skip, Furcadia Client Login
        ///with Firewall/Proxy settings and manage the client triggers ourselves
        /// </remarks>
        public bool Standalone
        {
            get { return standalone; }
            set { standalone = value; }
        }

        private int resetSettingTime = 20;

        /// <summary>
        /// time delay to reset furcadia settings when
        /// the furcadia client is launched.
        /// </summary>
        /// <value>
        /// The reset time in seconds
        /// <para/>
        /// Defaults to 20 seconds;
        /// </value>
        public int ResetSettingTime
        {
            get { return resetSettingTime; }
            set { resetSettingTime = value; }
        }

        /// <summary>
        /// Character Ini file to connect to the Game server with
        /// </summary>
        /// <remarks>
        /// <para>
        /// Furcadia.Exe Command Line options
        /// </para>
        /// <para>
        /// If only a Character.ini is Specified then NetProxy will Connect
        /// with Legacy Connection.
        /// </para>
        /// <para>
        /// If NoArguments specified then we'll login with Account Login window
        /// </para>
        /// <para>
        /// If we use -url="" We can Open the client and the server will
        /// select the character for us and bypass the Account Login Screen
        /// and the Game News feed
        /// </para>
        /// </remarks>
        public string CharacterIniFile
        {
            get { return characterini; }
            set { characterini = value; }
        }

        /// <summary>
        /// Host name or IP Address for the proxy server
        /// <para/>
        /// Defaults to "localhost'
        /// </summary>
        public string ProxyHost
        {
            get { return proxyHost; }
            set { proxyHost = value; }
        }

        /// <summary>
        /// Gets or sets the connection time out.
        /// </summary>
        /// <value>
        /// Time out in seconds
        /// <para/>
        /// Default: 10 seconds
        /// </value>
        public int ConnectionTimeOut
        {
            get { return connectionTimeOut; }
            set { connectionTimeOut = value; }
        }

        /// <summary>
        /// Gets or sets the connection retries.
        /// </summary>
        /// <value>
        /// Number of reconnection attempts
        /// <para/>
        /// Default: 5 tries
        /// </value>
        public int ConnectionRetries
        {
            get { return connectionRetries; }
            set { connectionRetries = value; }
        }

        /// <summary>
        /// Furcadia working folder path to the Client install we want to use
        /// </summary>
        public string FurcadiaInstallPath
        {
            get
            {
                if (string.IsNullOrEmpty(furcinstallpath))
                {
                    furcinstallpath = FurcadiaFilePaths.GetFurcadiaInstallPath();
                }
                return furcinstallpath;
            }
            set
            {
                if (!Directory.Exists(value))
                    throw new NetProxyException("Process path not found.");
                if (!File.Exists(Path.Combine(value, "Furcadia.exe")))
                    throw new NetProxyException($"Client executable '{Path.Combine(value, "Furcadia.exe")}' not found.");
                FurcadiaFilePaths = new IO.Paths(value);
                furcinstallpath = FurcadiaFilePaths.GetFurcadiaInstallPath();
            }
        }

        /// <summary>
        /// Furcadia Client executable
        /// </summary>
        public string FurcadiaProcess
        {
            get { return furcprocess; }
            set { furcprocess = value; }
        }

        /// <summary>
        /// Localhost TCP port
        /// </summary>
        public int LocalhostPort
        {
            get { return localhostport; }
            set
            {
                //if (!PortOpen(value))
                //    throw new ArgumentException($"The Selected Prot {value} is not available");
                localhostport = value;
            }
        }

        #endregion Public Properties
    }
}