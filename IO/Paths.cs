using Microsoft.Win32;
using System;
using System.IO;

namespace Furcadia.IO
{
    /// <summary>
    /// This class contains all the paths related to the users furcadia installation.
    ///<para>***NOTICE: DO NOT REMOVE***</para>
    ///<para> Credits go to Artex for helping me fix Path issues and contributing his code.</para>
    ///<para>***NOTICE: DO NOT REMOVE.***</para>
    ///<para>Log Header</para>
    ///<para>Format: (date,Version) AuthorName, Changes.</para>
    ///<para> (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path</para>
    ///<para>  (June 1, 2016) Gerolkae, Added possible missing Registry Paths for x86/x64 Windows and Mono Support. Wine Support also contains these corrections.</para>
    ///<remarks>
    ///  Theory check all known default paths
    ///<para> check localdir.ini</para>
    ///<para>  then check each registry hives for active CPU type</para><
    ///<para>  Then check each give for default 32bit path(Non wow6432node)</para>
    ///<para>  then check Wine variants(C++ Win32 client)</para>
    ///<para>  then check Mono Versions for before mentioned(C#? Client)</para>
    ///<para>  then check default drive folder paths</para>
    ///<para>  If all Fail... Throw <see cref="FurcadiaNotInstalled"/> exception</para>
    ///<para>  Clients Should check for this error and then ask the user where to manually locate Furccadia</para>
    ///</remarks>
    /// </summary>
    public class Paths
    {
        #region Private Fields

        private Net.Utils.Utilities FurcadiaUtilities;

        #endregion Private Fields

        #region Public Constructors

        public Paths()
        {
            FurcadiaUtilities = new Net.Utils.Utilities();
            sLocaldirPath = GetFurcadiaLocaldirPath();
        }

        public Paths(string path)
        {
            FurcadiaUtilities = new Net.Utils.Utilities();
            sLocaldirPath = GetFurcadiaLocaldirPath();
        }

        #endregion Public Constructors



        #region Private Fields

        // Storing localdir upon class generation so the information is
        // cached. Otherwise, each property would have to look up localdir separately.
        private string sLocaldirPath;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Cache path - contains all the Furcadia cache and resides in the
        /// global user space.
        ///
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye Productions\Furcadia
        /// </summary>
        public string CachePath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(sLocaldirPath, @"tmp") : DefaultCachePath;
            }
        }

        /// <summary>
        /// Character file path - contains furcadia.ini files with login
        /// information for each character.
        ///
        /// Default: My Documents\Furcadia\Furcadia Characters\
        /// </summary>
        public string CharacterPath
        {
            get
            {
                return (UsingLocaldir) ? sLocaldirPath : DefaultCharacterPath;
            }
        }

        //--- FURCADIA CACHE ------------------------------------------------//
        public string DefaultCachePath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    @"Dragon's Eye Productions", @"Furcadia");
            }
        }

        public string DefaultCharacterPath
        {
            get
            {
                return Path.Combine(DefaultPersonalDataPath, @"Furcadia Characters");
            }
        }

        public string DefaultDreamsPath
        {
            get
            {
                return Path.Combine(DefaultPersonalDataPath, @"Dreams");
            }
        }

        /// <summary>
        /// Default Furcadia install folder - this path is used by default
        /// to install Furcadia to.
        ///
        /// Default: c:\Program Files\Furcadia
        /// </summary>
        public string DefaultFurcadiaPath
        {
            get
            {
                if (Environment.Is64BitOperatingSystem)
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                         @"Furcadia");
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    @"Furcadia");
            }
        }

        public string DefaultGlobalMapsPath
        {
            get
            {
                return Path.Combine(DefaultFurcadiaPath, @"maps");
            }
        }

        public string DefaultGlobalSkinsPath
        {
            get
            {
                return Path.Combine(DefaultFurcadiaPath, @"skins");
            }
        }

        public string DefaultLocalSkinsPath
        {
            get
            {
                return Path.Combine(DefaultPersonalDataPath, @"Skins");
            }
        }

        public string DefaultLogsPath
        {
            get
            {
                return Path.Combine(DefaultPersonalDataPath, @"Logs");
            }
        }

        /// <summary>
        /// Path to the default patch (graphics, sounds, layout) folder used
        /// to display Furcadia itself, its tools and environment.
        ///
        /// Default: c:\Program Files\Furcadia\patches\default
        /// </summary>
        public string DefaultPatchPath
        {
            get
            {
                return GetDefaultPatchPath();
            }
        }

        public string DefaultPermanentMapsCachePath
        {
            get
            {
                return Path.Combine(DefaultCachePath, @"Permanent Maps");
            }
        }

        public string DefaultPersonalDataPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    @"Furcadia");
            }
        }

        public string DefaultPortraitCachePath
        {
            get
            {
                return Path.Combine(DefaultCachePath, @"Portrait Cache");
            }
        }

        public string DefaultScreenshotsPath
        {
            get
            {
                return Path.Combine(DefaultPersonalDataPath, @"Screenshots");
            }
        }

        public string DefaultSettingsPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Dragon's Eye Productions", @"Furcadia");
            }
        }

        public string DefaultTemporaryDreamsPath
        {
            get
            {
                return Path.Combine(DefaultCachePath, @"Temporary Dreams");
            }
        }

        public string DefaultTemporaryFilesPath
        {
            get
            {
                return Path.Combine(DefaultCachePath, @"Temporary Files");
            }
        }

        public string DefaultTemporaryPatchesPath
        {
            get
            {
                return Path.Combine(DefaultCachePath, @"Temporary Patches");
            }
        }

        public string DefaultWhisperLogsPath
        {
            get
            {
                return Path.Combine(DefaultLogsPath, @"Whispers");
            }
        }

        /// <summary>
        /// Dreams path - contains Furcadia dreams made by the player.
        ///
        /// Default: My Documents\Furcadia\Dreams
        /// </summary>
        public string DreamsPath
        {
            get
            {
                return (UsingLocaldir) ? sLocaldirPath : DefaultDreamsPath;
            }
        }

        /// <summary>
        /// Furcadia Localdir path - this path (when explicitly set),
        /// indicates the whereabouts of the data files used in Furcadia. If
        /// localdir.ini is present in the Furcadia folder, Furcadia.exe
        /// will load those files from the specific path and not the regular ones.
        ///
        /// Default: -NONE-
        /// </summary>
        public string FurcadiaLocaldirPath
        {
            get
            {
                return GetFurcadiaLocaldirPath();
            }
        }

        //--- FURCADIA PROGRAM FILES ----------------------------------------//
        /// <summary>
        /// Furcadia install path - this path corresponds to the path where
        /// Furcadia is installed on the current machine. If Furcadia is not
        /// found, this property will be null.
        /// </summary>
        public string FurcadiaPath
        {
            get
            {
                return GetFurcadiaInstallPath();
            }
        }

        /// <summary>
        /// Path to the global maps, distributed with Furcadia and loadable
        /// during gameplay in some main dreams.
        ///
        /// Default: c:\Program Files\Furcadia\maps
        /// </summary>
        public string GlobalMapsPath
        {
            get
            {
                string path = FurcadiaPath;
                return (path != null) ? Path.Combine(path, @"maps") : null;
            }
        }

        /// <summary>
        /// Path to the global skins that change Furcadia's graphical
        /// layout. They are stored in the Furcadia program files folder.
        ///
        /// Default: c:\Program Files\Furcadia\skins
        /// </summary>
        public string GlobalSkinsPath
        {
            get
            {
                string path = FurcadiaPath;
                return (path != null) ? Path.Combine(path, @"skins") : null;
            }
        }

        /// <summary>
        /// LocalDir path - a specific path where all the player-specific
        /// and cache data is stored in its classic form. Used mainly to
        /// retain the classic path structure or to run Furcadia from a
        /// removable disk.
        /// </summary>
        public string LocaldirPath
        {
            get
            {
                return GetFurcadiaLocaldirPath();
            }
        }

        /// <summary>
        /// Local skins path - contains Furcadia skins used locally by each user.
        ///
        /// Default: My Documents\Furcadia\Skins
        /// </summary>
        public string LocalSkinsPath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(sLocaldirPath, @"skins") : DefaultLocalSkinsPath;
            }
        }

        /// <summary>
        /// Logs path - contains session logs for each character and a
        /// subfolder with whisper logs, should Pounce be enabled.
        ///
        /// Default: My Documents\Furcadia\Logs
        /// </summary>
        public string LogsPath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(sLocaldirPath, @"logs") : DefaultLogsPath;
            }
        }

        /// <summary>
        /// Permanent Maps cache path - contains downloaded main maps such
        /// as the festival maps or other DEP-specific customized dreams.
        ///
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Permanent Maps
        /// </summary>
        public string PermanentMapsCachePath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(sLocaldirPath, @"maps") : DefaultPermanentMapsCachePath;
            }
        }

        /// <summary>
        /// Personal data path - contains user-specific files such as logs,
        /// patches, screenshots and character files.
        ///
        /// Default: My Documents\Furcadia\
        /// </summary>
        public string PersonalDataPath
        {
            get
            {
                return (UsingLocaldir) ? sLocaldirPath : DefaultPersonalDataPath;
            }
        }

        /// <summary>
        /// Portrait cache path - contains downloaded portraits and desctags
        /// cache for faster loading and bandwidth optimization.
        ///
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Portrait Cache
        /// </summary>
        public string PortraitCachePath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(sLocaldirPath, @"portraits") : DefaultPortraitCachePath;
            }
        }

        /// <summary>
        /// Screenshots path - contains screen shot files taken by Furcadia
        /// with the CTRL+F1 hotkey. At the time of writing, in PNG format.
        ///
        /// Default: My Documents\Furcadia\Screenshots
        /// </summary>
        public string ScreenshotsPath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(sLocaldirPath, @"screenshots") : DefaultScreenshotsPath;
            }
        }

        /// <summary>
        /// Personal settings path - contains all the Furcadia settings for
        /// each user that uses it.
        ///
        /// Default (VISTA+): %USERPROFILE%\Local\AppData\Dragon's Eye Productions\Furcadia
        /// </summary>
        public string SettingsPath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(sLocaldirPath, @"settings") : DefaultSettingsPath;
            }
        }

        /// <summary>
        /// Temporary dreams path - contains downloaded player dreams for
        /// subsequent loading.
        ///
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Temporary Dreams
        /// </summary>
        public string TemporaryDreamsPath
        {
            get
            {
                return (UsingLocaldir) ? CachePath : DefaultTemporaryDreamsPath;
            }
        }

        /// <summary>
        /// Temporary files path - contains downloaded and uploaded files
        /// that are either used to upload packages or download them for extraction.
        ///
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Temporary Files
        /// </summary>
        public string TemporaryFilesPath
        {
            get
            {
                return (UsingLocaldir) ? CachePath : DefaultTemporaryFilesPath;
            }
        }

        /// <summary>
        /// Temporary patch path - contains downloaded temporary patches.
        /// This technology is never in use, yet supported, so this folder
        /// is always empty.
        ///
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Temporary Patches
        /// </summary>
        public string TemporaryPatchesPath
        {
            get
            {
                return (UsingLocaldir) ? CachePath : DefaultTemporaryPatchesPath;
            }
        }

        /// <summary>
        /// </summary>
        public bool UsingLocaldir
        {
            get { return (sLocaldirPath != null); }
        }

        //--- FURCADIA PERSONALIZED FILES -----------------------------------//
        /// <summary>
        /// Whisper logs path - contains whisper logs for each character
        /// whispered, recorded by Pounce with the whisper windows.
        ///
        /// Default: My Documents\Furcadia\Logs\Whispers
        /// </summary>
        public string WhisperLogsPath
        {
            get
            {
                return (UsingLocaldir) ? Path.Combine(LogsPath, @"whispers") : DefaultWhisperLogsPath;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Find the path to the default patch folder on the current machine.
        /// </summary>
        /// <returns>
        /// Path to the default patch folder or null if not found.
        /// </returns>
        public string GetDefaultPatchPath()
        {
            string path;

            // Checking registry for a path first of all.
            using (RegistryKey regkey = Registry.LocalMachine)
            {
                RegistryKey regpath = null;
                try
                {
                    regpath = regkey.OpenSubKey(FurcadiaUtilities.ReggistryPathX86 + @"Patches", false);
                    if (regpath != null)
                    {
                        path = regpath.GetValue("default").ToString();
                        regpath.Close();
                        if (System.IO.Directory.Exists(path))
                            return path; // Path found
                    }
                }
                catch
                {
                }
                try
                {
                    regpath = regkey.OpenSubKey(FurcadiaUtilities.ReggistryPathX64 + @"\Patches", false);
                    if (regpath != null)
                    {
                        path = regpath.GetValue("default").ToString();
                        regpath.Close();
                        if (System.IO.Directory.Exists(path))
                            return path; // Path found
                    }
                }
                catch
                {
                }
                try
                {
                    regpath = regkey.OpenSubKey(FurcadiaUtilities.ReggistryPathMono + @"/Patches", false);
                    if (regpath != null)
                    {
                        path = regpath.GetValue("default").ToString();
                        regpath.Close();
                        if (System.IO.Directory.Exists(path))
                            return path; // Path found
                    }
                }
                catch
                {
                }
            }
            // Making a guess from the FurcadiaPath or FurcadiaDefaultPath property.
            path = FurcadiaPath;
            if (path == null)
                path = DefaultFurcadiaPath;

            path = System.IO.Path.Combine(path, "patches", "default");

            if (System.IO.Directory.Exists(path))
                return path; // Path found

            // All options were exhausted - assume Furcadia not installed.
            return null;
        }

        //--- FURCADIA PERSONALIZED SETTINGS --------------------------------//
        //---  Functions ---//
        /// <summary>
        /// Find the path to Furcadia data files currently installed on this system.
        /// </summary>
        /// <returns>
        /// Path to the Furcadia program folder or null if not found/not installed.
        /// </returns>
        public string GetFurcadiaInstallPath()
        {
            string path;

            // Checking registry for a path first of all.
            using (RegistryKey regkey = Registry.LocalMachine)
            {
                RegistryKey regpath = null;
                try
                {
                    regpath = regkey.OpenSubKey(FurcadiaUtilities.ReggistryPathX64 + @"\Programs", false);
                    if (regpath != null)
                    {
                        path = regpath.GetValue("Path").ToString();
                        regpath.Close();
                        if (System.IO.Directory.Exists(path))
                            return path; // Path found
                    }
                }
                catch
                {
                }
                try
                {
                    regpath = regkey.OpenSubKey(FurcadiaUtilities.ReggistryPathX86 + @"\Programs", false);
                    if (regpath != null)
                    {
                        path = regkey.GetValue("Path").ToString();
                        regpath.Close();
                        if (System.IO.Directory.Exists(path))
                            return path; // Path found
                    }
                }
                catch
                {
                }
                try
                {
                    regpath = regkey.OpenSubKey(FurcadiaUtilities.ReggistryPathMono + @"/Programs", false);

                    if (regpath != null)
                    {
                        path = regpath.GetValue("Path").ToString();
                        regpath.Close();
                        if (System.IO.Directory.Exists(path))
                            return path; // Path found
                    }
                }
                catch
                {
                }
            }
            // Making a guess from the FurcadiaDefaultPath property.
            path = DefaultFurcadiaPath;
            if (System.IO.Directory.Exists(path))
                return path; // Path found

            // All options were exhausted - assume Furcadia not installed.
            return null;
        }

        /// <summary>
        /// Find the current localdir path where data files would be stored
        /// on the current machine.
        /// </summary>
        /// <returns>
        /// Path to the data folder from localdir.ini or null if not found.
        /// </returns>
        public string GetFurcadiaLocaldirPath()
        {
            string path;
            string install_path = FurcadiaPath;

            sLocaldirPath = null; // Reset in case we don't find it.

            // If we can't find Furc, we won't find localdir.ini
            if (install_path == null)
                return null; // Furcadia install path not found.

            // Try to locate localdir.ini
            string ini_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localdir.ini");
            if (!System.IO.File.Exists(ini_path))
                ini_path = Path.Combine(install_path, "localdir.ini");
            if (!System.IO.File.Exists(ini_path))
                return null; // localdir.ini not found - regular path structure applies.

            // Read localdir.ini for remote path and verify it.
            using (StreamReader sr = new StreamReader(ini_path))
            {
                path = sr.ReadLine();
                if (path != null)
                    path = path.Trim();
                else
                    path = Path.GetDirectoryName(ini_path);
                sr.Close();
            }

            if (!System.IO.Directory.Exists(path))
                return null; // localdir.ini found, but the path in it is missing.

            sLocaldirPath = path; // Cache for the class use.

            return sLocaldirPath; // Localdir path found!
        }

        #endregion Public Methods
    }
}