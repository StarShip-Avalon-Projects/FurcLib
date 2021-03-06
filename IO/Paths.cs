using Microsoft.Win32;
using System;
using System.IO;

namespace Furcadia.IO
{
    ///<summary>
    /// This class contains all the paths related to the users furcadia installation.
    ///<para>***NOTICE: DO NOT REMOVE***</para>
    ///<para> Credits go to Artex for helping me fix Path issues and contributing his code.</para>
    ///<para>***NOTICE: DO NOT REMOVE.***</para>
    ///<para>Log Header</para>
    ///<para>Format: (date,Version) AuthorName, Changes.</para>
    ///<para> (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path</para>
    ///<para>  (June 1, 2016) Gerolkae, Added possible missing Registry Paths for x86/x64 Windows and Mono Support. Wine Support also contains these corrections.</para>
    ///</summary>
    ///<remarks>
    ///  Theory check all known default paths
    ///<para> check localdir.ini</para>
    ///<para>  then check each registry hives for active CPU type</para>
    ///<para>  Then check each give for default 32bit path(Non wow6432node)</para>
    ///<para>  then check Wine variants(C++ Win32 client)</para>
    ///<para>  then check Mono Versions for before mentioned(C#? Client)</para>
    ///<para>  then check default drive folder paths</para>
    ///<para>  If all Fail... Throw <see cref="FurcadiaNotFoundException"/> exception</para>
    ///<para>  Clients Should check for this error and then ask the user where to manually locate Furccadia</para>
    ///</remarks>
    public class Paths

    {
        #region Private Fields

        private Net.Utils.Utilities FurcadiaUtilities;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Load Default Furcadia Paths
        /// </summary>
        public Paths()
        {
            FurcadiaUtilities = new Net.Utils.Utilities();
            sLocaldirPath = GetFurcadiaLocaldirPath();
        }

        /// <summary>
        /// Load Paths Based on <paramref name="path"/>
        /// </summary>
        /// <param name="path">Specified directory to look for a nonstandard Furcadia install</param>
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
        ///<para/>
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye Productions\Furcadia
        /// </summary>
        public string CachePath => UsingLocaldir ? Path.Combine(sLocaldirPath, @"tmp") : DefaultCachePath;

        /// <summary>
        /// Character file path - contains furcadia.ini files with login
        /// information for each character.
        ///<para/>
        /// Default: My Documents\Furcadia\Furcadia Characters\
        /// </summary>
        [Legacy]
        [Obsolete("As of The Second Dreaming, Tis is now legacy")]
        public string CharacterPath => UsingLocaldir ? sLocaldirPath : DefaultCharacterPath;

        /// <summary>
        /// c:\Program Data\
        /// </summary>
        public string DefaultCachePath => Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    @"Dragon's Eye Productions", @"Furcadia");

        /// <summary>
        /// Default Character Path
        /// </summary>
        [Legacy]
        [Obsolete("As of The Second Dreaming, Tis is now legacy")]
        public string DefaultCharacterPath => Path.Combine(DefaultPersonalDataPath,
            @"Furcadia Characters");

        /// <summary>
        /// Personal Dreams Folder
        /// </summary>
        public string DefaultDreamsPath => Path.Combine(DefaultPersonalDataPath, @"Dreams");

        /// <summary>
        /// Default Furcadia install folder - this path is used by default
        /// to install Furcadia to.
        ///<para/>
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

        /// <summary>
        /// Main Maps Default path
        /// </summary>
        public string DefaultGlobalMapsPath => Path.Combine(DefaultFurcadiaPath, @"maps");

        /// <summary>
        /// default skins
        /// </summary>
        public string DefaultGlobalSkinsPath => Path.Combine(DefaultFurcadiaPath, @"skins");

        /// <summary>
        /// default local skins
        /// </summary>
        public string DefaultLocalSkinsPath => Path.Combine(DefaultPersonalDataPath, @"Skins");

        /// <summary>
        /// default personal log folder
        /// </summary>
        [Legacy]
        [Obsolete("As of The Second Dreaming, Tis is now legacy")]
        public string DefaultLogsPath => Path.Combine(DefaultPersonalDataPath, @"Logs");

        /// <summary>
        /// Path to the default patch (graphics, sounds, layout) folder used
        /// to display Furcadia itself, its tools and environment.
        ///<para/>
        /// Default: c:\Program Files\Furcadia\patches\default
        /// </summary>
        public string DefaultPatchPath => GetDefaultPatchPath();

        /// <summary>
        /// Default Main Maps
        /// </summary>
        public string DefaultPermanentMapsCachePath => Path.Combine(DefaultCachePath, @"Permanent Maps");

        /// <summary>
        /// Default Documents\Furcadia
        /// </summary>
        public string DefaultPersonalDataPath => Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    @"Furcadia");

        /// <summary>
        /// Portrait cache
        /// </summary>
        public string DefaultPortraitCachePath => Path.Combine(DefaultCachePath, @"Portrait Cache");

        /// <summary>
        /// Furcadia Screen Shots default folder
        /// </summary>
        public string DefaultScreenshotsPath => Path.Combine(DefaultPersonalDataPath, @"Screenshots");

        /// <summary>
        /// User App Data Settings
        /// </summary>
        public string DefaultSettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Dragon's Eye Productions", @"Furcadia");

        /// <summary>
        /// Temporary dreams
        /// </summary>
        public string DefaultTemporaryDreamsPath => Path.Combine(DefaultCachePath, @"Temporary Dreams");

        /// <summary>
        /// Default Temporary Files
        /// </summary>
        public string DefaultTemporaryFilesPath => Path.Combine(DefaultCachePath, @"Temporary Files");

        /// <summary>
        /// Temporary patches
        /// </summary>
        public string DefaultTemporaryPatchesPath => Path.Combine(DefaultCachePath, @"Temporary Patches");

        /// <summary>
        /// Whisper Logs
        /// </summary>
        [Legacy]
        [Obsolete("As of The Second Dreaming, Tis is now legacy")]
        public string DefaultWhisperLogsPath => Path.Combine(DefaultLogsPath, @"Whispers");

        /// <summary>
        /// Dreams path - contains Furcadia dreams made by the player.
        ///
        /// Default: My Documents\Furcadia\Dreams
        /// </summary>
        public string DreamsPath => UsingLocaldir ? sLocaldirPath : DefaultDreamsPath;

        /// <summary>
        /// Furcadia Localdir path - this path (when explicitly set),
        /// indicates the whereabouts of the data files used in Furcadia. If
        /// localdir.ini is present in the Furcadia folder, Furcadia.exe
        /// will load those files from the specific path and not the regular ones.
        ///
        /// Default: -NONE-
        /// </summary>
        public string FurcadiaLocaldirPath => GetFurcadiaLocaldirPath();

        /// <summary>
        /// Furcadia install path - this path corresponds to the path where
        /// Furcadia is installed on the current machine. If Furcadia is not
        /// found, this property will be null.
        /// </summary>
        public string FurcadiaPath => GetFurcadiaInstallPath();

        /// <summary>
        /// Path to the global maps, distributed with Furcadia and loadable
        /// during game play in some main dreams.
        ///<para>
        /// Default: c:\Program Files\Furcadia\maps
        /// </para>
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
        ///<para>
        ///Default: c:\Program Files\Furcadia\skins
        /// </para>
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
        public string LocaldirPath => GetFurcadiaLocaldirPath();

        /// <summary>
        /// Local skins path - contains Furcadia skins used locally by each user.
        /// <para>
        /// Default: My Documents\Furcadia\Skins
        /// </para>
        /// </summary>
        public string LocalSkinsPath => UsingLocaldir ? Path.Combine(sLocaldirPath, @"skins") : DefaultLocalSkinsPath;

        /// <summary>
        /// Logs path - contains session logs for each character and a
        /// sub-folder with whisper logs, should Pounce be enabled.
        /// <para>
        /// Default: My Documents\Furcadia\Logs
        /// </para>
        /// </summary>
        [Legacy]
        [Obsolete("As of The Second Dreaming, Tis is now legacy")]
        public string LogsPath => UsingLocaldir ? Path.Combine(sLocaldirPath, @"logs") : DefaultLogsPath;

        /// <summary>
        /// Permanent Maps cache path - contains downloaded main maps such
        /// as the festival maps or other DEP-specific customized dreams.
        ///<para>
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye Productions\Furcadia\Permanent Maps
        /// </para>
        /// </summary>
        public string PermanentMapsCachePath => UsingLocaldir ? Path.Combine(sLocaldirPath, @"maps") : DefaultPermanentMapsCachePath;

        /// <summary>
        /// Personal data path - contains user-specific files such as logs,
        /// patches, screen shots and character files.
        ///<para >
        /// Default: My Documents\Furcadia\
        /// </para>
        /// </summary>
        public string PersonalDataPath => UsingLocaldir ? sLocaldirPath : DefaultPersonalDataPath;

        /// <summary>
        /// Portrait cache path - contains downloaded portraits and desctags
        /// cache for faster loading and bandwidth optimization.
        /// <para>
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye Productions\Furcadia\Portrait Cache
        /// </para>
        /// </summary>
        public string PortraitCachePath => UsingLocaldir ? Path.Combine(sLocaldirPath, @"portraits") : DefaultPortraitCachePath;

        /// <summary>
        /// Screen shots path - contains screen shot files taken by Furcadia
        /// with the CTRL+F1 hotkey. At the time of writing, in PNG format.
        /// <para>
        /// Default: My Documents\Furcadia\Screenshots
        /// </para>
        /// </summary>
        public string ScreenshotsPath => UsingLocaldir ? Path.Combine(sLocaldirPath, @"screenshots") : DefaultScreenshotsPath;

        /// <summary>
        /// Personal settings path - contains all the Furcadia settings for
        /// each user that uses it.
        ///<para/>
        /// Default (VISTA+): %USERPROFILE%\Local\AppData\Dragon's Eye Productions\Furcadia
        /// </summary>
        public string SettingsPath => UsingLocaldir ? Path.Combine(sLocaldirPath, @"settings") : DefaultSettingsPath;

        /// <summary>
        /// Temporary dreams path - contains downloaded player dreams for
        /// subsequent loading.
        ///<para/>
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Temporary Dreams
        /// </summary>
        public string TemporaryDreamsPath => UsingLocaldir ? CachePath : DefaultTemporaryDreamsPath;

        /// <summary>
        /// Temporary files path - contains downloaded and uploaded files
        /// that are either used to upload packages or download them for extraction.
        ///<para/>
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Temporary Files
        /// </summary>
        public string TemporaryFilesPath => UsingLocaldir ? CachePath : DefaultTemporaryFilesPath;

        /// <summary>
        /// Temporary patch path - contains downloaded temporary patches.
        /// This technology is never in use, yet supported, so this folder
        /// is always empty.
        /// <para/>
        /// Default: %ALLUSERSPROFILE%\Dragon's Eye
        ///          Productions\Furcadia\Temporary Patches
        /// </summary>
        public string TemporaryPatchesPath => UsingLocaldir ? CachePath : DefaultTemporaryPatchesPath;

        /// <summary>
        /// Has LoclDir.ini been detected?
        /// </summary>
        public bool UsingLocaldir => sLocaldirPath != null;

        /// <summary>
        /// Whisper logs path - contains whisper logs for each character
        /// whispered, recorded by Pounce with the whisper windows.
        ///
        /// Default: My Documents\Furcadia\Logs\Whispers
        /// </summary>
        [Legacy]
        [Obsolete("As of The Second Dreaming, Tis is now legacy")]
        public string WhisperLogsPath => UsingLocaldir ? Path.Combine(LogsPath, @"whispers") : DefaultWhisperLogsPath;

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
                        if (Directory.Exists(path))
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
                        if (Directory.Exists(path))
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
                        if (Directory.Exists(path))
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

            path = Path.Combine(path, "patches", "default");

            if (Directory.Exists(path))
                return path; // Path found

            // All options were exhausted - assume Furcadia not installed.
            return null;
        }

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
                        if (Directory.Exists(path))
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
                        if (Directory.Exists(path))
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
                        if (Directory.Exists(path))
                            return path; // Path found
                    }
                }
                catch
                {
                }
            }
            // Making a guess from the FurcadiaDefaultPath property.
            path = DefaultFurcadiaPath;
            if (Directory.Exists(path))
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
            if (!File.Exists(ini_path))
                ini_path = Path.Combine(install_path, "localdir.ini");
            if (!File.Exists(ini_path))
                return null; // localdir.ini not found - regular path structure applies.

            // Read localdir.ini for remote path and verify it.
            using (StreamReader streamReader = new StreamReader(ini_path))
            {
                path = streamReader.ReadLine();
                if (path != null)
                    path = path.Trim();
                else
                    path = Path.GetDirectoryName(ini_path);
            }

            if (!Directory.Exists(path))
                return null; // localdir.ini found, but the path in it is missing.

            sLocaldirPath = path; // Cache for the class use.

            return sLocaldirPath; // Localdir path found!
        }

        #endregion Public Methods
    }
}