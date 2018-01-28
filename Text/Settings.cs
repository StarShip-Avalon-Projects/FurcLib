/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (Oct 27,2009,0.1) Squizzle, Initial Developer.
 * (Unknown) Gerolkae, Switched proxy.ini to settings.ini firewall settings to support Vista+
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to wirk with a Supplied path
*/

using Furcadia.IO;
using Furcadia.Logging;
using Furcadia.Net.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Furcadia.Text
{
    /// <summary>
    /// A simple way to load settings whether from ini or xml.
    /// </summary>
    public class Settings
    {
        #region Private Feilds

        private object RestoreLock = new object();
        private static object ReadSettings = new object();

        private object ReadSettingLock = new object();

        private static object setUserSettings = new object();
        private ProxyOptions options;

        /// <summary>
        /// RegEx for Setting.ini Key=Value pairs
        /// </summary>
        private static Regex regexkey = new Regex("^\\s*([^=\\s]*)[^=]*=(.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));

        private List<string> currentSettings;

        /// <summary>
        /// Furcadia Settings file
        /// </summary>
        private const string settingsFile = "settings.ini";

        /// <summary>
        /// Furcadia Default file paths
        /// </summary>
        private static Paths FurcPath = new Paths();

        /// <summary>
        /// Proxy/Firewall Keys
        /// </summary>
        private readonly List<string> Keys;

        /// <summary>
        /// Our Proxy/Firewall Values
        /// </summary>
        private readonly List<string> values;

        /// <summary>
        /// Furcadia Settings path
        /// </summary>
        private string sPath;

        #endregion Private Feilds

        #region Public Constructors

        /// <summary>
        ///
        /// </summary>
        public Settings()
        {
            //PreventRrated = No
            //WarnWhenAccessing = Yes
            Initialize();
            options = new ProxyOptions();
            Keys = new List<string> {
                "UseProxyOrFirewall",
                "ProxyHost",
                "ProxyPort",
                "SessionCloseCheck",
                "ProxyHostType",
                "ProxyCustomType",
                "ProxyCustomData",
                "ProxyApplyToFs",
                "UseTls",
                "PreventRrated",
                "WarnWhenAccessing"
            };
            values = new List<string> {
                "Yes",
                options.ProxyHost,
                options.LocalhostPort.ToString(),
                "no",
                "0",
                "0",
                "CONNECT %host% %port%",
                "no",
                "no",
                "no",
                "no"
            };
            if (Keys.Count != values.Count)
                throw new ArgumentOutOfRangeException($"Keys.Length: {Keys.Count} != values.Length: {values.Count}");
        }

        /// <summary>
        /// </summary>
        /// <param name="Options">Pxoxy Options</param>
        public Settings(ProxyOptions Options)
        {
            Initialize();
            options = Options;
            Keys = new List<string> {
                "UseProxyOrFirewall",
                "ProxyHost",
                "ProxyPort",
                "SessionCloseCheck",
                "ProxyHostType",
                "ProxyCustomType",
                "ProxyCustomData",
                "ProxyApplyToFs",
                "UseTls",
                "PreventRrated",
                "WarnWhenAccessing"
            };
            values = new List<string> {
                "Yes",
                options.ProxyHost,
                options.LocalhostPort.ToString(),
                "no",
                "0",
                "0",
                "CONNECT %host% %port%",
                "no",
                "no",
                "no",
                "no"
            };
            if (Keys.Count != values.Count)
                throw new ArgumentOutOfRangeException($"Keys.Length: {Keys.Count} != values.Length: {values.Count}");
        }

        /// <summary>
        /// Initializes fields for this instance.
        /// </summary>
        private void Initialize()
        {
#if DEBUG
            if (!Debugger.IsAttached)
                Logger.Disable<Settings>();
#else
            Logger.Disable<Settings>();
#endif
            currentSettings = new List<string>();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Initializes the furcadia settings asynchronous.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public async Task InitializeFurcadiaSettingsAsync(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                sPath = FurcPath.SettingsPath;
            else
                sPath = path;
            var set = ReadSettingsIni(Path.Combine(sPath, settingsFile));
            currentSettings.AddRange(set);
            for (int Key = 0; Key < Keys.Count; Key++)
            {
                SetUserSetting(Keys[Key], values[Key], ref set);
            }
            // Save settings.ini?
            await Task.Run(() => SaveFurcadiaSettings(sPath, settingsFile, set));
        }

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
            Logging.Logger.Debug<Settings>($"Set Furcadia Settings '{path}'");
            if (string.IsNullOrWhiteSpace(path))
                sPath = FurcPath.SettingsPath;
            else
                sPath = path;
            var set = ReadSettingsIni(Path.Combine(sPath, settingsFile));
            currentSettings.AddRange(set);
            for (int Key = 0; Key < Keys.Count; Key++)
            {
                SetUserSetting(Keys[Key], values[Key], ref set);
            }
            // Save settings.ini?
            SaveFurcadiaSettings(sPath, settingsFile, set);
        }

        /// <summary>
        /// Restores the Furcadia Settings we backed up earlier.
        /// </summary>
        /// <param name="BackupSettings">
        /// Backed up settings array
        /// </param>
        public void RestoreFurcadiaSettings()
        {
            Logging.Logger.Debug<Settings>("Restore Furcadia Settings");
            // Get the New Changes by Furcadia Suite
            var set = ReadSettingsIni(Path.Combine(sPath, settingsFile));
            lock (RestoreLock)
                for (int Key = 0; Key < Keys.Count; Key++)
                {
                    // Capture Back up Fields
                    string Value = GetUserSetting(Keys[Key]);
                    // Use Backup for Settings
                    SetUserSetting(Keys[Key], Value, ref set);
                }
            //Save settings.ini
            SaveFurcadiaSettings(sPath, settingsFile, set);
        }

        /// <summary>
        /// Restores the Furcadia Settings we backed up earlier.
        /// </summary>
        /// <param name="BackupSettings">
        /// Backed up settings array
        /// </param>
        public async Task RestoreFurcadiaSettingsAsync()
        {
            await Task.Delay(5000);
            Logging.Logger.Debug<Settings>("Restore Furcadia Settings");
            // Get the New Changes by Furcadia Suite
            List<string> FurcSettings = ReadSettingsIni(Path.Combine(sPath, settingsFile));

            for (int Key = 0; Key < Keys.Count; Key++)
            {
                // Capture Back up Fields
                string Value = GetUserSetting(Keys[Key]);
                // Use Backup for Settings
                SetUserSetting(Keys[Key], Value, ref FurcSettings);
            }
            //Save settings.ini

            await Task.Run(() => SaveFurcadiaSettings(sPath, settingsFile, FurcSettings));
        }

        /// <summary>
        /// Load Current Furcadia Settings
        /// <para/>
        /// No need to set the Settings, We just want to read them
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="file">
        /// </param>
        /// <returns>
        /// </returns>
        internal bool LoadFurcadiaSettings(string path, string file = settingsFile)
        {
            var ReturnValue = false;
            Logging.Logger.Debug<Settings>($"path: '{path}' file: '{file}'");
            // long FileIn, WiDx;
            List<string> SettingFile = new List<string>();
            try
            {
                currentSettings.AddRange(ReadSettingsIni(Path.Combine(path, file)));
                ReturnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception(" Couldn't Load Furcadia settings (" + file + ") to change.", ex);
            }
            return ReturnValue;
        }

        /// <summary>
        /// Save the furcadia configuration to settings.ini
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="file">
        /// </param>
        /// <param name="SettingFile">
        /// </param>
        public static void SaveFurcadiaSettings(string path, string file, List<String> SettingFile)
        {
            Logging.Logger.Debug<Settings>($"path: '{path}' file: '{file}' SettingFile {SettingFile}");
            try
            {
                File.WriteAllLines(Path.Combine(path, file), SettingFile, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't Save Furcadia settings (" + file + ") to change.", e);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// sets feilds in the FurcSettings array
        /// </summary>
        /// <param name="Setting">
        /// </param>
        /// <param name="Value">
        /// </param>
        /// <param name="SettingFile">
        /// </param>
        private void SetUserSetting(string Setting, string Value, ref List<string> SettingFile)
        {
            Logging.Logger.Debug<Settings>($"WhichSetting: '{Setting}' WhichValue '{Value}' SettingFile: '{SettingFile}'");
            lock (setUserSettings)
                for (int i = 1; i < SettingFile.Count; i++)
                {
                    Match SettingMatch = regexkey.Match(SettingFile[i]);
                    if (regexkey.Match(SettingFile[i]).Success && SettingMatch.Groups[1].Value == Setting)
                    {
                        SettingFile[i] = $"{Setting} = {Value}";
                        return;
                    }
                }
            throw new ArgumentException($"Couldn't find setting {Setting} to change.");
        }

        /// <summary>
        /// Retrieves a field setting in the FurcSettings array
        /// </summary>
        /// <param name="Setting">The setting to retrieve</param>
        /// <param name="SettingFile">The setting file.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Couldn't find Furcadia setting(" + WhichSetting + ") to change.</exception>
        internal string GetUserSetting(string Setting)
        {
            lock (ReadSettingLock)
            {
                Logging.Logger.Debug<Settings>(Setting);
                for (var i = 0; i < currentSettings.Count; i++)
                {
                    Match SettingMatch = regexkey.Match(currentSettings[i]);
                    if (regexkey.Match(currentSettings[i]).Success && SettingMatch.Groups[1].Value == Setting)
                    {
                        return SettingMatch.Groups[2].Value.Trim();
                    }
                }
            }
            throw new ArgumentException("Couldn't find Furcadia setting(" + Setting + ") to change.");
        }

        /// <summary>
        /// Read Furcadia settings from Furcadia install path.
        /// If the settings does not exist We'll use our
        /// Embedded version from source
        /// </summary>
        /// <param name="SettingsIni">Full file path to settings.ini</param>
        /// <returns>Array of settings</returns>
        private static List<string> ReadSettingsIni(string SettingsIni)
        {
            Logging.Logger.Debug<Settings>(SettingsIni);
            List<string> lines = new List<string>();
            if (File.Exists(SettingsIni))
            {
                lines.AddRange(File.ReadAllLines(SettingsIni));
            }
            else
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Furcadia.Resources.settings.ini";
                lock (ReadSettings)
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                            lines.Add(reader.ReadLine());
                    }
            }
            return lines;
        }

        #endregion Private Methods
    }
}