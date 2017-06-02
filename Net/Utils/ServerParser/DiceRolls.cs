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
        #region Public Fields

        private double diceCount;

        private double diceModifyer;

        private double diceResult;

        private double diceSides;

        /// <summary>
        /// </summary>
        public DiceObject Dice
        {
            get
            {
                return dice;
            }
        }

        #region Private Fields

        private string diceCompnentMatch;

        #endregion Private Fields

        #region Private Fields

        private DiceObject dice;

        #endregion Private Fields

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        /// <param name="ServerInstruction">
        /// </param>
        public DiceRolls(string ServerInstruction) : base(ServerInstruction)
        {
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
            double.TryParse(DiceMatch.Groups[4].Value, out diceSides);
            double.TryParse(DiceMatch.Groups[3].Value, out diceCount);
            dice.DiceCompnentMatch = DiceMatch.Groups[6].Value;
            dice.DiceModifyer = 0.0;
            double.TryParse(DiceMatch.Groups[5].Value, out diceModifyer);
            double.TryParse(DiceMatch.Groups[8].Value, out diceResult);
        }

        #endregion Public Constructors
    }
}