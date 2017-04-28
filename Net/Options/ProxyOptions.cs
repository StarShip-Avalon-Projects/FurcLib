namespace Furcadia.Net.Options
{
    /// <summary>
    /// Configuration options for NetProxy
    /// </summary>
    public class ProxyOptions : ClientOptions
    {
        #region Private Fields

        private static IO.Paths FurcPath;
        private string characterini;

        /// <summary>
        /// Furcadia Client Executable
        /// </summary>
        private string furcprocess;

        /// <summary>
        /// LocalHost port
        /// </summary>
        private int localhostport;

        /// <summary>
        /// Furcadia File Paths
        /// </summary>
        public IO.Paths FurcadiaFilePaths
        {
            get { return FurcPath; }
            set { FurcPath = value; }
        }

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
        /// Deault settingds
        /// </summary>
        public ProxyOptions()
        {
            localhostport = 6700;
            characterini = "";
            furcprocess = @"Furcadia.exe";
            FurcPath = new IO.Paths();
        }

        #endregion Public Constructors

        #region Public Properties

        private string furcinstallpath;

        /// <summary>
        /// Character Ini file to connect to the Game server with
        /// </summary>
        public string CharacterIniFile
        {
            get { return characterini; }
            set { characterini = value; }
        }

        /// <summary>
        /// </summary>
        public string FurcadiaInstallPath
        {
            get
            {
                if (string.IsNullOrEmpty(furcinstallpath))
                {
                    FurcPath = new IO.Paths();
                    furcinstallpath = FurcPath.GetInstallPath();
                }
                return furcinstallpath;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    FurcPath = new IO.Paths();
                    furcinstallpath = FurcPath.GetInstallPath();
                }
                else
                {
                    FurcPath = new IO.Paths(value);
                    furcinstallpath = FurcPath.GetInstallPath();
                }
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