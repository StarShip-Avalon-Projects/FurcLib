using Furcadia.Net.Dream;
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
        #region Private Fields

        private DiceObject dice;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public DiceRolls(string ServerInstruction) : base(ServerInstruction)
        {
            dice = new DiceObject();
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

            player = new FURRE((DiceMatch.Groups[3].Value));
            player.Message = DiceMatch.Groups[7].Value;
            double num = 0;
            double.TryParse(DiceMatch.Groups[4].Value, out num);
            dice.DiceSides = num;
            num = 0;
            double.TryParse(DiceMatch.Groups[3].Value, out num);
            dice.DiceCount = num;
            char cchar = '+';
            char.TryParse(DiceMatch.Groups[6].Value, out cchar);
            dice.DiceCompnentMatch = cchar;
            num = 0.0;
            double.TryParse(DiceMatch.Groups[5].Value, out num);
            dice.DiceModifyer = num;
            num = 0;
            double.TryParse(DiceMatch.Groups[8].Value, out num);
            dice.DiceSides = num;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// </summary>
        public DiceObject Dice
        {
            get
            {
                return dice;
            }
            set
            {
                dice = value;
            }
        }

        #endregion Public Properties
    }
}