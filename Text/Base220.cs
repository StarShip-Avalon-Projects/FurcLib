using Furcadia.Net.Utils;
using System;
using System.Text;

namespace Furcadia.Text
{
    /// <summary>
    /// Furcadia Base220 Encoding
    /// <para>
    /// Author: Artex (aka, 1337)
    /// </para>
    /// <para>
    /// Modified by: Gerolkae
    /// </para>
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
        /// </summary>
        public Base220() : this(0)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="n">
        /// </param>
        public Base220(int n)
        {
            Value = n;
        }

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        public Base220(string s)
        {
            FromString(s);
        }

        #endregion Public Constructors

        #region Conversion Operators

        /// <summary>
        /// </summary>
        /// <param name="b220n">
        /// </param>
        public static explicit operator short(Base220 b220n)
        {
            return (short)b220n.Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="n">
        /// </param>
        public static implicit operator Base220(int n)
        {
            return new Base220(n);
        }

        /// <summary>
        /// </summary>
        /// <param name="n">
        /// </param>
        public static implicit operator Base220(short n)
        {
            return new Base220(n);
        }

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        public static implicit operator Base220(string s)
        {
            return new Base220(s);
        }

        /// <summary>
        /// </summary>
        /// <param name="b220n">
        /// </param>
        public static implicit operator byte[] (Base220 b220n)
        {
            return b220n.ToByteArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="b220n">
        /// </param>
        public static implicit operator int(Base220 b220n)
        {
            return b220n.Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="b220n">
        /// </param>
        public static implicit operator string(Base220 b220n)
        {
            return b220n.ToString();
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
        public static Base220 operator -(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value - n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator !=(Base220 n1, Base220 n2)
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
        public static Base220 operator %(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value % n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static Base220 operator *(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value * n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static Base220 operator /(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value / n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static Base220 operator +(Base220 n1, Base220 n2)
        {
            return new Base220(n1.Value + n2.Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="n1">
        /// </param>
        /// <param name="n2">
        /// </param>
        /// <returns>
        /// </returns>
        public static bool operator <(Base220 n1, Base220 n2)
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
        public static bool operator ==(Base220 n1, Base220 n2)
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
        public static bool operator >(Base220 n1, Base220 n2)
        {
            return n1.Value > n2.Value;
        }

        #endregion Other Operators

        /*** Static Functions ***/

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
        /// </summary>
        /// <param name="b220str">
        /// </param>
        /// <returns>
        /// </returns>
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
        /// </summary>
        /// <param name="b220chr">
        /// </param>
        /// <returns>
        /// </returns>
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
        /// </summary>
        /// <param name="num">
        /// </param>
        /// <returns>
        /// </returns>
        public static string ConvertToBase220(int num)
        {
            return ConvertToBase220(num, 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="num">
        /// </param>
        /// <param name="nDigits">
        /// </param>
        /// <returns>
        /// </returns>
        public static string ConvertToBase220(int num, int nDigits)
        {
            StringBuilder b220str = new StringBuilder("");
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
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Base220))
                return false;
            return this.Value == ((Base220)obj).Value;
        }

        /// <summary>
        /// </summary>
        /// <param name="s">
        /// </param>
        /// <returns>
        /// </returns>
        public int FromString(string s)
        {
            return Value = ConvertFromBase220(s);
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
            // System.Text.Encoding.GetEncoding(EncoderPage).GetBytes

            return System.Text.Encoding.GetEncoding(Utilities.GetEncoding).GetBytes(ToString(nDigits));
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return ConvertToBase220(Value);
        }

        /// <summary>
        /// </summary>
        /// <param name="nDigits">
        /// </param>
        /// <returns>
        /// </returns>
        public string ToString(int nDigits)
        {
            return ConvertToBase220(Value, nDigits);
        }

        #endregion Public Methods

        #region Interface Implementation

        /// <summary>
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// </returns>
        public int CompareTo(int other)
        {
            return Value.CompareTo(other);
        }

        /// <summary>
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Equals(int other)
        {
            return Value.Equals(other);
        }

        #endregion Interface Implementation
    }
}