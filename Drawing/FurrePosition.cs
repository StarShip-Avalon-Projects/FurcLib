using static Furcadia.Text.Base220;

namespace Furcadia.Drawing
{
    /// <summary>
    /// Furcadia Isometric Corrdinates
    /// </summary>
    public class FurrePosition
    {
        #region Public Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FurrePosition()
        {
        }

        /// <summary>
        /// Furre Position using Base 220 Corrdinates
        /// </summary>
        /// <param name="X">
        /// Base 220 X Coordinate
        /// </param>
        /// <param name="Y">
        /// Base 220 Y coordinate
        /// </param>
        public FurrePosition(string X, string Y)
        {
            this.X = ConvertFromBase220(X);
            this.Y = ConvertFromBase220(Y);
        }

        /// <summary>
        /// Furre Position using integer Corrdinates
        /// </summary>
        /// <param name="X">
        /// Integer X Coordinate
        /// </param>
        /// <param name="Y">
        /// Integer Y Coordinate
        /// </param>
        public FurrePosition(int X, int Y)
        {
            this.Y = this.Y;
            this.X = this.X;
        }

        /// <summary>
        /// </summary>
        /// <param name="Position">
        /// </param>
        public FurrePosition(FurrePosition Position)
        {
            X = Position.X;
            Y = Position.Y;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// x coordinate
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// y coordinate
        /// </summary>
        public int Y { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() == typeof(FurrePosition))
            {
                FurrePosition ob = (FurrePosition)obj;
                return ob.Y == Y && ob.X == X;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return string.Format("({0}, {1})", X.ToString(), Y.ToString());
        }

        #endregion Public Methods
    }
}