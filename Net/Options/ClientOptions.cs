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
        protected ClientOptions()
        {
            FurcadiaUtilities = new Utils.Utilities();
            FurcadiaFilePaths = new IO.Paths();
            gameserverport = 6500; // TODO: Settings Prefered Serve Port
            gameserverhost = FurcadiaUtilities.GameServerHost;
        }

        #endregion Public Constructors
    }
}