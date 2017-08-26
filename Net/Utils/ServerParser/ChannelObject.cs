using Furcadia.Net.Dream;
using System.Text.RegularExpressions;
using static Furcadia.Text.FurcadiaMarkup;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Base Server Instruction object for Channel Processing
    /// </summary>
    public class ChannelObject : BaseServerInstruction
    {
        #region Internal Fields

        /// <summary>
        /// Active Triggering avatar
        /// </summary>
        internal FURRE player;

        #endregion Internal Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public ChannelObject(string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction[0] == '(')
                instructionType = ServerInstructionType.DisplayText;

            player = new FURRE();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Channel Name
        /// </summary>
        public string Channel
        {
            get { return Regex.Match(RawInstruction, ChannelNameFilter).Groups[1].Value; }
        }

        /// <summary>
        /// Raw unformatted channel text
        /// </summary>
        public string ChannelText
        {
            get
            {
                return Regex.Match(RawInstruction, EntryFilter).Groups[2].Value; ;
            }
        }

        /// <summary>
        /// returns Clear Text to display in a log
        /// </summary>
        public string FormattedChannelText
        {
            get
            {
                Regex txt = new Regex(ChannelNameFilter);
                return txt.Replace(ChannelText, "[$1]");
            }
        }

        /// <summary>
        /// Active Triggering avatar
        /// </summary>
        public FURRE Player
        {
            get { return player; }
            set { player = value; }
        }

        #endregion Public Properties
    }
}