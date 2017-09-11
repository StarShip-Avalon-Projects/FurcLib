namespace Furcadia.Net.Options
{
    /// <summary>
    /// Pounce Server Configureation settings
    /// </summary>
    internal class PouncrOptions
    {
        #region Protected Internal Fields

        /// <summary>
        /// Furcadia Utilities with Library Default settings <paa></paa>
        /// </summary>
        protected internal readonly Utils.Utilities FurcadiaUtilities;

        #endregion Protected Internal Fields

        #region Private Fields

        private string serverhost;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Default Pounce Settings constructor
        /// <para>
        /// <see cref="ServerHost">= "on.furcadia.com/q"</see>
        /// </para>
        /// </summary>
        public PouncrOptions()
        {
            FurcadiaUtilities = new Utils.Utilities();
            serverhost = FurcadiaUtilities.PounceServerHost;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Pounce Server address
        /// </summary>
        public string ServerHost
        {
            get { return serverhost; }
            set { serverhost = value; }
        }

        #endregion Public Properties
    }
}