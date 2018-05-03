using Furcadia.Text;
using System;
using System.Text;
using static Furcadia.Text.Base220;

namespace Furcadia.Drawing
{
    /// <summary>
    /// Furcadia Isometric Corrdinates
    /// </summary>
    public class FurrePosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurrePosition"/> class.
        /// </summary>
        public FurrePosition()
        {
        }

        /// <summary>
        /// Takes a B220 encoded string representing the x,y coordinates and convert them to Furcadia (X,Y) coordinates
        /// </summary>
        /// <param name="b220EncodedString">4 byte string</param>
        public FurrePosition(string b220EncodedString)
        {
            if (b220EncodedString.Length < 4)
                throw new ArgumentOutOfRangeException("b220EncodedString", b220EncodedString, "Not enough bytes to process");
            if (b220EncodedString.Length > 4)
                throw new ArgumentOutOfRangeException("b220EncodedString", b220EncodedString, "Too many bytes to process");
            X = (Base220)b220EncodedString.Substring(0, 2) * 2;
            Y = b220EncodedString.Substring(2, 2);
        }

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FurrePosition"/> class.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        public FurrePosition(Base220 X, Base220 Y)
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
        public Base220 X { get; set; }

        /// <summary>
        /// y coordinate
        /// </summary>
        public Base220 Y { get; set; }

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
            if (obj is null)
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
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{X}");
            sb.Append($"{Y}");
            return sb.ToString();
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