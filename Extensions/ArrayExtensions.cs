﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
///
/// </summary>
public static class ArrayExtensions
{/// <summary>
 ///
 /// </summary>
 /// <typeparam name="T"></typeparam>
 /// <param name="arr"></param>
 /// <param name="seperator"></param>
 /// <returns></returns>
    public static string ToString<T>(this T[] arr, char seperator = ',')
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i <= arr.Length - 1; i++)
        {
            sb.Append(arr[i]);
            if (i <= arr.Length - 1) sb.Append(seperator);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr">The arr.</param>
    /// <param name="seperator">The seperator.</param>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public static string ToString<T>(this T[] arr, string seperator = ", ")
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i <= arr.Length - 1; i++)
        {
            sb.Append(arr[i]);
            if (i <= arr.Length - 1) sb.Append(seperator);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="arr">The arr.</param>
    /// <param name="seperator">The seperator.</param>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public static string ToString<K, V>(this IDictionary<K, V> arr, char seperator = ',')
    {
        StringBuilder sb = new StringBuilder();
        var list = arr.ToList();
        for (int i = 0; i <= list.Count - 1; i++)
        {
            sb.Append(list[i].Key).Append('=').Append(list[i].Value);
            if (i != list.Count - 1) sb.Append(seperator);
        }

        return sb.ToString();
    }
}