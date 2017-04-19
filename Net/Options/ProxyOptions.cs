namespace Furcadia.Net.Options
{
    public class ProxyOptions : ClientOptions
    {
        #region Private Fields

        private string characterini;
        private int localhostport;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Deault settingds
        /// </summary>
        public ProxyOptions() : base()
        {
            localhostport = 6700;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Character Ini file to connect to the Game server with
        /// </summary>
        public string CharacterIniFile
        {
            get { return characterini; }
            set { characterini = value; }
        }

        /// <summary>
        /// Localhost TCP port
        /// </summary>
        public int LocalhostPort
        {
            get { return LocalhostPort; }
            set { localhostport = value; }
        }

        #endregion Public Properties
    }
}