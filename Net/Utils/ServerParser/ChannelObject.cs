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

        #region Private Fields

        private string channel;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public ChannelObject(string ServerInstruction) : base(ServerInstruction)
        {
            if (ServerInstruction[0] == '(')
                instructionType = ServerInstructionType.DisplayText;
            Channel = Regex.Match(ServerInstruction, ChannelNameFilter).Groups[1].Value;
            channelText = Regex.Match(ServerInstruction, EntryFilter).Groups[2].Value;
            player = new FURRE();
        }

        #endregion Public Constructors

        #region Public Properties

        private string channelText;

        /// <summary>
        /// Channel Name
        /// </summary>
        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        /// <summary>
        /// Raw unformatted channel text
        /// </summary>
        public string ChannelText
        {
            get
            {
                return channelText;
            }
            set
            {
                channelText = value;
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
                return txt.Replace(channelText, string.Empty);
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