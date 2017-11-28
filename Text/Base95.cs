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
    /// </summary>
    public class Base95 : IComparable<uint>, IEquatable<uint>
    {
        #region Public Fields

        /// <summary>
        /// </summary>
        public const byte BASE = 95;

        /// <summary>
        /// </summary>
        public const byte CHAR_OFFSET = (byte)' ';

        /// <summary>
        /// </summary>
        [CLSCompliant(false)]
        public uint Value;

        #endregion Public Fields

        /*** Constructor ***/

        #region Public Constructors

        /// <summary>
        /// </summary>
        public Base95() : this(0)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="n">
        /// </param>
        [CLSCompliant(false)]
        public Base95(uint n)
        {
            Value = n;
        }

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        public Base95(string s)
        {
            FromString(s);
        }

        #endregion Public Constructors

        #region Conversion Operators

        /// <summary>
        /// </summary>
        /// <param name="b95n">
        /// </param>
        [CLSCompliant(false)]
        public static explicit operator ushort(Base95 b95n)
        {
            return (ushort)b95n.Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="n">
        /// </param>
        [CLSCompliant(false)]
        public static implicit operator Base95(uint n)
        {
            return new Base95(n);
        }

        /// <summary>
        /// </summary>
        /// <param name="n">
        /// </param>
        [CLSCompliant(false)]
        public static implicit operator Base95(ushort n)
        {
            return new Base95(n);
        }

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        [CLSCompliant(false)]
        public static implicit operator Base95(string s)
        {
            return new Base95(s);
        }

        /// <summary>
        /// </summary>
        /// <param name="b95n">
        /// </param>
        [CLSCompliant(false)]
        public static implicit operator byte[] (Base95 b95n)
        {
            return b95n.ToByteArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="b95n">
        /// </param>
        [CLSCompliant(false)]
        public static implicit operator string(Base95 b95n)
        {
            return b95n.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="b95n">
        /// </param>
        [CLSCompliant(false)]
        public static implicit operator uint(Base95 b95n)
        {
            return b95n.Value;
        }

        #endregion Conversion Operators

        #region Other Operators

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        [CLSCompliant(false)]
        public static Base95 operator -(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value - n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        [CLSCompliant(false)]
        public static bool operator !=(Base95 n1, Base95 n2)
        {
            return !(n1 == n2);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        [CLSCompliant(false)]
        public static Base95 operator %(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value % n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static Base95 operator *(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value * n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static Base95 operator /(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value / n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static Base95 operator +(Base95 n1, Base95 n2)
        {
            return new Base95(n1.Value + n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator <(Base95 n1, Base95 n2)
        {
            return n1.Value < n2.Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator ==(Base95 n1, Base95 n2)
        {
            return n1.Equals(n2);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator >(Base95 n1, Base95 n2)
        {
            return n1.Value > n2.Value;
        }

        #endregion Other Operators

        /*** Static Functions ***/

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="b95str">
        /// </param>
        /// <returns>
        /// </returns>
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
        /// </summary>
        /// <param name="num">
        /// </param>
        /// <returns>
        /// </returns>
        [CLSCompliant(false)]
        public static string ConvertToBase95(uint num)
        {
            return ConvertToBase95(num, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="num">
        /// </param>
        /// <param name="nDigits">
        /// </param>
        /// <returns>
        /// </returns>
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

        /*** Methods ***/

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Base95))
                return false;
            return this.Value == ((Base95)obj).Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        /// <returns>
        /// </returns>
        [CLSCompliant(false)]
        public uint FromString(string s)
        {
            return Value = ConvertFromBase95(s);
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

        public byte[] ToByteArray()
        {
            return ToByteArray(0);
        }

        /// <summary>
        /// </summary>
        /// <param name="nDigits">
        /// </param>
        /// <returns>
        /// </returns>
        public byte[] ToByteArray(int nDigits)
        {
            System.Text.ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetBytes(ToString(nDigits));
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return ConvertToBase95(Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="nDigits">
        /// </param>
        /// <returns>
        /// </returns>
        public string ToString(int nDigits)
        {
            return ConvertToBase95(Value, nDigits);
        }

        #endregion Public Methods

        #region Interface Implementation

        /// <summary>
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// </returns>
        [CLSCompliant(false)]
        public int CompareTo(uint other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// </returns>
        [CLSCompliant(false)]
        public bool Equals(uint other)
        {
            return Value.Equals(other);
        }

        #endregion Interface Implementation
    }
}