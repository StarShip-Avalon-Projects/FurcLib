using Furcadia.Net.DreamInfo;
using Furcadia.Net.Utils.ServerObjects;
using System.Text.RegularExpressions;
using static Furcadia.Text.FurcadiaMarkup;

namespace Furcadia.Net.Utils.ServerParser
{
    /// <summary>
    /// Parse Dice rolls
    /// </summary>
    public class DiceRolls : ChannelObject
    {
        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public DiceRolls(string ServerInstruction) : base(ServerInstruction)
        {
            Dice = new DiceObject();
            //Dice Filter needs Player Name "forced"
            Regex DiceREGEX = new Regex(DiceFilter, RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match DiceMatch = DiceREGEX.Match(ServerInstruction);

            //Matches, in order:
            //1:      shortname()
            //2:      full(name)
            //3:      dice(count)
            //4:      sides()
            //5: +/-#
            //6: +/-  (component match)
            //7:      additional(Message)
            //8:      Final(result)

            player = new Furre(DiceMatch.Groups[3].Value)
            {
                Message = DiceMatch.Groups[7].Value
            };
            double.TryParse(DiceMatch.Groups[4].Value, out double num);
            Dice.DiceSides = num;
            num = 0;
            double.TryParse(DiceMatch.Groups[3].Value, out num);
            Dice.DiceCount = num;
            char.TryParse(DiceMatch.Groups[6].Value, out char cchar);
            Dice.DiceCompnentMatch = cchar;
            num = 0.0;
            double.TryParse(DiceMatch.Groups[5].Value, out num);
            Dice.DiceModifyer = num;
            num = 0;
            double.TryParse(DiceMatch.Groups[8].Value, out num);
            Dice.DiceSides = num;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the Dice Roll result from the game server
        /// </summary>
        public DiceObject Dice
        {
            get;
            set;
        }

        #endregion Public Properties
    }
}