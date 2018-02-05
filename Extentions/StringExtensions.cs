using System.Text;
using System.Text.RegularExpressions;

/// <summary>
///
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Escapes for c sharp.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns></returns>
    public static string EscapeForCSharp(this string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
            switch (c)
            {
                case '\'':
                case '"':
                case '\\':
                    sb.Append(c.EscapeForCSharp());
                    break;

                default:
                    if (char.IsControl(c))
                        sb.Append(c.EscapeForCSharp());
                    else
                        sb.Append(c);
                    break;
            }
        return sb.ToString();
    }

    /// <summary>
    /// Escapes for c sharp.
    /// </summary>
    /// <param name="chr">The character.</param>
    /// <returns></returns>
    public static string EscapeForCSharp(this char chr)
    {
        switch (chr)
        {
            case '\'':
                return @"\'";

            case '"':
                return "\\\"";

            case '\\':
                return @"\\";

            case '\0':
                return @"\0";

            case '\a':
                return @"\a";

            case '\b':
                return @"\b";

            case '\f':
                return @"\f";

            case '\n':
                return @"\n";

            case '\r':
                return @"\r";

            case '\t':
                return @"\t";

            case '\v':
                return @"\v";

            default:
                if (char.IsControl(chr) || char.IsHighSurrogate(chr) || char.IsLowSurrogate(chr))
                    return @"\u" + ((int)chr).ToString("X4");
                else
                    return new string(chr, 1);
        }
    }

    /// <summary>
    /// Determines whether [is null or blank].
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>
    ///   <c>true</c> if [is null or blank] [the specified string]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrBlank(this string str)
    {
        return string.IsNullOrEmpty(str) || (str.Length == 1 && str[0] == ' ');
    }

    /// <summary>
    /// To the short name of the furcadia.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static string ToFurcadiaShortName(this string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        return Regex.Replace(name.ToLower(), "[^a-z0-9\0x0020_.;&\\\v]+", string.Empty, RegexOptions.CultureInvariant | RegexOptions.Compiled);
    }

    /// <summary>
    /// To the stripped furcadia markup string.
    /// </summary>
    /// <param name="Text">The text.</param>
    /// <returns></returns>
    public static string ToStrippedFurcadiaMarkupString(this string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return null;
        Regex r = new Regex("<(.*?)>", RegexOptions.Compiled);
        Text = r.Replace(Text, string.Empty);
        return Text.Replace("|", " ");
    }
}