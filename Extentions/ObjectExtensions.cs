using System;

namespace Furcadia.Extensions
{
    public static class ObjectExtensions
    {
        public static T As<T>(this object obj)
        {
            if (obj == null) return default(T);
            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch { return default(T); }
        }
    }
}