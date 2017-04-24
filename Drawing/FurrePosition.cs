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
            x = ConvertFromBase220(X);
            y = ConvertFromBase220(Y);
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
            y = y;
            x = x;
        }

        /// <summary>
        /// </summary>
        /// <param name="Position">
        /// </param>
        public FurrePosition(FurrePosition Position)
        {
            x = Position.x;
            y = Position.y;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// x coordiante
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// y corrdinate
        /// </summary>
        public int y { get; set; }

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
                return ob.y == y && ob.x == x;
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
            return string.Format("({0}, {1})", x.ToString(), y.ToString());
        }

        #endregion Public Methods
    }
}