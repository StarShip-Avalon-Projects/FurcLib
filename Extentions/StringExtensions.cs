using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
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

    public static string ToFurcadiaShortName(this string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        return Regex.Replace(name.ToLower(), "[^a-z0-9\0x0020_.;&\\|]+", string.Empty, RegexOptions.CultureInvariant | RegexOptions.Compiled).Replace("|", string.Empty);
    }

    public static string ToStrippedFurcadiaMarkupString(this string Text)
    {
        var r = new Regex("<(.*?)>");
        Text = r.Replace(Text, string.Empty);
        return Text.Replace("|", " ").ToLower();
    }
}