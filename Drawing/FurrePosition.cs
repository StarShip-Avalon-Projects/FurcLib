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
        /// Takes a B220 encoded string representing the x,y coordinates and convert them to Furcadia (X,Y) coordinates
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
        /// Initializes a new instance of the <see cref="FurrePosition"/> class.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        public FurrePosition(int X, int Y)
        {
            this.Y = Y;
            this.X = X;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurrePosition"/> class.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        public FurrePosition(double X, double Y)
        {
            this.Y = (int)Y;
            this.X = (int)X;
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
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is FurrePosition ob)
            {
                return ob.Y == Y && ob.X == X;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (int)x ^ (int)y;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
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