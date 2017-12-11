using Furcadia.Net.DreamInfo;

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
    public class DreamBookmark : BaseServerInstruction, IDream
    {
        #region Private Fields

        private int type;
        private string dreamURL;
        private string dreamOwner;

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
            type = ServerInstruction[2];
            if (ServerInstruction.Contains(":"))
            {
                string NameStr = ServerInstruction.Substring(0, ServerInstruction.IndexOf(":"));
                dreamOwner = NameStr;
            }
            else if (ServerInstruction.EndsWith("/") && !ServerInstruction.Contains(":"))
            {
                string NameStr = ServerInstruction.Substring(0, ServerInstruction.IndexOf("/"));
                dreamOwner = NameStr;
            }
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
        /// Gets the dream owner.
        /// </summary>
        /// <value>
        /// The dream owner.
        /// </value>
        public string DreamOwner
        {
            get
            {
                return dreamOwner;
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

        public bool IsModern => throw new System.NotImplementedException();

        public string Name
        {
            get
            {
                return dreamURL.Substring(3);
            }
            set => throw new System.NotImplementedException();
        }

        public string ShortName
        {
            get
            {
                return Name.ToFurcadiaShortName();
            }
        }

        #endregion Public Properties
    }
}