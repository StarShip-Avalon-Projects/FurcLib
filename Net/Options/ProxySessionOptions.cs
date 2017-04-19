namespace Furcadia.Net.Options
{
    /// <summary>
    /// </summary>
    public class ProxySessionOptions : ProxyOptions
    {
        #region Private Fields

        private int reconnectattempts;
        private int reconnectdelay;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// default Options
        /// </summary>
        public ProxySessionOptions() : base()
        {
            reconnectdelay = 45;
            reconnectattempts = 5;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// number of connection attempts to connect to the game server
        /// </summary>
        public int ReconnectAttempts { get { return reconnectattempts; } set { reconnectattempts = value; } }

        /// <summary>
        /// Reconnection Delay between attempts in seconds
        /// </summary>
        public int ReconnectTimeOutDelay
        {
            get { return reconnectdelay; }
            set { reconnectdelay = value; }
        }

        #endregion Public Properties
    }
}