using Furcadia.Logging;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

// Programmer: Ludvik Jerabek
// Date: 08\23\2010
// Purpose: Allow INI manipulation in .NET

namespace Furcadia.IO
{
    /// <summary>
    /// IniFile class used to read and write ini files by loading the file into memory
    /// </summary>
    [CLSCompliant(true)]
    public class IniFile
    {
        #region "Private Fields"

        private string _code = "";
        // List of IniSection objects keeps track of all the sections in the
        // INI file

        private Hashtable m_sections = new Hashtable();

        #endregion "Private Fields"

        #region "Public Constructors"

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFile"/> class.
        /// </summary>
        public IniFile()
        {
            m_sections = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion "Public Constructors"

        #region "Public Properties"

        /// <summary>
        /// If a Code section is detected, It's the last section in the ini file and all
        /// lines are read into here.
        /// <para/>
        /// this is for DSC systyle wizard scripts
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Gets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public System.Collections.ICollection Sections
        {
            get { return m_sections.Values; }
        }

        #endregion "Public Properties"

        #region "Public Methods"

        /// <summary>
        /// Adds a section to the IniFile object, returns a IniSection object
        /// to the new or existing object
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <returns></returns>
        public IniSection AddSection(string sSection)
        {
            IniSection s = null;
            sSection = sSection.Trim();
            // Trim spaces
            if (m_sections.ContainsKey(sSection))
            {
                s = (IniSection)m_sections[sSection];
            }
            else
            {
                s = new IniSection(this, sSection);
                m_sections[sSection] = s;
            }
            return s;
        }

        /// <summary>
        /// Gets the key value.
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <param name="sKey">The s key.</param>
        /// <returns></returns>
        public string GetKeyValue(string sSection, string sKey)
        {
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                IniSection.IniKey k = s.GetKey(sKey);
                if (k != null)
                {
                    return k.Value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <returns></returns>
        public IniSection GetSection(string sSection)

        {
            sSection = sSection.Trim();
            // Trim spaces
            if (m_sections.ContainsKey(sSection))
            {
                return (IniSection)m_sections[sSection];
            }
            return null;
        }

        /// <summary>
        /// Loads the Reads the data in the ini file into the IniFile object
        /// </summary>
        /// <param name="sFileName">Name of the s file.</param>
        /// <param name="bMerge">if set to <c>fase</c>, remove all sections.</param>
        public void Load(string sFileName, bool bMerge = false)
        {
            if (!File.Exists(sFileName))
                throw new FileNotFoundException("Ini not fount", sFileName);
            if (!bMerge)
            {
                RemoveAllSections();
            }
            // Clear the object...
            IniSection tempsection = null;

            Regex regexcomment = new Regex("^([\\s]*#.*|;.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            // Broken but left for history
            //Dim regexsection As New Regex("\[[\s]*([^\[\s].*[^\s\]])[\s]*\]", (RegexOptions.Singleline Or RegexOptions.IgnoreCase))
            Regex regexsection = new Regex("^[\\s]*\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\][\\s]*$", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            Regex regexkey = new Regex("^\\s*([^=]*)[^=]*=\\s?\"?(.*)\"?", (RegexOptions.Singleline | RegexOptions.IgnoreCase));

            using (var oReader = new StreamReader(sFileName))
            {
                while (!oReader.EndOfStream)
                {
                    string line = oReader.ReadLine();
                    if (line != string.Empty)
                    {
                        Match m = null;
                        if (regexcomment.Match(line).Success)
                        {
                            m = regexcomment.Match(line);

                            Logger.Debug<IniFile>($"Skipping Comment: {m.Groups[0].Value}");
                        }
                        else if (regexsection.Match(line).Success)
                        {
                            m = regexsection.Match(line);

                            if (m.Groups[1].Value.ToLower() == "code")
                            {
                                Logger.Debug<IniFile>($"Copying Code Section [{m.Groups[1].Value}]");

                                _code = oReader.ReadToEnd();
                            }
                            else
                            {
                                Logger.Debug<IniFile>($"Adding section [{m.Groups[1].Value}]");

                                tempsection = AddSection(m.Groups[1].Value);
                            }
                        }
                        else if (regexkey.Match(line).Success && tempsection != null)
                        {
                            m = regexkey.Match(line);
                            Logger.Debug<IniFile>($"Adding Key [{m.Groups[1].Value}]=[{m.Groups[2].Value}]");
                            tempsection.AddKey(m.Groups[1].Value).Value = m.Groups[2].Value;
                        }
                        else if (tempsection != null)
                        {
                            // Handle Key without value

                            Logger.Debug<IniFile>($"Adding Key [{line}]");

                            tempsection.AddKey(line);
                        }
                        else
                        {
                            // This should not occur unless the tempsection is
                            // not created yet...

                            Logger.Debug<IniFile>($"Skipping unknown type of data: {line}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes all existing sections, returns trus on success
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllSections()
        {
            m_sections.Clear();
            return (m_sections.Count == 0);
        }

        /// <summary>
        /// Remove a key by section name and key name
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <param name="sKey">The s key.</param>
        /// <returns></returns>
        public bool RemoveKey(string sSection, string sKey)
        {
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                return s.RemoveKey(sKey);
            }
            return false;
        }

        /// <summary>
        /// Removes a section by its name sSection, returns trus on success
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <returns></returns>
        public bool RemoveSection(string sSection)
        {
            sSection = sSection.Trim();
            return RemoveSection(GetSection(sSection));
        }

        /// <summary>
        /// Removes section by object, returns trus on success
        /// </summary>
        /// <param name="Section">The section.</param>
        /// <returns></returns>
        public bool RemoveSection(IniSection Section)
        {
            if (Section != null)
            {
                try
                {
                    m_sections.Remove(Section.Name);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error<IniFile>(ex.Message);
                }
            }
            return false;
        }

        /// <summary>
        /// Renames an existing key returns true on success, false if the key
        /// didn't exist or there was another section with the same sNewKey
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <param name="sKey">The s key.</param>
        /// <param name="sNewKey">The s new key.</param>
        /// <returns></returns>
        public bool RenameKey(string sSection, string sKey, string sNewKey)
        {
            // Note string trims are done in lower calls.
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                IniSection.IniKey k = s.GetKey(sKey);
                if (k != null)
                {
                    return k.SetName(sNewKey);
                }
            }
            return false;
        }

        /// <summary>
        /// Renames an existing section returns true on success, false if the
        /// section didn't exist or there was another section with the same sNewSection
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <param name="sNewSection">The s new section.</param>
        /// <returns></returns>
        public bool RenameSection(string sSection, string sNewSection)
        {
            // Note string trims are done in lower calls.
            bool bRval = false;
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                bRval = s.SetName(sNewSection);
            }
            return bRval;
        }

        /// <summary>
        /// Used to save the data back to the file or your choice
        /// </summary>
        /// <param name="sFileName">Name of the s file.</param>
        public void Save(string sFileName)
        {
            using (var oWriter = new StreamWriter(sFileName, false))
            {
                foreach (IniSection s in Sections)
                {
                    Logger.Debug<IniFile>($"Writing Section: [{s.Name}]");

                    oWriter.WriteLine($"[{s.Name}]");
                    foreach (IniSection.IniKey k in s.Keys)
                    {
                        if (!string.IsNullOrWhiteSpace(k.Value))
                        {
                            Logger.Debug<IniFile>($"Writing Key: {k.Name}={k.Value}");

                            oWriter.WriteLine($"{k.Name}={k.Value}");
                        }
                        else
                        {
                            Logger.Debug<IniFile>($"Writing Key: {k.Name}");

                            oWriter.WriteLine($"Writing Key: {k.Name}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets a KeyValuePair in a certain section
        /// </summary>
        /// <param name="sSection">The s section.</param>
        /// <param name="sKey">The s key.</param>
        /// <param name="sValue">The s value.</param>
        /// <returns></returns>
        public bool SetKeyValue(string sSection, string sKey, string sValue)
        {
            IniSection s = AddSection(sSection);
            if (s != null)
            {
                IniSection.IniKey k = s.AddKey(sKey);
                if (k != null)
                {
                    k.Value = sValue;
                    return true;
                }
            }
            return false;
        }

        #endregion "Public Methods"

        #region "Public Classes"

        /// <summary>
        /// IniSection class
        /// </summary>
        public class IniSection
        {
            #region "Private Fields"

            // List of IniKeys in the section

            private Hashtable m_keys;

            // IniFile IniFile object instance
            private IniFile m_pIniFile;

            // Name of the section

            private string m_sSection;

            #endregion "Private Fields"

            #region "Protected Internal Constructors"

            /// <summary>
            /// Constuctor so objects are internally managed
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="sSection">The s section.</param>
            protected internal IniSection(IniFile parent, string sSection)
            {
                m_pIniFile = parent;
                m_sSection = sSection;
                m_keys = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            }

            #endregion "Protected Internal Constructors"

            #region "Public Properties"

            /// <summary>
            /// Returns all the keys in a section
            /// </summary>
            /// <value>
            /// The keys.
            /// </value>
            public System.Collections.ICollection Keys

            {
                get { return m_keys.Values; }
            }

            /// <summary>
            /// Returns the section name
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name
            {
                get { return m_sSection; }
            }

            #endregion "Public Properties"

            #region "Public Methods"

            /// <summary>
            /// Adds a key to the IniSection object, returns a IniKey object
            /// to the new or existing object
            /// </summary>
            /// <param name="sKey">The s key.</param>
            /// <returns></returns>
            public IniKey AddKey(string sKey)
            {
                sKey = sKey.Trim();
                IniKey k = null;
                if (sKey.Length != 0)
                {
                    if (m_keys.ContainsKey(sKey))
                    {
                        k = (IniKey)m_keys[sKey];
                    }
                    else
                    {
                        k = new IniKey(this, sKey);
                        m_keys[sKey] = k;
                    }
                }
                return k;
            }

            /// <summary>
            ///  Returns a IniKey object to the key by name, NULL if it was
            ///  not found
            /// </summary>
            /// <param name="sKey">The s key.</param>
            /// <returns></returns>
            public IniKey GetKey(string sKey)
            {
                sKey = sKey.Trim();
                if (m_keys.ContainsKey(sKey))
                {
                    return (IniKey)m_keys[sKey];
                }
                return null;
            }

            /// <summary>
            /// Returns the section name
            /// </summary>
            /// <returns></returns>
            public string GetName()
            {
                return m_sSection;
            }

            /// <summary>
            /// Removes all the keys in the section
            /// </summary>
            /// <returns></returns>
            public bool RemoveAllKeys()
            {
                m_keys.Clear();
                return (m_keys.Count == 0);
            }

            /// <summary>
            /// Removes all the keys in the section
            /// </summary>
            /// <param name="sKey">The s key.</param>
            /// <returns></returns>
            public bool RemoveKey(string sKey)
            {
                return RemoveKey(GetKey(sKey));
            }

            /// <summary>
            /// Removes a single key by IniKey object
            /// </summary>
            /// <param name="Key">The key.</param>
            /// <returns></returns>
            public bool RemoveKey(IniKey Key)
            {
                if (Key != null)
                {
                    try
                    {
                        m_keys.Remove(Key.Name);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error<IniSection>(ex.Message);
                    }
                }
                return false;
            }

            /// <summary>
            /// Sets the section name, returns true on success, fails if the
            /// section name sSection already exists
            /// </summary>
            /// <param name="sSection">The s section.</param>
            /// <returns></returns>
            public bool SetName(string sSection)
            {
                sSection = sSection.Trim();
                if (sSection.Length != 0)
                {
                    // Get existing section if it even exists...
                    IniSection s = m_pIniFile.GetSection(sSection);
                    if (!ReferenceEquals(s, this) && s != null)
                    {
                        return false;
                    }
                    try
                    {
                        // Remove the current section
                        m_pIniFile.m_sections.Remove(m_sSection);
                        // Set the new section name to this object
                        m_pIniFile.m_sections[sSection] = this;
                        // Set the new section name
                        m_sSection = sSection;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error<IniSection>(ex.Message);
                    }
                }
                return false;
            }

            #endregion "Public Methods"

            #region "Public Classes"

            /// <summary>
            /// IniKey class
            /// </summary>
            public class IniKey
            {
                #region "Private Fields"

                // Pointer to the parent CIniSection

                private IniSection m_section;

                // Name of the Key
                private string m_sKey;

                // Value associated

                private string m_sValue;

                #endregion "Private Fields"

                #region "Protected Internal Constructors"

                /// <summary>
                /// Constuctor so objects are internally managed
                /// </summary>
                /// <param name="parent">The parent.</param>
                /// <param name="sKey">The s key.</param>
                protected internal IniKey(IniSection parent, string sKey)
                {
                    m_section = parent;
                    m_sKey = sKey;
                }

                #endregion "Protected Internal Constructors"

                #region "Public Properties"

                /// <summary>
                /// Returns the name of the Key
                /// </summary>
                /// <value>
                /// The name.
                /// </value>
                public string Name
                {
                    get { return m_sKey; }
                }

                /// <summary>
                /// Sets or Gets the value of the key
                /// </summary>
                /// <value>
                /// The value.
                /// </value>
                public string Value
                {
                    get { return m_sValue; }
                    set { m_sValue = value; }
                }

                #endregion "Public Properties"

                #region "Public Methods"

                /// <summary>
                /// Returns the name of the Key
                /// </summary>
                /// <returns></returns>
                public string GetName()
                {
                    return m_sKey;
                }

                /// <summary>
                /// Returns the value of the Key
                /// </summary>
                /// <returns></returns>
                public string GetValue()
                {
                    return m_sValue;
                }

                /// <summary>
                /// Sets the key name Returns true on success, fails if the
                /// section name sKey already exists
                /// </summary>
                /// <param name="sKey">The s key.</param>
                /// <returns></returns>
                public bool SetName(string sKey)
                {
                    sKey = sKey.Trim();
                    if (sKey.Length != 0)
                    {
                        IniKey k = m_section.GetKey(sKey);
                        if (!ReferenceEquals(k, this) && k != null)
                        {
                            return false;
                        }
                        try
                        {
                            // Remove the current key
                            m_section.m_keys.Remove(m_sKey);
                            // Set the new key name to this object
                            m_section.m_keys[sKey] = this;
                            // Set the new key name
                            m_sKey = sKey;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error<IniSection>(ex.Message);
                        }
                    }
                    return false;
                }

                /// <summary>
                /// Sets the value of the key
                /// </summary>
                /// <param name="sValue">The s value.</param>
                public void SetValue(string sValue)
                {
                    m_sValue = sValue;
                }

                #endregion "Public Methods"
            }

            #endregion "Public Classes"

            // End of IniKey class
        }

        #endregion "Public Classes"

        // End of IniSection class
    }
}

// End of IniFile class