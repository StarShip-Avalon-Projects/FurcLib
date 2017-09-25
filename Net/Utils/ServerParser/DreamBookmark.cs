namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Triggered when the connection enters a new dream.
    /// <para>
    /// This instruction tells the client to download the specified dream
    /// data from the file server.
    /// </para>
    /// <para>
    /// Respond with client command when furcadia client is not available "vasecodegamma"
    /// </para>
    /// </summary>
    /// <remarks>
    ///]CBookmark Type[1]Dream URL[*]
    ///<para> Type 0 = temporary</para>
    /// Type 1 = Regular (per user requests)
    ///<para>DreamUrl = "furc://uploadername:dreamname/entrycode "</para>
    /// Credits FTR
    /// </remarks>
    public class DreamBookmark : BaseServerInstruction
    {
        #region Private Fields

        private int type;
        private string dreamURL;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///
        /// </summary>
        public DreamBookmark() : base()
        {
            base.instructionType = ServerInstructionType.BookmarkDream;
            dreamURL = null;
            type = -1;
        }

        /// <summary>
        /// Constructor with Dream Data definitions
        /// </summary>
        /// <param name="ServerInstruction">
        /// Raw server instruction from the game server
        /// </param>
        public DreamBookmark(string ServerInstruction) : base(ServerInstruction)
        {
            base.instructionType = ServerInstructionType.BookmarkDream;
            dreamURL = ServerInstruction.Substring(3);
            type = int.Parse(ServerInstruction.Substring(2, 1));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The Dreams URL
        /// </summary>
        public string DreamUrl
        {
            get
            {
                return dreamURL;
            }
        }

        /// <summary>
        /// Dream Type
        /// <para> Type 0 = Temporary</para>
        /// Type 1 = Regular
        /// Type -1 = undefined
        /// </summary>
        public int DreamType
        {
            get { return type; }
        }

        #endregion Public Properties
    }
}