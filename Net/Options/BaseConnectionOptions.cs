namespace Furcadia.Net.Options
{
    /// <summary>
    /// Game server connection settings collection
    /// </summary>
    public class BaseConnectionOptions
    {
        #region Private Fields

        /// <summary>
        /// Furcadia Paths File Path Structure.
        /// </summary>
        protected internal IO.Paths FurcadiaFilePaths;

        /// <summary>
        /// Furcadia Utilities
        /// </summary>
        protected internal Utils.Utilities FurcadiaUtilities;

        private int connectionRetries;
        private int connectionTimeOut;

        #endregion Private Fields

        #region Private Properties

        /// <summary>
        /// Host name or IP of the game server
        /// </summary>
        public string GameServerHost { get; set; }

        /// <summary>
        /// Game server TCP Port
        /// </summary>
        public int GameServerPort { get; set; }

        #endregion Private Properties

        #region Public Constructors

        /// <summary>
        /// </summary>
        public BaseConnectionOptions()
        {
            FurcadiaUtilities = new Utils.Utilities();
            FurcadiaFilePaths = new IO.Paths();
            GameServerPort = int.Parse(FurcadiaUtilities.GameServerPort);
            GameServerHost = FurcadiaUtilities.GameServerHost;
            ConnectionTimeOut = 10;
            ConnectionRetries = 5;
        }

        /// <summary>
        /// </summary>
        protected BaseConnectionOptions(string host, int port)
        {
            FurcadiaUtilities = new Utils.Utilities();
            FurcadiaFilePaths = new IO.Paths();
            GameServerPort = port; // TODO: Settings Prefered Serve Port
            GameServerHost = host;
            ConnectionTimeOut = 10;
            ConnectionRetries = 5;
        }

        #endregion Public Constructors

        #region Public Properties

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
            get => connectionRetries;
            set => connectionRetries = value;
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
            get => connectionTimeOut;
            set => connectionTimeOut = value;
        }

        #endregion Public Properties
    }
}