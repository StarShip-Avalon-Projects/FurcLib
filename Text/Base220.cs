using Furcadia.Net.Utils;
using System;
using System.Text;

namespace Furcadia.Text
{
    /// <summary>
    /// Furcadia Base220 Encoding
    /// <para/>
    /// Author: Artex (aka, 1337)
    /// <para/>
    /// Modified by: Gerolkae
    /// </summary>
    /// <remarks>
    /// Reference http://dev.furcadia.com/docs/base220.pdf
    /// </remarks>
    public class Base220 : IComparable<int>, IEquatable<int>
    {
        #region Private Fields

        private const byte BASE = 220;
        private const byte CHAR_OFFSET = (byte)'#';
        private int Value;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Base220"/> class.
        /// </summary>
        public Base220() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Base220"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        public Base220(int n)
        {
            Value = n;
        }

        /// <summary>
        /// </summary>
        /// <param name="Base220String">
        /// </param>
        public Base220(string Base220String)
        {
            FromString(Base220String);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Process Base220 Strings.
        /// <para>
        /// these are string Prefixed with a Base220 character representing
        /// the Lengeth of the string
        /// </para>
        /// </summary>
        /// <param name="b220str">
        /// </param>
        /// <returns>
        /// </returns>
        /// <remarks>
        /// Reference Base 220 Strings http://dev.furcadia.com/docs/base220.pdf
        /// </remarks>
        public static int Base220StringLengeth(ref string b220str)
        {
            int Length = ConvertFromBase220(b220str[0].ToString());
            b220str = b220str.Substring(1, Length);
            return Length;
        }

        /// <summary>
        /// Converts from base220.
        /// </summary>
        /// <param name="b220str">The B220STR.</param>
        /// <returns></returns>
        public static int ConvertFromBase220(string b220str)
        {
            int num = 0;
            int mod = 1;
            if (string.IsNullOrEmpty(b220str))
                return 0;
            // Conversion
            for (int i = 0; i < b220str.Length; i++)
            {
                num += (b220str[i] - CHAR_OFFSET) * mod;
                mod *= 220;
            }

            return num;
        }

        /// <summary>
        /// Converts from base220.
        /// </summary>
        /// <param name="b220chr">The B220CHR.</param>
        /// <returns></returns>
        public static int ConvertFromBase220(char b220chr)
        {
            int num = 0;
            int mod = 1;
            if (b220chr == '\0')
                return 0;
            // Conversion
            for (int i = 0; i < 1; i++)
            {
                num += (b220chr - CHAR_OFFSET) * mod;
                mod *= 220;
            }

            return num;
        }

        /// <summary>
        /// Converts to base220.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        public static string ConvertToBase220(int num)
        {
            return ConvertToBase220(num, 0);
        }

        /// <summary>
        /// Converts to base220.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <param name="nDigits">The n digits.</param>
        /// <returns></returns>
        public static string ConvertToBase220(int num, int nDigits)
        {
            StringBuilder b220str = new StringBuilder();
            int ch;

            // Conversion
            while (num > 0)
            {
                ch = (num % 220) + CHAR_OFFSET; num /= 220;
                b220str.Append((char)ch);
            }

            // Applying padding if necessary.
            if (nDigits > 0)
            {
                if (b220str.Length < nDigits)
                    return b220str.Append(new String((char)CHAR_OFFSET, nDigits - b220str.Length)).ToString();
                if (b220str.Length > nDigits)
                    return new String((char)(CHAR_OFFSET + BASE - 1), nDigits);
            }

            return b220str.ToString();
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Base220"/> to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="b220n">The B220N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator short(Base220 b220n)
        {
            return (short)b220n.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="Base220"/>.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Base220(int n)
        {
            return new Base220(n);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int16"/> to <see cref="Base220"/>.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Base220(short n)
        {
            return new Base220(n);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Base220"/>.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Base220(string s)
        {
            return new Base220(s);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Base220"/> to <see cref="System.Byte[]"/>.
        /// </summary>
        /// <param name="b220n">The B220N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator byte[] (Base220 b220n)
        {
            return b220n.ToByteArray();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Base220"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="b220n">The B220N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator int(Base220 b220n)
        {
            return b220n.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Base220"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="b220n">The B220N.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(Base220 b220n)
        {
            return b220n.ToString();
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base220 operator -(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value - n2.Value);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Base220 n1, Base220 n2)
        {
            return !(n1 == n2);
        }

        /// <summary>
        /// Implements the operator %.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base220 operator %(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value % n2.Value);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base220 operator *(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value * n2.Value);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base220 operator /(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value / n2.Value);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Base220 operator +(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value + n2.Value);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="n1">The n1.</param>
        /// <param name="n2">The n2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(Base220 n1, Base220 n2)
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
        public static bool operator ==(Base220 n1, Base220 n2)
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
        public static bool operator >(Base220 n1, Base220 n2)
        {
            return n1.Value > n2.Value;
        }

        /*** Static Functions ***/

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(int other)
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
            if (!(obj is Base220))
                return false;
            return this.Value == ((Base220)obj).Value;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(int other)
        {
            return Value.Equals(other);
        }

        /// <summary>
        /// Froms the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public int FromString(string s)
        {
            return Value = ConvertFromBase220(s);
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
            // System.Text.Encoding.GetEncoding(EncoderPage).GetBytes

            return System.Text.Encoding.GetEncoding(Utilities.GetEncoding).GetBytes(ToString(nDigits));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ConvertToBase220(Value);
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
            return ConvertToBase220(Value, nDigits);
        }

        #endregion Public Methods
    }
}