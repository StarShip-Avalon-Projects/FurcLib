namespace Furcadia.Text
{
    /// <summary>
    /// Stuff for Furcadia Markup Language (FML)
    /// </summary>
    public class FurcadiaMarkup
    {
        #region Public Fields

        /// <summary>
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
        public const string DiceFilter = "^<font color='roll'><img src='fsh://system.fsh:101' alt='@roll' /><channel name='@roll' /> <name shortname='([^ ]+)'>([^ ]+)</name> rolls (\\d+)d(\\d+)((-|\\+)\\d+)? ?(.*) & gets (\\d+)\\.</font>$";

        /// <summary>
        /// </summary>
        public const string EntryFilter = "^<font color='([^']*?)'>(.*?)</font>$";

        /// <summary>
        /// </summary>
        public const string Iconfilter = "<img src='fsh://system.fsh:([^']*)'(.*?)/>";

        /// <summary>
        /// </summary>
        public const string NameFilter = "<name shortname='([^']*)' ?(.*?)?>([\\x21-\\x3B\\=\\x3F-\\x7E]+)</name>";

        /// <summary>
        /// Whispers Name
        /// </summary>
        public const string RegExName = "< name shortname='(.*?)' src='whisper-(.*?)'>";

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