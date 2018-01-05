﻿using Furcadia.Net.DreamInfo;
using System.Text;
using System.Text.RegularExpressions;

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

        private const string UrlRegex = @"furc://([a-z0-9]+)?:?([a-z0-9]+)/";
        private int type;
        private string dreamOwner;
        private string title;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        ///
        /// </summary>
        public DreamBookmark() : base()
        {
            base.instructionType = ServerInstructionType.BookmarkDream;
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
            type = int.Parse(ServerInstruction[2].ToString());
            var URLRegex = new Regex(UrlRegex, RegexOptions.None);
            var UrlMatch = URLRegex.Match(RawInstruction);

            dreamOwner = UrlMatch.Groups[1].Value;
            title = UrlMatch.Groups[2].Value;
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
                if (string.IsNullOrWhiteSpace(dreamOwner))
                    return $"furc://{title}/";
                return $"furc://{dreamOwner}:{title}/";
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
        /// Dream title
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return title; }
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
                    return $"{title}";
                return $"{dreamOwner}:{title}";
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
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"DreamOwner: '{DreamOwner}' Title: '{title}' DreamUrl: '{DreamUrl}'");
            return sb.ToString();
        }
    }
}