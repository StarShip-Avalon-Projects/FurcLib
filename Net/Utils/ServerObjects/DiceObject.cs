namespace Furcadia.Net.Utils.ServerObjects
{
    /// <summary>
    /// Dice for the @roll channel
    /// </summary>
    public class DiceObject : DataObject
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
        /// Consgructor
        /// </summary>
        public DiceObject()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// + or - Modifyer
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
        /// Sum of the Dice Result
        /// </summary>
        public double DiceResult { get { return diceResult; } set { diceResult = value; } }

        /// <summary>
        /// Number of sides per Die
        /// </summary>
        public double DiceSides { get { return diceSides; } set { diceSides = value; } }

        #endregion Public Properties
    }
}