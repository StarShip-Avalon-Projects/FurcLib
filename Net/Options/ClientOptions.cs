namespace Furcadia.Net.Options
{
    /// <summary>
    /// Game server connection settings collection
    /// </summary>
    internal class ClientOptions
    {
        #region Private Fields

        /// <summary>
        /// Game Server TCP port
        /// </summary>
        private int gameserverport;

        /// <summary>
        /// Host Name or IP address
        /// </summary>
        private string hosts;

        #endregion Private Fields

        #region Public Constructors

        public ClientOptions()
        {
        }

        #endregion Public Constructors
    }
}