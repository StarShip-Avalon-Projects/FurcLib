using Furcadia.Net.Dream;
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

        private string diceCompnentMatch;
        private double diceCount;
        private double diceModifyer;
        private double diceResult;
        private double diceSides;

        #endregion Private Fields

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
            DiceCompnentMatch = DiceMatch.Groups[6].Value;
            DiceModifyer = 0.0;
            double.TryParse(DiceMatch.Groups[5].Value, out diceModifyer);
            double.TryParse(DiceMatch.Groups[8].Value, out diceResult);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// </summary>
        public string DiceCompnentMatch { get { return diceCompnentMatch; } set { diceCompnentMatch = value; } }

        /// <summary>
        /// Number of Dice
        /// </summary>
        public double DiceCount { get { return diceCount; } set { diceCount = value; } }

        /// <summary>
        /// Die offset +/- n
        /// </summary>
        public double DiceModifyer { get { return diceModifyer; } set { diceModifyer = value; } }

        /// <summary>
        /// </summary>
        public double DiceResult { get { return diceResult; } set { diceResult = value; } }

        /// <summary>
        /// Number of sides per Die
        /// </summary>
        public double DiceSides { get { return diceSides; } set { diceSides = value; } }

        #endregion Public Properties
    }
}