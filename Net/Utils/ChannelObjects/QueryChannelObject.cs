using Furcadia.Net.DreamInfo;
using Furcadia.Net.Utils.ServerParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Furcadia.Text.FurcadiaMarkup;

namespace Furcadia.Net.Utils.ChannelObjects
{
    [Flags]
    public enum QueryType
    {
        /// <summary>
        /// Join your company
        /// </summary>
        join,

        /// <summary>
        /// Summon yo to ...
        /// </summary>
        summon,

        /// <summary>
        /// yead you too...
        /// </summary>
        lead,

        /// <summary>
        /// Follow you
        /// </summary>
        follow,

        /// <summary>
        /// cubble with you
        /// </summary>
        cuddle
    }

    /// <summary>
    /// Query Commands
    /// </summary>
    /// <seealso cref="Furcadia.Net.Utils.ServerParser.ChannelObject" />
    public class QueryChannelObject : ChannelObject
    {
        /// <summary>
        /// Gets the query command type.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public QueryType Query
        {
            get => qType;
        }

        private QueryType qType;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryChannelObject"/> class.
        /// </summary>
        /// <param name="ServerInstruction"></param>
        public QueryChannelObject(string ServerInstruction) : base(ServerInstruction)
        {
            Match QueryMatch = QueryCommand.Match(ServerInstruction);
            var name = NameRegex.Match(ServerInstruction).Groups[2].Value;
            player = new Furre(name);
            switch ($"{QueryMatch.Groups[6].Value} {QueryMatch.Groups[7].Value}")
            {
                case "join their":
                    qType = QueryType.summon;
                    break;

                case "join your":
                    qType = QueryType.join;
                    break;

                case "follow you.":
                    qType = QueryType.follow;
                    break;

                case "lead you.":
                    qType = QueryType.lead;
                    break;

                case "cuddle with":
                    qType = QueryType.cuddle;
                    break;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryChannelObject"/> class.
        /// </summary>
        /// <param name="ServerInstruction">The server instruction.</param>
        /// <param name="Furr">The furr.</param>
        public QueryChannelObject(string ServerInstruction, Furre Furr) : base(ServerInstruction, Furr)
        {
            Match QueryMatch = QueryCommand.Match(ServerInstruction);
            var name = NameRegex.Match(ServerInstruction).Groups[2].Value;
            player = Furr;
            switch (QueryMatch.Groups[6].Value)
            {
                case "join their":
                    qType = QueryType.summon;
                    break;

                case "join your":
                    qType = QueryType.join;
                    break;

                case "follow you":
                    qType = QueryType.follow;
                    break;

                case "lead you":
                    qType = QueryType.lead;
                    break;

                case "cuddle with":
                    qType = QueryType.cuddle;
                    break;
            }
        }
    }
}