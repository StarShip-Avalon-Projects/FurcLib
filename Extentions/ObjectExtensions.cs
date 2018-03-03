using Furcadia.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furcadia.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Ases the int64.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="default">The default.</param>
        /// <returns></returns>
        public static long AsInt64(this object obj, long @default = -1L)
        {
            if (obj == null) return @default;
            try
            {
                return Convert.ToInt64(obj);
            }
            catch { return @default; }
        }

        /// <summary>
        /// Ases the int16.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="default">The default.</param>
        /// <returns></returns>
        public static short AsInt16(this object obj, short @default = -1)
        {
            if (obj == null) return @default;
            try
            {
                return Convert.ToInt16(obj);
            }
            catch { return @default; }
        }

        /// <summary>
        /// Ases the int32.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="default">The default.</param>
        /// <returns></returns>
        public static int AsInt32(this object obj, int @default = -1)
        {
            if (obj == null) return @default;
            try
            {
                return Convert.ToInt32(obj);
            }
            catch { return @default; }
        }

        /// <summary>
        /// Ases the double.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="default">The default.</param>
        /// <returns></returns>
        public static double AsDouble(this object obj, double @default = -1d)
        {
            if (obj == null) return @default;
            if (obj is double) return (double)obj;
            try
            {
                return Convert.ToDouble(obj);
            }
            catch { return @default; }
        }

        /// <summary>
        /// Ases the string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="default">The default.</param>
        /// <returns></returns>
        public static string AsString(this object obj, string @default = null)
        {
            if (obj == null) return @default;
            if (obj is string) return (string)obj;
            try
            {
                return Convert.ToString(obj);
            }
            catch { return @default; }
        }
    }
}