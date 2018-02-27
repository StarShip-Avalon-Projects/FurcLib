namespace Furcadia.Net.Utils.ServerObjects
{
    /// <summary>
    /// Dice for the @roll channel
    /// </summary>
    public class DiceObject : DataObject
    {
        #region Private Fields

        private char diceCompnentMatch;

        private double diceCount;

        private double diceModifyer;

        private double diceResult;

        private double diceSides;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiceObject"/> class.
        /// </summary>
        public DiceObject()
        {
            diceCompnentMatch = '+';
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// + or - Modifyer
        /// </summary>
        public char DiceCompnentMatch
        {
            get => diceCompnentMatch;
            set => diceCompnentMatch = value;
        }

        /// <summary>
        /// Number of Dice
        /// </summary>
        public double DiceCount
        {
            get => diceCount;
            set => diceCount = value;
        }

        /// <summary>
        /// Die offset +/- n
        /// </summary>
        public double DiceModifyer
        {
            get => diceModifyer;
            set => diceModifyer = value;
        }

        /// <summary>
        /// Sum of the Dice Result
        /// </summary>
        public double DiceResult
        {
            get => diceResult;
            set => diceResult = value;
        }

        /// <summary>
        /// Number of sides per Die
        /// </summary>
        public double DiceSides
        {
            get => diceSides;
            set => diceSides = value;
        }

        #endregion Public Properties
    }
}