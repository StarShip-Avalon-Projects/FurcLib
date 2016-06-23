/****NOTICE: DO NOT REMOVE
 * Credits go to Artex for helping me fix Path issues and
 * contributing his code.
 ****NOTICE: DO NOT REMOVE.*
*Log Header
 *Format: (date,Version) AuthorName, Changes.
 * (Mar 12,2014,0.2.12) Gerolkae, Adapted Paths to work with a Supplied path
 *  (June 1, 2016) Gerolkae, Added possible missing Registry Paths for X86/x64 Windows and Mono Support. Wine Support also contains these corrections
 */

/*
 * Theory Check all known default paths
 * check localdir.ini
 * then Check Each Registry Hives for active CPU type
 * Then Check each Hive For default 32bit path (Non wow6432node)
 * then Check Wine Varants (C++ Win32 client)
 * then Check Mono Versions for beforementioned (C# Client)
 * then check Default Drive folder Paths
 * If all Fail... Throw FurcadiaNotInstalled Excempt
 * Clients Should check for this error and then ask the user where to manually locate Furc
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using Microsoft.Win32;

namespace Furcadia.IO
{
	/// <summary>
	/// Contains functions for getting some base paths related to Furcadia.
	/// </summary>
	public class Paths
	{
		
		private string RegPathx64 = @"SOFTWARE\Wow6432Node\Dragon's Eye Productions\Furcadia\";
		private string RegPathx32 = @"SOFTWARE\Dragon's Eye Productions\Furcadia\";
        			
#region Constructors
		/// <summary>
        /// Defines the base path for the Furcadia Directory
        /// </summary>
        public  Paths()
        {
            _installpath = null;
        }
			/// <summary>
        /// Defines the base path for the Furcadia Directory
        /// Throws FurcadiaNotFound exception if furcadia.exe cannot be found in specified path
        /// </summary>
        /// <param name="path"></param>
        public  Paths(string path)
        {
            if (!File.Exists(Path.Combine( path , "Furcadia.exe"))) throw new FurcadiaNotFoundException("Furcadia Framework cannot find the fucadia installation, Please supply a valid path");
            _installpath = path;
        }
#endregion

        private  string _FurcadiaCharactersPath = null;


        /// <summary>
        /// Gets the location of the Furcadia Character Files
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> containing the location of Furcadia Characters folder in "My Documents".
        /// </returns>
        public  string GetFurcadiaCharactersPath()
        {
            if (!String.IsNullOrEmpty(_FurcadiaCharactersPath)) return _FurcadiaCharactersPath;
            string path = System.IO.Path.Combine(GetFurcadiaDocPath(),"Furcadia Characters");
            if (!System.IO.Directory.Exists(path))
                path = GetFurcadiaDocPath();
            if (Directory.Exists(path))
            {
                _FurcadiaCharactersPath = path;
                return _FurcadiaCharactersPath;
            }
            throw new FurcadiaNotFoundException("Furcadia Characters path not found.\n" + path);
        }


		private   string _FurcadiaDocpath;
		/// <summary>
		/// Gets the location of the Furcadia folder located in "My Documents"
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> containing the location of Furcadia folder in "My Documents".
		/// </returns>
		public  string GetFurcadiaDocPath()
		{
			if (!String.IsNullOrEmpty(_FurcadiaDocpath)) return _FurcadiaDocpath;
			string path = GetLocaldirPath();
			if (String.IsNullOrEmpty(path))
				path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
									"Furcadia");

			if (Directory.Exists(path))
			{
				_FurcadiaDocpath = path;
				return _FurcadiaDocpath;
			}
			throw new FurcadiaNotFoundException("Furcadia documents path not found.\n" + path);
		}

		/// <summary>
		/// Determines the registry path by platform. (x32/x64)
		/// Thanks to Ioka for this one.
		/// </summary>
		/// <returns>
		/// A path to the Furcadia registry folder or NullReferenceException.
		/// </returns>
		public  string ProgramFilesX86()
		{
#if Net40 == True
            if (OSBitness.Is64BitOperatingSystem())
#else
            if (Environment.Is64BitOperatingSystem)
#endif
            {
				return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
			}
			return Environment.GetEnvironmentVariable("ProgramFiles");
		}
		
				/// <summary>
		/// Determines the registry path by platform. (x32/x64)
		/// Thanks to Ioka for this one.
		/// </summary>
		/// <returns>
		/// A path to the Furcadia registry folder or NullReferenceException.
		/// </returns>
		public  string GetRegistryPath()
		{
#if Net40 == True
            if (OSBitness.Is64BitOperatingSystem())
#else
            if (Environment.Is64BitOperatingSystem)
#endif

			{
				return RegPathx64;
			}
			return RegPathx32;
		}

		private  string _installpath;
		/// <summary>
		/// Find the path to Furcadia data files currently installed on this
		/// system.
		/// </summary>
		/// <returns>Path to the Furcadia program folder or null if not found/not installed.</returns>
		public string GetInstallPath()
		{
			//If path already found return it.
			if (!string.IsNullOrEmpty(_installpath)) return _installpath;
			string path = null;

			// Checking registry for a path (WINDOWS ONLY)
			if (Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				Environment.OSVersion.Platform == PlatformID.Win32NT)
			{

                // Current User Hive with x64 CPU Check
				RegistryKey regkey = Registry.CurrentUser;
				try
				{
					regkey = regkey.OpenSubKey(GetRegistryPath() + "Programs", false);
					path = regkey.GetValue("path").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_installpath = path;
						return _installpath; // Path found
					}
				}
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG: HKCU\\{0} \n\r{1}", GetRegistryPath(), e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

                // Local Machine Hive with x64 CPU Check
                regkey = Registry.LocalMachine;
				try
				{
					regkey = regkey.OpenSubKey(GetRegistryPath() + "Programs", false);
					path = regkey.GetValue("path").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_installpath = path;
						return _installpath; // Path found
					}
				}
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG: HKLM\\{0} \n\r{1}", GetRegistryPath(), e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

                // Current User Hive with x64 CPU Check Failed
				regkey = Registry.CurrentUser;
				try
				{
					regkey = regkey.OpenSubKey(RegPathx32 + "Programs", false);
					path = regkey.GetValue("path").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_installpath = path;
						return _installpath; // Path found
					}
				}
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG: HKCU\\{0} \n\r{1}", RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }
				regkey = Registry.LocalMachine;
                try
                {
                    regkey = regkey.OpenSubKey(RegPathx32 + "Programs", false);
                    path = regkey.GetValue("Path").ToString();
                    regkey.Close();
                    if (System.IO.Directory.Exists(path))
                    {
                        _installpath = path;
                        return _installpath; // Path found
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG::Programs:: HKLM\\{0} , {1}",RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

                // Current User Hive with x64 CPU Check Failed
                regkey = Registry.CurrentUser;
                try
                {
                    regkey = regkey.OpenSubKey(RegPathx32 + "Programs", false);
                    path = regkey.GetValue("Path").ToString();
                    regkey.Close();
                    if (System.IO.Directory.Exists(path))
                    {
                        _installpath = path;
                        return _installpath; // Path found
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG::Programs:: HKCU\\{0} , {1}", RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

				// Making a guess from the FurcadiaDefaultPath property.
				path = Path.Combine(ProgramFilesX86(), "Furcadia");
			}
			// Scanning registry for a path (NON-WINDOWS ONLY)
			else
			{
				path = RegistryExplorerForWine.ReadSubKey("\\HKEY_LOCAL_MACHINE\\" + GetRegistryPath().Replace("\\", "/") + "Programs", "Path");
				if (path == null)
					path = RegistryExplorerForWine.ReadSubKey("\\HKEY_CURRENT_USER\\" + GetRegistryPath().Replace("\\", "/") + "Programs", "Path");
				if (path == null)
					path = RegistryExplorerForWine.ReadSubKey("\\HKEY_LOCAL_MACHINE\\" + RegPathx32.Replace("\\", "/") + "Programs", "Path");
				if (path == null)
					path = RegistryExplorerForWine.ReadSubKey("\\HKEY_CURRENT_USER\\" + RegPathx32.Replace("\\", "/") + "Programs", "Path");
				if (System.IO.Directory.Exists(path))
				{
					_installpath = path;
					return _installpath; // Path found
				}
			}
			#region "Mono Runtime"
			//Prep for c# Client Mono install
			// Wine don't have the registry lets Try Mono
			Type t = Type.GetType ("Mono.Runtime");
       		if (t != null)
       		{
                // Current Users Hive with x64 CPU Check
       			RegistryKey regkey = Registry.CurrentUser;
				try
				{
					regkey = regkey.OpenSubKey(GetRegistryPath() + "Programs", false);
					path = regkey.GetValue("path").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_installpath = path;
						return _installpath; // Path found
					}
				}
				catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG::mono:: HKCU\\{0} \n\r{1}", GetRegistryPath(), e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

                // Mono Local Machine with x64 CPU Check
				regkey = Registry.LocalMachine;
				try
				{
					regkey = regkey.OpenSubKey(GetRegistryPath() + "Programs", false);
					path = regkey.GetValue("path").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_installpath = path;
						return _installpath; // Path found
					}
				}
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG: HKLM\\{0} \n\r{1}", GetRegistryPath(), e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

				regkey = Registry.LocalMachine;
                try
                {
                    regkey = regkey.OpenSubKey(RegPathx32 + "Programs", false);
                    path = regkey.GetValue("path").ToString();
                    regkey.Close();
                    if (System.IO.Directory.Exists(path))
                    {
                        _installpath = path;
                        return _installpath; // Path found
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG: HKLM\\{0} \n\r{1}", RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Close();
                }
      			// All options were exhausted - assume Furcadia not installed.
				if (System.IO.Directory.Exists(path))
				{
					_installpath = path;
					return _installpath; // Path found
				}

			}
			throw new FurcadiaNotFoundException("Furcadia Install path not found." + "\n" + path);
			#endregion
			
		}
		private string _defaultpatchpath;
			/// <summary>
		/// Find the path to the default patch folder on the current machine.
		/// </summary>
		/// <returns>Path to the default patch folder or null if not found.</returns>
		public string GetDefaultPatchPath()
		{
			//If path already found return it.
			if (!string.IsNullOrEmpty(_defaultpatchpath)) return _defaultpatchpath;
			string path;

			// Checking registry for a path first of all.
			if (Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
                // Current Hive with x64 CPU Check
				RegistryKey regkey = Registry.CurrentUser;
				try
				{
					regkey = regkey.OpenSubKey(GetRegistryPath() + "Patches", false);
					path = regkey.GetValue("default").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_defaultpatchpath = path;
						return _defaultpatchpath; // Path found
					}
				}
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG::Patched:: HKCU\\{0} , {1}", GetRegistryPath(), e.Message));
#endif
                }
                finally
                {
                    regkey.Close();
                }

                // Local Machine Hive with x64 CPU Check
                regkey = Registry.LocalMachine;
                try
                {
                    regkey = regkey.OpenSubKey(GetRegistryPath() + "Patches", false);
                    path = regkey.GetValue("default").ToString();
                    regkey.Close();
                    if (System.IO.Directory.Exists(path))
                    {
                        _defaultpatchpath = path;
                        return _defaultpatchpath; // Path found
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG::Patched:: HKCU\\{0} , {1}", GetRegistryPath(), e.Message));
#endif
                }
                finally
                {
                    regkey.Close();
                }
				regkey = Registry.LocalMachine;
                try
                {
                    regkey = regkey.OpenSubKey(RegPathx32 + "Patches", false);
                    path = regkey.GetValue("default").ToString();
                    regkey.Close();
                    if (System.IO.Directory.Exists(path))
                    {
                        _defaultpatchpath = path;
                        return _defaultpatchpath; // Path found
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG::Patched:: HKLM\\{0} , {1}", RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }
				regkey = Registry.CurrentUser;
				try
				{
					regkey = regkey.OpenSubKey(RegPathx32 + "Patches", false);
					path = regkey.GetValue("default").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_defaultpatchpath = path;
						return _defaultpatchpath; // Path found
					}
				}
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG::Patched:: HKCU\\{0} , {1}", RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

				// Making a guess from the FurcadiaPath or FurcadiaDefaultPath property.
				path = GetInstallPath();
				if (path == string.Empty)
					path = Path.Combine(ProgramFilesX86(), "Furcadia");

				path = Path.Combine(path, "/patches/default");
			}
			else
			{
			// Search Wine paths if Wine is used
			//TODO: Check c# Client
				path = RegistryExplorerForWine.ReadSubKey("HKEY_LOCAL_MACHINE\\" + GetRegistryPath() + "Patches", "default");
				if (path == null)
					path = RegistryExplorerForWine.ReadSubKey("\\HKEY_CURRENT_USER\\" + GetRegistryPath() + "Patches", "default");
				if (path == null)
					path = RegistryExplorerForWine.ReadSubKey("\\HKEY_LOCAL_MACHINE\\" + RegPathx32 + "Patches", "Default");
				if (path == null)
					path = RegistryExplorerForWine.ReadSubKey("\\HKEY_CURRENT_USER\\" + RegPathx32 + "Patches", "default");
			}
			if (System.IO.Directory.Exists(path))
			{
				_defaultpatchpath = path;
				return _defaultpatchpath; // Path found
			}
			
			#region "Mono Runtime"
			//Prep for c# Client Mono install
			Type t = Type.GetType ("Mono.Runtime");
       		if (t != null)
       		{
				RegistryKey regkey = Registry.CurrentUser;
                try
                {
                    regkey = regkey.OpenSubKey(GetRegistryPath() + "Patches", false);
                    path = regkey.GetValue("default").ToString();
                    regkey.Close();
                    if (System.IO.Directory.Exists(path))
                    {
                        _defaultpatchpath = path;
                        return _defaultpatchpath; // Path found
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG:mono:Patched:: HKCU\\{0} , {1}", GetRegistryPath(), e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }

				regkey = Registry.LocalMachine;
                try
                {
                    regkey = regkey.OpenSubKey(RegPathx32 + "Patches", false);
                    path = regkey.GetValue("default").ToString();
                    regkey.Close();
                    if (System.IO.Directory.Exists(path))
                    {
                        _defaultpatchpath = path;
                        return _defaultpatchpath; // Path found
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG:mono:Patched:: HKLM\\{0} , {1}", RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }
				regkey = Registry.CurrentUser;
				try
				{
					regkey = regkey.OpenSubKey( RegPathx32 + "Patches", false);
					path = regkey.GetValue("default").ToString();
					regkey.Close();
					if (System.IO.Directory.Exists(path))
					{
						_defaultpatchpath = path;
						return _defaultpatchpath; // Path found
						}
					}
                catch (Exception e)
                {
#if DEBUG
                    Console.WriteLine(string.Format("DEBUG:mono:Patched:: HKCU\\{0} , {1}", RegPathx32, e.Message));
#endif
                }
                finally
                {
                    regkey.Dispose();
                }
				if (System.IO.Directory.Exists(path))
				{
					_defaultpatchpath = path;
					return _defaultpatchpath; // Path found
				}
			}
			#endregion

			// All options were exhausted - assume Furcadia not installed.
			throw new DirectoryNotFoundException("Furcadia Install path not found.");
		}
		
		
		private string _localsettingspath;
		/// <summary>
		/// Get the path to the Local Settings directory for Furcadia.
		/// </summary>
		/// <returns>Furcadia local settings directory.</returns>
		public string GetLocalSettingsPath()
		{
            if (!string.IsNullOrEmpty(_localsettingspath)) return _localsettingspath;
            else _localsettingspath = GetLocaldirPath() + "settings/";
            if (String.IsNullOrEmpty(GetLocaldirPath()))
				_localsettingspath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
												  "Dragon's Eye Productions/Furcadia");
			return _localsettingspath;
		}

		private string _cachepath;
		/// <summary>
		/// Get the All Users Application Data path for Furcadia.
		/// </summary>
		/// <returns>All Users Application Data path for Furcadia.</returns>
		public string GetCachePath()
		{
			if (!String.IsNullOrEmpty(_cachepath)) return _cachepath;
			else _cachepath = GetLocaldirPath();
			if (String.IsNullOrEmpty(_cachepath))
				_cachepath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
										  "Dragon's Eye Productions/Furcadia");
			return _cachepath;
		}

		private string _dynavpath;
		/// <summary>
		/// Get the All Dynamic Avatar path for Furcadia.
		/// </summary>
		/// <returns>All Dynamic Avatar path for Furcadia.</returns>
		public string GetDynAvatarPath()
		{
            if (!String.IsNullOrEmpty(_dynavpath)) return _dynavpath;
            else _dynavpath = GetLocaldirPath();
            if (String.IsNullOrEmpty(_dynavpath))
                _dynavpath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
										  "Dragon's Eye Productions/Furcadia/Dynamic Avatars");
            return _dynavpath;
		}

		private string _localdirpath;
		/// <summary>
		/// Find the current localdir path where data files would be stored
		/// on the current machine.
		/// </summary>
		/// <returns>Path to the data folder from localdir.ini or null if not found.</returns>
		public string GetLocaldirPath()
		{
			if (!string.IsNullOrEmpty(_localdirpath)) return _localdirpath;
			string path;
			string install_path = GetInstallPath();

			// If we can't find Furc, we won't find localdir.ini
			if (install_path == null)
				return null; // Furcadia install path not found.

			// Try to locate localdir.ini
			string ini_path = String.Format("{0}/localdir.ini", install_path);
			if (!System.IO.File.Exists(ini_path))
				return null; // localdir.ini not found - regular path structure applies.

			// Read localdir.ini for remote path and verify it.
			StreamReader sr = new StreamReader(ini_path);
			path = sr.ReadLine();
            if (!string.IsNullOrEmpty(path))
                path.Trim();
			sr.Close();
            if (String.IsNullOrEmpty(path))
                path = install_path;
			if (!System.IO.Directory.Exists(path))
				throw new DirectoryNotFoundException("Path not found in localdir.ini"); // localdir.ini found, but the path in it is missing.
			_localdirpath = path;
			return _localdirpath; // Localdir path found!
		}


        #region "MSPL Code"
#if Net40 == True
        // Source: http://1code.codeplex.com/SourceControl/changeset/view/39074#842775
        //**************************************************************************\
        //        * Portions of this source are subject to the Microsoft Public License.
        //        * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
        //        * All other rights reserved.
        //        * 
        //        * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
        //        * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
        //        * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
        //        \**************************************************************************


        public sealed class OSBitness
        {
            public OSBitness()
            {
            }
            #region "Is64BitOperatingSystem (IsWow64Process)"

            /// <summary>
            /// The function determines whether the current operating system is a 
            /// 64-bit operating system.
            /// </summary>
            /// <returns>
            /// The function returns true if the operating system is 64-bit; 
            /// otherwise, it returns false.
            /// </returns>
            public static bool Is64BitOperatingSystem()
            {
                if (IntPtr.Size == 8)
                {
                    // 64-bit programs run only on Win64
                    return true;
                }
                else
                {
                    // 32-bit programs run on both 32-bit and 64-bit Windows
                    // Detect whether the current process is a 32-bit process 
                    // running on a 64-bit system.
                    bool flag = false;
                    return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") && IsWow64Process(GetCurrentProcess(), ref flag)) && flag);
                }
            }

            /// <summary>
            /// The function determins whether a method exists in the export 
            /// table of a certain module.
            /// </summary>
            /// <param name="moduleName">The name of the module</param>
            /// <param name="methodName">The name of the method</param>
            /// <returns>
            /// The function returns true if the method specified by methodName 
            /// exists in the export table of the module specified by moduleName.
            /// </returns>
            private static bool DoesWin32MethodExist(string moduleName, string methodName)
            {
                IntPtr moduleHandle = GetModuleHandle(moduleName);
                if (moduleHandle == IntPtr.Zero)
                {
                    return false;
                }
                return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
            }

            [DllImport("kernel32.dll")]
            private static extern IntPtr GetCurrentProcess();

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr GetModuleHandle(string moduleName);

            [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]
string procName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool IsWow64Process(IntPtr hProcess, ref bool wow64Process);

            #endregion
        }
#endif
        #endregion



	}


}
