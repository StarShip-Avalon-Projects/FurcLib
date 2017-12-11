using Furcadia.Net.DreamInfo;
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
        internal Furre player;

        internal Match FontColorRegexMatch;

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
            FontColorRegexMatch = new Regex(FontChannelFilter, RegexOptions.Compiled | RegexOptions.CultureInvariant).Match(RawInstruction);
            channelText = ServerInstruction.ToStrippedFurcadiaMarkupString();
            player = new Furre();
        }

        #endregion Public Constructors

        #region Public Properties

        private string channelText;

        /// <summary>
        /// Raw unformatted channel text
        /// </summary>
        public string ChannelText
        {
            //set
            //{ channelText = value; }
            get
            { return channelText; }
        }

        /// <summary>
        /// Dynamic Channel filter
        /// </summary>
        public string DynamicChannel
        {
            get
            {
                return FontColorRegexMatch.Groups[3].Value;
            }
        }

        /// <summary>
        /// returns Clear Text to display in a log
        /// </summary>
        public string FormattedChannelText
        {
            get
            {
                return ChannelText;
            }
        }

        /// <summary>
        /// Active Triggering avatar
        /// </summary>
        public Furre Player
        {
            get { return player; }
            set { player = value; }
        }

        #endregion Public Properties
    }
}