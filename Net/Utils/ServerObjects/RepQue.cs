namespace Furcadia.Net.Utils.ServerObjects
{
    //TODO Check Furcadoia Popup Windows
    /// <summary>
    /// </summary>
    public struct Rep
    {
        #region Private Fields

        private string iD;

        private int type;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// </summary>
        public string ID { get { return iD; } set { iD = value; } }

        /// <summary>
        /// </summary>
        public int Type { get { return type; } set { type = value; } }

        #endregion Public Properties
    }
}