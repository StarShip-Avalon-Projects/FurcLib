﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Furcadia.IO
{
    /// <summary>
    /// Furcadia configuration class to backup/set firewall setting and restore them after we have
    /// connected to the game server
    /// <para>
    /// Author Gerolkae
    /// </para>
    /// <para>
    /// Courtesy to Dream Dancer for helping me with this
    /// </para>
    /// </summary>
    public class FurcIni
    {
        /// <summary>
        /// Retrieves a feild setting in the FurcSettings array
        /// </summary>
        /// <param name="file">
        /// </param>
        /// <returns>
        /// Value
        /// </returns>
        public static string GetUserSetting(string WhichSetting, string[] SettingFile)
        {
            Regex regexkey = new Regex("^\\s*([^=\\s]*)[^=]*=(.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));

            for (long WiDx = 0; WiDx < SettingFile.Length; WiDx++)
            {
                Match m = regexkey.Match(SettingFile[WiDx]);
                if (regexkey.Match(SettingFile[WiDx]).Success && m.Groups[1].Value == WhichSetting)
                {
                    return m.Groups[2].Value;
                }
            }
            throw new Exception("++ ERROR: Couldn't find " + WhichSetting + " to change.");
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
            /// long FileIn, WiDx;
            string[] SettingFile;
            try
            {
                SettingFile = File.ReadAllLines(Path.Combine(path, file));
                return SettingFile;
            }
            catch
            {
                throw new Exception("++ ERROR: Couldn't Load " + file + " to change.");
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
            /// long FileIn, WiDx;

            try
            {
                File.WriteAllLines(Path.Combine(path, file), SettingFile, Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new Exception("++ ERROR: Couldn't Load " + file + " to change.", e);
            }
        }

        /// <summary>
        /// sets feilds in the FurcSettings array
        /// </summary>
        /// <param name="file">
        /// </param>
        /// <returns>
        /// true on success.
        /// </returns>
        public static void SetUserSetting(string WhichSetting, string WhichValue, string[] SettingFile)
        {
            Regex regexkey = new Regex("^\\s*([^=\\s]*)[^=]*=(.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));

            for (int WiDx = 1; WiDx < SettingFile.Length; WiDx++)
            {
                Match m = regexkey.Match(SettingFile[WiDx]);
                if (regexkey.Match(SettingFile[WiDx]).Success && m.Groups[1].Value == WhichSetting)
                {
                    SettingFile[WiDx] = WhichSetting + " = " + WhichValue;
                    return;
                }
            }
            throw new Exception("++ ERROR: Couldn't find " + WhichSetting + " to change.");
        }
    }
}