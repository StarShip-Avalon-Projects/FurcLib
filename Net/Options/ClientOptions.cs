namespace Furcadia.Net.Options
{
    /// <summary>
    /// Game server connection settings collection
    /// </summary>
    public class ClientOptions
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

        /// <summary>
        /// Host Name or IP address
        /// </summary>
        private string gameserverhost;

        /// <summary>
        /// Game Server TCP port
        /// </summary>
        private int gameserverport;

        private int connectionRetries;
        private int connectionTimeOut;

        #endregion Private Fields

        #region Private Properties

        /// <summary>
        /// Host name or IP of the game server
        /// </summary>
        public string GameServerHost
        {
            get { return gameserverhost; }
            set { gameserverhost = value; }
        }

        /// <summary>
        /// Game server TCP Port
        /// </summary>
        public int GameServerPort
        {
            get { return gameserverport; }
            set { gameserverport = value; }
        }

        #endregion Private Properties

        #region Public Constructors

        /// <summary>
        /// </summary>
        public ClientOptions()
        {
            FurcadiaUtilities = new Utils.Utilities();
            FurcadiaFilePaths = new IO.Paths();
            gameserverport = int.Parse(FurcadiaUtilities.GameServerPort);
            gameserverhost = FurcadiaUtilities.GameServerHost;
            ConnectionTimeOut = 10;
            ConnectionRetries = 5;
        }

        /// <summary>
        /// </summary>
        protected ClientOptions(string host, int port)
        {
            FurcadiaUtilities = new Utils.Utilities();
            FurcadiaFilePaths = new IO.Paths();
            gameserverport = port; // TODO: Settings Prefered Serve Port
            gameserverhost = host;
            ConnectionTimeOut = 10;
            ConnectionRetries = 5;
        }

        #endregion Public Constructors

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
    }
}