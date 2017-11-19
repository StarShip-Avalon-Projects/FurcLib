using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// Furcadia String Extentions
/// </summary>
public static class StringExtentions
{
    /// <summary>
    /// Converts a name to Furcadia Shortname Format
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ToFurcadiaShortName(this string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        return Regex.Replace(
            name
            .ToLower(), "[^a-z0-9\0x0020_.;&\\|]+", string.Empty, RegexOptions.CultureInvariant | RegexOptions.Compiled)
            .Replace("|", string.Empty);
    }

    /// <summary>
    /// Strip the nasty furcadia markup from a string
    /// </summary>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static string ToStrippedFurcadiaMarkupString(this string Text)
    {
        var r = new Regex("<(.*?)>");
        Text = r.Replace(Text, string.Empty);
        return Text.Replace("|", " ").ToLower();
    }
}