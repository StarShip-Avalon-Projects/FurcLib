using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Furcadia.Text
{
    /// <summary>
    /// Furcadia configuration class to backup/set Proxy/firewall settings
    /// and restore them after we have connected to the game server.
    /// <para>
    /// Author Gerolkae
    /// </para>
    /// <para>
    /// Courtesy to Dream Dancer for helping me with this.
    /// </para>
    /// </summary>
    public static class FurcadiaSettingsUtiliies
    {
        #region Private Fields

        /// <summary>
        /// RegEx for Setting.ini Key=Value pairs
        /// </summary>
        private static Regex regexkey = new Regex("^\\s*([^=\\s]*)[^=]*=(.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Retrieves a field setting in the FurcSettings array
        /// </summary>
        /// <param name="SettingFile">
        /// </param>
        /// <param name="WhichSetting">
        /// </param>
        /// <returns>
        /// </returns>
        public static string GetUserSetting(string WhichSetting, string[] SettingFile)
        {
            for (long WiDx = 0; WiDx < SettingFile.Length; WiDx++)
            {
                Match m = regexkey.Match(SettingFile[WiDx]);
                if (regexkey.Match(SettingFile[WiDx]).Success && m.Groups[1].Value == WhichSetting)
                {
                    return m.Groups[2].Value;
                }
            }
            throw new Exception("Couldn't find Furcadia setting(" + WhichSetting + ") to change.");
        }

        /// <summary>
        /// Backs up the current Furcadia Settings
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="file">
        /// </param>
        /// <returns>
        /// </returns>
        public static String[] LoadFurcadiaSettings(string path, string file)
        {
            // long FileIn, WiDx;
            string[] SettingFile;
            try
            {
                SettingFile = ReadSettingIni(Path.Combine(path, file));
                return SettingFile;
            }
            catch (Exception ex)
            {
                throw new Exception(" Couldn't Load Furcadia settings (" + file + ") to change.", ex);
            }
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
        public static void SaveFurcadiaSettings(string path, string file, string[] SettingFile)
        {
            try
            {
                File.WriteAllLines(Path.Combine(path, file), SettingFile, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new Exception("Couldn't Save Furcadia settings (" + file + ") to change.", e);
            }
        }

        /// <summary>
        /// Loads an ini file and returns a key/value pair of values. (Note:
        /// It reads Key=Value pairs only.) (Add: Also the ini must be
        /// proper, one key/value per line. No section garbage.)
        /// </summary>
        /// <param name="file">
        /// </param>
        /// <returns>
        /// A new Hashtable, or a empty Hashtable on file not found.
        /// </returns>
        public static Hashtable Load(string file)
        {
            Hashtable ret = new Hashtable();
            List<string> lines = new List<string>();
            lines.AddRange(ReadSettingIni(file));
            foreach (string line in lines)
            {
                //get key/value!
                string[] key_value = line.Split(new char[] { '=' }, 2);
                if (key_value.Length == 2) ret.Add(key_value[0], key_value[1]);
            }
            return ret;
        }

        /// <summary>
        /// Read Furcadia settings from Furcadia install path. If the settings does not exist We'll use our
        /// Embedded from source
        /// </summary>
        /// <param name="SettingsIni">Full file path to settings.ini</param>
        /// <returns>Array of settings</returns>
        public static string[] ReadSettingIni(string SettingsIni)
        {
            List<string> lines = new List<string>();
            if (File.Exists(SettingsIni))
            {
                lines.AddRange(File.ReadAllLines(SettingsIni));
            }
            else
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Furcadia.Resources.settings.ini";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                        lines.Add(reader.ReadLine());
                }
            }
            return lines.ToArray();
        }

        /// <summary>
        /// sets feilds in the FurcSettings array
        /// </summary>
        /// <param name="WhichSetting">
        /// </param>
        /// <param name="WhichValue">
        /// </param>
        /// <param name="SettingFile">
        /// </param>
        public static void SetUserSetting(string WhichSetting, string WhichValue, ref string[] SettingFile)
        {
            for (int WiDx = 1; WiDx < SettingFile.Length; WiDx++)
            {
                Match m = regexkey.Match(SettingFile[WiDx]);
                if (regexkey.Match(SettingFile[WiDx]).Success && m.Groups[1].Value == WhichSetting)
                {
                    SettingFile[WiDx] = WhichSetting + " = " + WhichValue;
                    return;
                }
            }
            throw new Exception("Couldn't find setting" + WhichSetting + " to change.");
        }

        #endregion Public Methods
    }
}