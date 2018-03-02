/* Author: Artex (aka, 1337)
 * Base95 Format.
 * Uses: I have no idea.  Enjoy!
 * P.S: jk above...
*/

using System;
using System.Text;

namespace Furcadia.Text
{
    /// <summary>
    /// Base95 Encoding
    /// <para/>
    /// Author: Artex (aka, 1337)
    /// <para/>
    /// Modified by: Gerolkae
    /// </summary>
    /// </summary>
    /// <seealso cref="System.IComparable{System.UInt32}" />
    /// <seealso cref="System.IEquatable{System.UInt32}" />
    public class Base95 : IComparable<uint>, IEquatable<uint>
    {
        #region Public Fields

        /// <summary>
        /// The base
        /// </summary>
        public const byte BASE = 95;

        /// <summary>
        /// The character offset
        /// </summary>
        public const byte CHAR_OFFSET = (byte)' ';

        /// <summary>
        /// The value
        /// </summary>
        [CLSCompliant(false)]
        public uint Value;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Base95"/> class.
        /// </summary>
        public Base95() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Base95"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        [CLSCompliant(false)]
        public Base95(uint n)
        {
            Value = n;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Base95"/> class.
        /// </summary>
        /// <param name="s">The s.</param>
        public Base95(string s)
        {
            FromString(s);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Converts from base95.
        /// </summary>
        /// <param name="b95str">The B95STR.</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static uint ConvertFromBase95(string b95str)
        {
            uint num = 0;
            uint mod = 1;

            // Conversion
            for (int i = b95str.Length - 1; i >= 0; i--)
            {
                num += (uint)((b95str[i] - CHAR_OFFSET) * mod);
                mod *= 95;
            }

            return num;
        }

        /// <summary>
        /// Converts to base95.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string ConvertToBase95(uint num)
        {
            return ConvertToBase95(num, 0);
        }

        /// <summary>
        /// Converts to base95.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <param name="nDigits">The n digits.</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string ConvertToBase95(uint num, int nDigits)
        {
            StringBuilder b95str = new StringBuilder("");
            uint ch;

            // Conversion
            while (num > 0)
            {
                ch = (num % 95) + CHAR_OFFSET; num /= 95;
                b95str.Insert(0, (byte)ch);
            }

            // Applying padding if necessary.
            if (nDigits > 0)
            {
                if (b95str.Length < nDigits)
                    return b95str.Insert(0, new string((char)CHAR_OFFSET, nDigits - b95str.Length)).ToString();
                if (b95str.Length > nDigits)
                    return new string((char)(CHAR_OFFSET + BASE - 1), nDigits);
            }

            return b95str.ToString();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Base95"/> to <see cref="System.UInt16"/>.
        /// </summary>
        /// <param name="b95n">The B95N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [CLSCompliant(false)]
        public static explicit operator ushort(Base95 b95n)
        {
            return (ushort)b95n.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.UInt32"/> to <see cref="Base95"/>.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator Base95(uint n)
        {
            return new Base95(n);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.UInt16"/> to <see cref="Base95"/>.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator Base95(ushort n)
        {
            return new Base95(n);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Base95"/>.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator Base95(string s)
        {
            return new Base95(s);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Base95"/> to <see cref="System.Byte[]"/>.
        /// </summary>
        /// <param name="b95n">The B95N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator byte[] (Base95 b95n)
        {
            return b95n.ToByteArray();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Base95"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="b95n">The B95N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator string(Base95 b95n)
        {
            return b95n.ToString();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Base95"/> to <see cref="System.UInt32"/>.
        /// </summary>
        /// <param name="b95n">The B95N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator uint(Base95 b95n)
        {
            return b95n.Value;
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [CLSCompliant(false)]
        public static Base95 operator -(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value - n2.Value);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [CLSCompliant(false)]
        public static bool operator !=(Base95 n1, Base95 n2)
        {
            return n1 != n2;
        }

        /// <summary>
        /// Implements the operator %.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [CLSCompliant(false)]
        public static Base95 operator %(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value % n2.Value);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base95 operator *(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value * n2.Value);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base95 operator /(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value / n2.Value);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base95 operator +(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value + n2.Value);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(Base95 n1, Base95 n2)
        {
            return n1.Value < n2.Value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Base95 n1, Base95 n2)
        {
            return n1.Equals(n2);
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(Base95 n1, Base95 n2)
        {
            return n1.Value > n2.Value;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        [CLSCompliant(false)]
        public int CompareTo(uint other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Base95))
                return false;
            return this.Value == ((Base95)obj).Value;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        [CLSCompliant(false)]
        public bool Equals(uint other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Froms the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public uint FromString(string s)
        {
            return Value = ConvertFromBase95(s);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// To the byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            return ToByteArray(0);
        }

        /// <summary>
        /// To the byte array.
        /// </summary>
        /// <param name="nDigits">The n digits.</param>
        /// <returns></returns>
        public byte[] ToByteArray(int nDigits)
        {
            System.Text.ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetBytes(ToString(nDigits));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToBase95(Value);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="nDigits">The n digits.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(int nDigits)
        {
            return ConvertToBase95(Value, nDigits);
        }

        #endregion Public Methods
    }
}