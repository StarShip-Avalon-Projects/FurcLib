namespace Furcadia.Net.Options
{
    /// <summary>
    /// Configuration options for NetProxy
    /// </summary>
    public class ProxyOptions : ClientOptions
    {
        #region Private Fields

        private string characterini;

        /// <summary>
        /// Furcadia Client Executable
        /// </summary>
        private string furcprocess;

        /// <summary>
        /// LocalHost port
        /// </summary>
        private int localhostport;

        #endregion Private Fields

        private bool standalone;

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

        #region Public Constructors

        /// <summary>
        /// Deault settings
        /// <para>
        /// <see cref="localhostport"/> = 6700
        /// </para>
        /// <para>
        /// <see cref="furcprocess"/> = "Furcadia.exe"
        /// </para>
        /// </summary>
        public ProxyOptions() : base()
        {
            localhostport = 6700;
            furcprocess = FurcadiaUtilities.DefaultClient;
        }

        #endregion Public Constructors

        #region Public Properties

        private string furcinstallpath;

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
        /// Furcadia working folder path to the Client install we want to use
        /// </summary>
        public string FurcadiaInstallPath
        {
            get
            {
                if (string.IsNullOrEmpty(furcinstallpath))
                {
                    furcinstallpath = FurcadiaFilePaths.GetInstallPath();
                }
                return furcinstallpath;
            }
            set
            {
                FurcadiaFilePaths = new IO.Paths(value);
                furcinstallpath = FurcadiaFilePaths.GetInstallPath();
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
            set { localhostport = value; }
        }

        #endregion Public Properties
    }
}