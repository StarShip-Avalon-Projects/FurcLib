﻿using System;
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
        /// You eat cookie filter
        /// </summary>
        public const string YouEatCookieFilter = "<img src='fsh://system.fsh:90' alt='@cookie' /><channel name = '@cookie' /> You eat a cookie.(.*?)";

        /// <summary>
        /// The desc filter
        /// </summary>
        public const string DescFilter = "<desc shortname='([^']*)' />(.*)";

        /// <summary>
        /// The dice filter
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
        public const string FontChannelFilter = "^<font color=[\'|\\\"](.*?)['|\\\"]>(" + Iconfilter + ")?" + "(" + ChannelNameFilter + ")?(.*?)</font>$";

        /// <summary>
        /// </summary>
        public const string Iconfilter = "<img src='(fsh://system.fsh:([^']*)|http://apollo.furcadia.com/cache/(.*?))' (alt='(.*?)' )?/>";

        /// <summary>
        /// </summary>
        public const string NameFilter = "<name shortname=('(.*?)'|\"(.*?)\") ?>(.*?)</name>";

        /// <summary>
        /// Whispers Name
        /// </summary>
        public const string YouWhisperRegex = "^<font color=('(whisper)'|\"(whisper)\")>\\[ ?You whisper \"(.*?)\" to ?<name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\") forced(=('forced')|(\"forced\")?) src='whisper-to'>(.*?)</name>\\. \\]</font>$";

        /// <summary>
        /// You shout filter
        /// </summary>
        public const string YouShoutFilter = "<font color=('(shout)'|\"(shout)\")>You shout, \"(.*?)\"</font>";

        /// <summary>
        ///
        /// </summary>
        public const string WhisperRegex = "^\\<font color=('(whisper)'|\"(whisper)\")\\>\\[ \\<name shortname=('[a-z0-9]{2,64}'|\"[a-z0-9]{2,64}\") src=('whisper-from'|\"whisper-from\")\\>(.{2,64})\\</name\\> whispers, \"(?<msg>.+)\" to you\\. \\]\\</font\\>$";

        /// <summary>
        /// The shout regex filter
        /// </summary>
        public const string ShoutRegexFilter = "<font color=('(shout)'|\"(shout)\")>\\{S\\} <name shortname=('(.*?)'|\"(.*?)\")>(.*?)</name>(.*)shouts: (.*?)</font>";

        /// <summary>
        ///
        /// </summary>
        public const string EmoteRegexFilter = "<font color='(.*?)'><name shortname=('(.*?)'|\"(.*?)\")>(.*?)</name>(.*?)</font>";

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

        /// <summary>
        /// Queries (join, summon, lead follow, cuddle)
        /// </summary>
        // <font color='query'><name shortname=['\"](.^?)['\"]>(.^?)</name> (.^?), <a href='command://(.^?)'>click here</a> or type `(.^?) and press &lt;enter&gt;.</font>
        public static Regex QueryCommand = new Regex("<font color='(.*?)'><name shortname='(.*?)'>(.*?)</name> ((requests permission to|asks you to) (.*?) (.*?) (.*?))", RegexOptions.None);

        /// <summary>
        /// Dream Urls
        /// </summary>
        public const string UrlRegex = @"furc://([0-9a-zA-Z /v]+)(:([0-9a-zA-Z /v]+))?/";

        /// <summary>
        /// Dream Url regex
        /// </summary>
        [CLSCompliant(false)]
        public static Regex URLRegex = new Regex(UrlRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The channel regex
        /// </summary>
        public static Regex ChannelRegex = new Regex("<font color=[\'|\\\"](.*?)['|\\\"]>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #region Public Constructors

        /// <summary>
        /// </summary>
        public FurcadiaMarkup()
        {
        }

        #endregion Public Constructors
    }
}