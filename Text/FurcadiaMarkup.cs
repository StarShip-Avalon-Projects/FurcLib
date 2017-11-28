using System.Text.RegularExpressions;

namespace Furcadia.Text
{
    /// <summary>
    /// Furcadia Markup Language (FML) REGEX
    /// </summary>
    public sealed class FurcadiaMarkup
    {
        #region Public Methods

        /// <summary>
        /// Format Channel Tags
        /// <para>
        /// &lt;channel name='@channelname' /&gt;
        /// </para>
        /// </summary>
        /// <param name="serverData">
        /// Raw server data string
        /// </param>
        /// <param name="replaceText">
        /// Reg ex supported text replacement
        /// </param>
        /// <returns>
        /// True on a successful match
        /// </returns>
        public static bool ChannelTag(ref string serverData, string replaceText)
        {
            Regex IconRegex = new Regex(ChannelNameFilter);
            Match IconMatch = IconRegex.Match(serverData);
            serverData = IconRegex.Replace(serverData, replaceText);

            return IconMatch.Success;
        }

        /// <summary>
        /// Strip a string from all Furcadia Markup
        /// </summary>
        /// <param name="Text">string to process</param>
        /// <returns></returns>

        #endregion Public Methods

        #region Public Fields

        /// <summary>
        /// Dynamic Channel tags
        /// <para>
        /// &lt;channel name='@channelName' / &gt;
        /// </para>
        /// </summary>
        public const string ChannelNameFilter = "<channel name='(.*?)' />";

        /// <summary>
        /// </summary>
        public const string CookieToMeREGEX = "<name shortname='(.*?)'>(.*?)</name> just gave you";

        public const string YouEatCookieFilter = "<img src='fsh://system.fsh:90' alt='@cookie' /><channel name = '@cookie' /> You eat a cookie.(.*?)";

        /// <summary>
        /// </summary>
        public const string DescFilter = "<desc shortname='([^']*)' />(.*)";

        /// <summary>
        /// </summary>
        public const string DiceFilter = "^<font color='roll'><img src='fsh://system.fsh:101' alt='@roll' />" + ChannelNameFilter + " <name shortname='([^ ]+)'>([^ ]+)</name> rolls (\\d+)d(\\d+)((-|\\+)\\d+)? ?(.*) & gets (\\d+)\\.</font>$";

        /// <summary>
        /// <para/>
        /// font = 1
        /// <para/>
        /// system.fhs =2
        /// <para/>
        /// system alt = 3
        /// <para/>
        /// Channel Name = 4
        /// <para/>
        /// Text = 5
        /// </summary>
        public const string FontChannelFilter = "^<font color='([^']*?)'>(" + Iconfilter + ")?" + "(" + ChannelNameFilter + ")?(.*?)</font>$";

        /// <summary>
        /// </summary>
        public const string Iconfilter = "<img src='(fsh://system.fsh:([^']*)|http://apollo.furcadia.com/cache/(.*?))' (alt='(.*?)' )?/>";

        /// <summary>
        /// </summary>
        public const string NameFilter = "<name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\") ?>(.{2,64})</name>";

        /// <summary>
        /// Whispers Name
        /// </summary>
        public const string YouWhisperRegex = "^<font color=('(whisper)'|\"(whisper)\")>\\[ ?You whisper \"(.*?)\" to ?<name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\") forced(=('forced')|(\"forced\")?) src='whisper-to'>(.*?)</name>\\. \\]</font>$";

        public const string YouShoutFilter = "<font color=('(shout)'|\"(shout)\")>You shout, \"(.*?)\"</font>";

        /// <summary>
        ///
        /// </summary>
        public const string WhisperRegex = "^\\<font color=('(whisper)'|\"(whisper)\")\\>\\[ \\<name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\") src=('whisper-from'|\"whisper-from\")\\>(.{2,64})\\</name\\> whispers, \"(?<msg>.+)\" to you\\. \\]\\</font\\>$";

        public const string ShoutRegexFilter = "<font color=('(shout)'|\"(shout)\")>\\{S\\} <name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\")>(.{2,64})</name>(.*)shouts: (.*?)</font>";

        /// <summary>
        ///
        /// </summary>
        public const string EmoteRegexFilter = "<font color='(.*?)'><name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\")>(.{2,64})</name> (.*?)</font>";

        /// <summary>
        /// Regex for working with HTML URLS
        /// </summary>
        public const string UrlFilter = "<a href='?\"?(.*?)'?\"?>(.*?)</a>";

        /// <summary>
        /// </summary>
        public const string YouSayFilter = "You ([\\x21-\\x3B\\=\\x3F-\\x7E]+), \"([^']*)\"";

        #endregion Public Fields

        /// <summary>
        /// Filter the Name Markup
        /// </summary>
        public static Regex NameRegex = new Regex(NameFilter, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Pesky Desc tags filter
        /// </summary>
        public static Regex DescTagRegex = new Regex(Iconfilter + "(.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        #region Public Constructors

        /// <summary>
        /// </summary>
        public FurcadiaMarkup()
        {
        }

        #endregion Public Constructors
    }
}