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
        #region Private Fields

        private string channel;

        #endregion Private Fields

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
            get
            { return channel; }
            set
            { channel = value; }
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
        /// Dynamic Channel filter
        /// </summary>
        public string DynamicChannel
        {
            get { return Regex.Match(RawInstruction, ChannelNameFilter).Groups[1].Value; }
        }

        /// <summary>
        /// returns Clear Text to display in a log
        /// </summary>
        public string FormattedChannelText
        {
            get
            {
                string Text = ChannelText;
                SystemFshIcon(ref Text, "[$1]");
                return Text;
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