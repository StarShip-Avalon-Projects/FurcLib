using System;
using static Furcadia.Text.Base220;

namespace Furcadia.Drawing
{
    /// <summary>
    /// Furcadia Isometric Corrdinates
    /// </summary>
    public class FurrePosition
    {
        private double x;
        private double y;

        /// <summary>
        /// Initializes a new instance of the <see cref="FurrePosition"/> class.
        /// </summary>
        public FurrePosition()
        {
        }

        /// <summary>
        /// Tak a B220 encoded string representing the x,y coordinates and convert them to Furcadia (X,Y) Coordinates
        /// </summary>
        /// <param name="b220Encoded">4 byte string</param>
        public FurrePosition(string b220Encoded)
        {
            if (b220Encoded.Length < 4)
                throw new ArgumentOutOfRangeException("Not enough bytes to process");
            X = ConvertFromBase220(b220Encoded.Substring(0, 2)) * 2;
            Y = ConvertFromBase220(b220Encoded.Substring(2, 2));
        }

        #region Public Constructors

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
            this.Y = Y;
            this.X = X;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurrePosition"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public FurrePosition(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// x coordinate
        /// </summary>
        public int X { get => (int)x; set => x = value; }

        /// <summary>
        /// y coordinate
        /// </summary>
        public int Y { get => (int)y; set => y = value; }

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
            return $"({x}, {y})";
        }

        #endregion Public Methods

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(FurrePosition obj1, FurrePosition obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(FurrePosition obj1, FurrePosition obj2)
        {
            return !obj1.Equals(obj2);
        }
    }
}