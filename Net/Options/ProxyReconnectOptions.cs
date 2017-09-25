using System;

namespace Furcadia.Net.Options
{
    /// <summary>
    /// </summary>
    public class ProxyReconnectOptions
    {
        #region Private Fields

        /// <summary>
        /// Max tries to reconnect to server before aborting
        /// </summary>
        private int reconnectmax;

        /// <summary>
        /// the time delay for the current connection attempt in seconds
        /// </summary>
        private TimeSpan reconnecttimeout;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public ProxyReconnectOptions()
        {
            ReconnectMax = 5;
            ConnectionTimeOut = 45;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the time delay for the current connection attempt in seconds.
        /// </summary>
        public int ConnectionTimeOut
        {
            get { return reconnecttimeout.Seconds * 2; }
            set { reconnecttimeout = TimeSpan.FromSeconds(value / 2); }
        }

        /// <summary>
        /// Maximum tries to reconnect to the server
        /// </summary>
        public int ReconnectMax
        {
            get { return reconnectmax; }
            set { reconnectmax = value; }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProxyReconnectOptions.TimeOutTimeSpan'

        public TimeSpan TimeOutTimeSpan
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProxyReconnectOptions.TimeOutTimeSpan'
        {
            get
            {
                return reconnecttimeout;
            }
        }

        #endregion Public Properties
    }
}