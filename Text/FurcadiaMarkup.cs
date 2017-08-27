using System.Text.RegularExpressions;

namespace Furcadia.Text
{
    /// <summary>
    /// Furcadia Markup Language (FML) REGEX
    /// </summary>
    public class FurcadiaMarkup
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
        /// Format Text string
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
        public static bool SystemFshIcon(ref string serverData, string replaceText)
        {
            Regex IconRegex = new Regex(Iconfilter);
            Match IconMatch = IconRegex.Match(serverData);
            serverData = IconRegex.Replace(serverData, replaceText);

            return IconMatch.Success;
        }

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

        /// <summary>
        /// </summary>
        public const string DescFilter = "<desc shortname='([^']*)' />(.*)";

        /// <summary>
        /// </summary>
        public const string DiceFilter = "^<font color='roll'><img src='fsh://system.fsh:101' alt='@roll' />" + ChannelNameFilter + " <name shortname='([^ ]+)'>([^ ]+)</name> rolls (\\d+)d(\\d+)((-|\\+)\\d+)? ?(.*) & gets (\\d+)\\.</font>$";

        /// <summary>
        /// </summary>
        public const string EntryFilter = "^<font color='([^']*?)'>(.*?)</font>$";

        /// <summary>
        /// </summary>
        public const string Iconfilter = "<img src='fsh://system.fsh:([^']*)'(.*?)/>";

        /// <summary>
        /// HTML images filter
        /// </summary>
        public const string ImgTagFilter = "<img src='(.*?)' alt='?(.*?)?'? />";

        /// <summary>
        /// </summary>
        public const string NameFilter = "<name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\") ?>(.{2,64})\\</name\\>";

        /// <summary>
        /// Whispers Name
        /// </summary>
        public const string RegExName = "<name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\") forced(=('forced'|\"forced\"))? src=('whisper-to'|\"whisper-to\")\\>";

        /// <summary>
        /// Regex for working with HTML URLS
        /// </summary>
        public const string UrlFilter = "<a href='?\"?(.*?)'?\"?>(.*?)</a>";

        /// <summary>
        /// </summary>
        public const string YouSayFilter = "You ([\\x21-\\x3B\\=\\x3F-\\x7E]+), \"([^']*)\"";

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// </summary>
        public FurcadiaMarkup()
        {
        }

        #endregion Public Constructors
    }
}