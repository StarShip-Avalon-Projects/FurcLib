namespace Furcadia.Net.Options
{
    /// <summary>
    /// </summary>
    public class ProxySessionOptions : ProxyOptions
    {
        #region Private Fields

        private ProxyReconnectOptions reconnectOptions;

        /// <summary>
        /// reconnection manager options
        /// </summary>
        public ProxyReconnectOptions ReconnectOptions
        {
            get
            {
                return reconnectOptions;
            }
            set
            {
                reconnectOptions = value;
            }
        }

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// default Options
        /// </summary>
        public ProxySessionOptions() : base()
        {
            ReconnectOptions = new ProxyReconnectOptions();
        }

        public ProxySessionOptions(ProxyReconnectOptions reconnectoptions)
        {
            ReconnectOptions = reconnectoptions;
        }

        #endregion Public Constructors
    }
}