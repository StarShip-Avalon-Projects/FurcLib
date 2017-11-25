/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (Oct 27,2009,0.1) Squizzle, Initial Developer.
 * (Unknown) Gerolkae, Switched proxy.ini to settings.ini firewall settings to support Vista+
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to wirk with a Supplied path
*/

using Furcadia.IO;
using Furcadia.Net.Options;
using System;
using System.IO;
using static Furcadia.Text.FurcadiaSettingsUtiliies;

namespace Furcadia.Text
{
    /// <summary>
    /// A simple way to load settings whether from ini or xml.
    /// </summary>
    public class Settings
    {
        #region Public Constructors

        private ProxyOptions options;

        private string[] SettingsBackup;

        /// <summary>
        ///
        /// </summary>
        public Settings()
        {
            options = new ProxyOptions();
            Keys = new string[] {
                "UseProxyOrFirewall",
                "ProxyHost",
                "ProxyPort",
                "SessionCloseCheck",
                "ProxyHostType",
                "ProxyCustomType",
                "ProxyCustomData",
                "ProxyApplyToFs",
                "UseTls"
            };
            values = new string[] {
                "Yes",
                options.ProxyHost,
                options.LocalhostPort.ToString(),
                "no",
                "0",
                "0",
                "CONNECT %host% %port%",
                "no",
                "no"
            };
            if (Keys.Length != values.Length)
                throw new ArgumentOutOfRangeException($"Keys.Length: {Keys.Length} != values.Length: {values.Length}");
        }

        /// <summary>
        /// </summary>
        /// <param name="options">Pxoxy Options</param>
        public Settings(ProxyOptions Options)
        {
            options = Options;
            Keys = new string[] {
                "UseProxyOrFirewall",
                "ProxyHost",
                "ProxyPort",
                "SessionCloseCheck",
                "ProxyHostType",
                "ProxyCustomType",
                "ProxyCustomData",
                "ProxyApplyToFs",
                "UseTls"
            };
            values = new string[] {
                "Yes",
                options.ProxyHost,
                options.LocalhostPort.ToString(),
                "no",
                "0",
                "0",
                "CONNECT %host% %port%",
                "no",
                "no"
            };
            if (Keys.Length != values.Length)
                throw new ArgumentOutOfRangeException($"Keys.Length: {Keys.Length} != values.Length: {values.Length}");
        }

        #endregion Public Constructors

        #region Private Fields

        /// <summary>
        /// Furcadia Settings file
        /// </summary>
        private const string sFile = "settings.ini";

        /// <summary>
        /// Furcadia Default file paths
        /// </summary>
        private static Paths FurcPath = new Paths();

        /// <summary>
        /// Proxy/Firewall Keys
        /// </summary>
        private readonly string[] Keys;

        /// <summary>
        /// Our Proxy/Firewall Values
        /// </summary>
        private readonly string[] values;

        /// <summary>
        /// Furcadia Settings path
        /// </summary>
        private string sPath;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Lets back up our Proxy/Firewall settings and then set the new
        /// settings for the Furcadia Client
        /// </summary>
        /// <param name="path">
        /// Furcadia Settings.ini path
        /// </param>
        /// <returns>
        /// Backup Settings for restoring later
        /// </returns>
        public void InitializeFurcadiaSettings(string path = null)
        {
            if (path == null)
                sPath = FurcPath.SettingsPath;
            else
                sPath = path;
            string[] FurcSettings = ReadSettingIni(Path.Combine(sPath, sFile));
            SettingsBackup = ReadSettingIni(Path.Combine(sPath, sFile));
            for (int Key = 0; Key < Keys.Length; Key++)
            {
                SetUserSetting(Keys[Key], values[Key], ref FurcSettings);
            }
            // Save settings.ini?
            SaveFurcadiaSettings(sPath, sFile, FurcSettings);
        }

        /// <summary>
        /// Restores the Furcadia Settings we backed up earlier.
        /// </summary>
        /// <param name="BackupSettings">
        /// Backed up settings array
        /// </param>
        public void RestoreFurcadiaSettings()
        {
            // Get the New Changes by Furcadia Suite
            string[] FurcSettings = ReadSettingIni(Path.Combine(sPath, sFile));

            for (int Key = 0; Key < Keys.Length; Key++)
            {
                // Capture Back up Fields
                string Value = GetUserSetting(Keys[Key], SettingsBackup);
                // Use Backup for Settings
                SetUserSetting(Keys[Key], Value, ref FurcSettings);
            }
            //Save settings.ini
            SaveFurcadiaSettings(sPath, sFile, FurcSettings);
        }

        #endregion Public Methods
    }
}