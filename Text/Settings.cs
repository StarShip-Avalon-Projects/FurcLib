/*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (Oct 27,2009,0.1) Squizzle, Initial Developer.
 * (Unknown) Gerolkae, Switched proxy.ini to settings.ini firewall settings to support Vista+
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to wirk with a Supplied path
*/

using Furcadia.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Furcadia.Text
{
    /// <summary>
    /// A simple way to load settings whether from ini or xml.
    /// </summary>
    public class Settings
    {
        #region Public Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Settings()
        {
            localport = 6700;
            localhost = "localhost";
            Keys = new string[9] { "UseProxyOrFirewall", "ProxyHost", "ProxyPort", "SessionCloseCheck", "ProxyHostType", "ProxyCustomType", "ProxyCustomData", "ProxyApplyToFs", "UseTls" };
            values = new string[9] { "Yes", localhost, localport.ToString(), "no", "0", "0", "CONNECT %host% %port%", "no", "no" };
        }

        /// <summary>
        /// </summary>
        /// <param name="Port">
        /// Local TCP Port
        /// </param>
        public Settings(int Port)
        {
            localport = Port;
            localhost = "localhost";
            Keys = new string[9] { "UseProxyOrFirewall", "ProxyHost", "ProxyPort", "SessionCloseCheck", "ProxyHostType", "ProxyCustomType", "ProxyCustomData", "ProxyApplyToFs", "UseTls" };
            values = new string[9] { "Yes", localhost, localport.ToString(), "no", "0", "0", "CONNECT %host% %port%", "no", "no" };
        }

        #endregion Public Constructors

        #region Public Properties

        private static string localhost;
        private static int localport;

        /// <summary>
        /// Localhost or local IP
        /// </summary>
        public string Localhost
        {
            get { return localhost; }
            set { localhost = value; }
        }

        /// <summary>
        /// Local port Furcadia Client connects to
        /// </summary>
        public int LocalPort
        {
            get { return localport; }
            set { localport = value; }
        }

        #endregion Public Properties

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
        /// Loads a xml file and returns a new instance of T. T must be XML Deserializable!
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="file">
        /// </param>
        /// <returns>
        /// Default of T (default(T)) on file not found. Else it returns a
        /// instance of T.
        /// </returns>
        public static T Load<T>(string file)
        {
            if (!File.Exists(file)) return default(T);
            try
            {
                object newO = null;
                using (StringReader reader = new StringReader(File.ReadAllText(file)))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    Console.WriteLine(reader.ToString());
                    newO = (T)xml.Deserialize(reader);
                    Console.WriteLine(newO);
                    if (newO == null) return default(T);
                }
                return (T)newO;
            }
            catch { return default(T); }
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
        /// Rrad Furcadia settings from Furcadia install path. If the settings does not exist We'll use our Embedded fresource
        /// </summary>
        /// <param name="SettingsIni"></param>
        /// <returns></returns>
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
        /// Lets back up our Proxy/Firewall settings and then set the new
        /// settings for the Furcadia Client
        /// </summary>
        /// <param name="path">
        /// Furcadia Settings.ini path
        /// </param>
        /// <returns>
        /// Backup Settings for restoring later
        /// </returns>
        public string[] InitializeFurcadiaSettings(string path = null)
        {
            if (path == null)
                sPath = FurcPath.SettingsPath;
            else
                sPath = path;
            string[] FurcSettings = FurcIni.LoadFurcadiaSettings(sPath, sFile);
            string[] Backup = FurcIni.LoadFurcadiaSettings(sPath, sFile);
            for (int Key = 0; Key < Keys.Length; Key++)
            {
                FurcIni.SetUserSetting(Keys[Key], values[Key], FurcSettings);
            }
            // Save settings.ini?
            FurcIni.SaveFurcadiaSettings(sPath, sFile, FurcSettings);
            return Backup;
        }

        /// <summary>
        /// Restores the Furcadia Settings we backed up earlier.
        /// </summary>
        /// <param name="BackupSettings">
        /// Backed up settings array
        /// </param>
        public void RestoreFurcadiaSettings(string[] BackupSettings)
        {
            // Get the New Changes by Furcadia Suite
            string[] FurcSettings = FurcIni.LoadFurcadiaSettings(sPath, sFile);

            for (int Key = 0; Key < Keys.Length; Key++)
            {
                // Capture Back up Fields
                string Value = FurcIni.GetUserSetting(Keys[Key], BackupSettings);
                // Use Backup for Settings
                FurcIni.SetUserSetting(Keys[Key], Value, FurcSettings);
            }
            //Save settings.ini
            FurcIni.SaveFurcadiaSettings(sPath, sFile, FurcSettings);
        }

        #endregion Public Methods
    }
}