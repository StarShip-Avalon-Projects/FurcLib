﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Furcadia.IO
{
    internal class RegistryExplorerForWine
    {
        #region Internal Methods

        internal static string FormatWineHDDir(string path)
        {
            string _s = path.Substring(0, 1);
            string newPath = path.Remove(0, 3);
            //newPath = newPath.Replace("\\",Path.DirectorySeparatorChar.ToString());
            switch (_s.ToLower())
            {
                case "c":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.wine/drive_c/" + newPath;
            }
            return path;
        }

        internal static string ReadSubKey(string regPath, string key)
        {
            string path = null;
            ProcessStartInfo startinfo = new ProcessStartInfo("regedit", "/S /E " + "tmp.reg " + "\"" + regPath + "\"");
            Process proc = Process.Start(startinfo);
            proc.Close();

            int _timeout = 0;
            while (File.Exists("tmp.reg") == false && _timeout < 10)
            {
                _timeout++;
                Thread.Sleep(100);
            }

            FileStream fs = TryOpenFile("tmp.reg", FileMode.Open, FileAccess.Read, FileShare.Read, 3, 1000);
            if (fs != null)
            {
                path = parseRegFile(fs, key);
                path = path.Remove(0, path.IndexOf('=') + 1);
                File.Delete("tmp.reg");
                if (path != null)
                {
                    path = FormatWineHDDir(path);
                }
            }
            return path;
        }

        #endregion Internal Methods

        #region UGLY Wine Registry Code

        internal static string parseRegFile(FileStream file, string key)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                List<string> lines = new List<string>();
                string _l = null;
                while ((_l = reader.ReadLine()) != null)
                {
                    lines.Add(_l);
                }
                for (int i = 1; i <= lines.Count - 1; i++)
                {
                    if (lines[i].Length > 4 && lines[i].Contains(key))
                    {
                        string line = lines[i].Substring(1, lines[i].Length - 1);
                        line = line.Replace("\"", "").Replace(@"\\", "/");
                        return line;
                    }
                }
            }
            return null;
        }

        internal static FileStream TryOpenFile(string filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int maximumAttempts, int attemptWaitMS)
        {
            FileStream fs = null;
            int attempts = 0;
            while (true)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        fs = File.Open(filePath, fileMode, fileAccess, fileShare);
                        return fs;
                    }
                }
#pragma warning disable CS0168 // The variable 'ioEx' is declared but never used
                catch (IOException ioEx)
#pragma warning restore CS0168 // The variable 'ioEx' is declared but never used
                {
                    attempts++;
                    if (attempts > maximumAttempts)
                    {
                        fs = null;
                        return null;
                    }
                    else
                    {
                        Thread.Sleep(attemptWaitMS);
                    }
                }
            }
#pragma warning disable CS0162 // Unreachable code detected
            return fs;
#pragma warning restore CS0162 // Unreachable code detected
        }

        #endregion UGLY Wine Registry Code
    }
}