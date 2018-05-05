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
        #region Private Fields

        private Match FontColorRegexMatch;

        internal Furre player;

        private string channelText;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public ChannelObject(string ServerInstruction) : base(ServerInstruction)
        {
            instructionType = ServerInstructionType.DisplayText;
            FontColorRegexMatch = new Regex(FontChannelFilter, RegexOptions.Compiled | RegexOptions.CultureInvariant).Match(RawInstruction);
            channelText = ServerInstruction.ToStrippedFurcadiaMarkupString();
            player = new Furre();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelObject"/> class.
        /// </summary>
        /// <param name="ServerInstruction">The server instruction.</param>
        /// <param name="Furr">The furr.</param>
        public ChannelObject(string ServerInstruction, Furre Furr) : base(ServerInstruction)
        {
            instructionType = ServerInstructionType.DisplayText;
            FontColorRegexMatch = new Regex(FontChannelFilter, RegexOptions.Compiled | RegexOptions.CultureInvariant).Match(RawInstruction);
            channelText = ServerInstruction.ToStrippedFurcadiaMarkupString();
            player = Furr;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Raw unformatted channel text
        /// </summary>
        public string ChannelText
        {
            get => channelText;
        }

        /// <summary>
        /// Dynamic Channel filter
        /// </summary>
        public string DynamicChannel
        {
            get => FontColorRegexMatch.Groups[3].Value;
        }

        /// <summary>
        /// returns Clear Text to display in a log
        /// </summary>
        public string FormattedChannelText
        {
            get => ChannelText;
        }

        /// <summary>
        /// Active Triggering avatar
        /// </summary>
        public Furre Player
        {
            get => player;
        }

        #endregion Public Properties
    }
}