using Furcadia.Logging;
using Furcadia.Net.DreamInfo;
using System;
using System.Text;
using System.Text.RegularExpressions;
using static Furcadia.Text.FurcadiaMarkup;

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

        private string dreamOwner;
        private string title;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Constructor with Dream Data definitions
        /// </summary>
        /// <param name="ServerInstruction">
        /// Raw server instruction from the game server
        /// </param>
        public DreamBookmark(string ServerInstruction) : base(ServerInstruction)
        {
            base.instructionType = ServerInstructionType.BookmarkDream;
            DreamType = int.Parse(ServerInstruction[2].ToString());

            var UrlMatch = URLRegex.Match(RawInstruction);

            if (string.IsNullOrWhiteSpace(UrlMatch.Groups[3].Value))
            {
                dreamOwner = UrlMatch.Groups[1].Value;
                title = null;
            }
            else
            {
                dreamOwner = UrlMatch.Groups[1].Value;
                title = UrlMatch.Groups[3].Value;
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
                var sb = new StringBuilder("furc://");
                sb.Append(dreamOwner.ToFurcadiaShortName());
                if (!string.IsNullOrWhiteSpace(title))
                    sb.Append($":{title.ToStrippedFurcadiaMarkupString()}");
                sb.Append("/");
                return sb.ToString();
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
            get => dreamOwner;
        }

        /// <summary>
        /// Dream title
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get => title;
        }

        /// <summary>
        /// Dream Type
        /// <para> Type 0 = Temporary</para>
        /// Type 1 = Regular
        /// Type -1 = undefined
        /// </summary>
        public int DreamType { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is modern.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is modern; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsModern => throw new System.NotImplementedException();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(dreamOwner))
                    throw new ArgumentException(dreamOwner);
                var sb = new StringBuilder();

                sb.Append(dreamOwner.ToFurcadiaShortName());
                if (!string.IsNullOrWhiteSpace(title))
                    sb.Append($":{title.ToFurcadiaShortName()}");
                return sb.ToString();
            }
            set => throw new System.NotImplementedException();
        }

        #endregion Public Properties

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"DreamOwner: '{DreamOwner}' Title: '{title}' DreamUrl: '{DreamUrl}'";
        }
    }
}