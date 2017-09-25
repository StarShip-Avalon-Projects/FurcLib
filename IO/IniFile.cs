using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

// Programmer: Ludvik Jerabek
// Date: 08\23\2010
// Purpose: Allow INI manipulation in .NET

// IniFile class used to read and write ini files by loading the file into memory
namespace Furcadia.IO
{
    /// <summary>
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

        // Public constructor
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniFile()'

        public IniFile()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniFile()'
        {
            m_sections = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion "Public Constructors"

        #region "Public Properties"

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Code'

        public string Code
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Code'
        {
            get { return _code; }
        }

        // Gets all the sections
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Sections'

        public System.Collections.ICollection Sections
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Sections'
        {
            get { return m_sections.Values; }
        }

        #endregion "Public Properties"

        #region "Public Methods"

        // Adds a section to the IniFile object, returns a IniSection object
        // to the new or existing object
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.AddSection(string)'

        public IniSection AddSection(string sSection)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.AddSection(string)'
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

        // Returns a KeyValue in a certain section
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.GetKeyValue(string, string)'

        public string GetKeyValue(string sSection, string sKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.GetKeyValue(string, string)'
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

        // Returns an IniSection to the section by name, NULL if it was not found
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.GetSection(string)'

        public IniSection GetSection(string sSection)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.GetSection(string)'
        {
            sSection = sSection.Trim();
            // Trim spaces
            if (m_sections.ContainsKey(sSection))
            {
                return (IniSection)m_sections[sSection];
            }
            return null;
        }

        // Loads the Reads the data in the ini file into the IniFile object
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Load(string, bool)'

        public void Load(string sFileName, bool bMerge = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Load(string, bool)'
        {
            if (!bMerge)
            {
                RemoveAllSections();
            }
            // Clear the object...
            IniSection tempsection = null;
            StreamReader oReader = new StreamReader(sFileName);
            Regex regexcomment = new Regex("^([\\s]*#.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            // Broken but left for history
            //Dim regexsection As New Regex("\[[\s]*([^\[\s].*[^\s\]])[\s]*\]", (RegexOptions.Singleline Or RegexOptions.IgnoreCase))
            Regex regexsection = new Regex("^[\\s]*\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\][\\s]*$", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            Regex regexkey = new Regex("^\\s*([^=]*)[^=]*=(.*)", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            while (!oReader.EndOfStream)
            {
                string line = oReader.ReadLine();
                if (line != string.Empty)
                {
                    Match m = null;
                    if (regexcomment.Match(line).Success)
                    {
                        m = regexcomment.Match(line);

                        Debug.WriteLine(string.Format("Skipping Comment: {0}", m.Groups[0].Value));
                    }
                    else if (regexsection.Match(line).Success)
                    {
                        m = regexsection.Match(line);

                        if (m.Groups[1].Value.ToLower() == "code")
                        {
                            Debug.WriteLine(string.Format("Copying Code Section [{0}]", m.Groups[1].Value));

                            _code = oReader.ReadToEnd();
                        }
                        else
                        {
                            Debug.WriteLine(string.Format("Adding section [{0}]", m.Groups[1].Value));

                            tempsection = AddSection(m.Groups[1].Value);
                        }
                    }
                    else if (regexkey.Match(line).Success && tempsection != null)
                    {
                        m = regexkey.Match(line);

                        Debug.WriteLine(string.Format("Adding Key [{0}]=[{1}]", m.Groups[1].Value, m.Groups[2].Value));
                        tempsection.AddKey(m.Groups[2].Value).Value = m.Groups[2].Value;
                    }
                    else if (tempsection != null)
                    {
                        // Handle Key without value

                        Debug.WriteLine(string.Format("Adding Key [{0}]", line));

                        tempsection.AddKey(line);
                    }
                    else
                    {
                        // This should not occur unless the tempsection is
                        // not created yet...

                        Debug.WriteLine(string.Format("Skipping unknown type of data: {0}", line));
                    }
                }
            }
            oReader.Close();
        }

        // Removes all existing sections, returns trus on success
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveAllSections()'

        public bool RemoveAllSections()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveAllSections()'
        {
            m_sections.Clear();
            return (m_sections.Count == 0);
        }

        // Remove a key by section name and key name
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveKey(string, string)'

        public bool RemoveKey(string sSection, string sKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveKey(string, string)'
        {
            IniSection s = GetSection(sSection);
            if (s != null)
            {
                return s.RemoveKey(sKey);
            }
            return false;
        }

        // Removes a section by its name sSection, returns trus on success
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveSection(string)'

        public bool RemoveSection(string sSection)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveSection(string)'
        {
            sSection = sSection.Trim();
            return RemoveSection(GetSection(sSection));
        }

        // Removes section by object, returns trus on success
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveSection(IniFile.IniSection)'

        public bool RemoveSection(IniSection Section)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RemoveSection(IniFile.IniSection)'
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
                    Debug.WriteLine(ex.Message);
                }
            }
            return false;
        }

        // Renames an existing key returns true on success, false if the key
        // didn't exist or there was another section with the same sNewKey
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RenameKey(string, string, string)'

        public bool RenameKey(string sSection, string sKey, string sNewKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RenameKey(string, string, string)'
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

        // Renames an existing section returns true on success, false if the
        // section didn't exist or there was another section with the same sNewSection
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RenameSection(string, string)'

        public bool RenameSection(string sSection, string sNewSection)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.RenameSection(string, string)'
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

        // Used to save the data back to the file or your choice
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Save(string)'

        public void Save(string sFileName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.Save(string)'
        {
            StreamWriter oWriter = new StreamWriter(sFileName, false);

            foreach (IniSection s in Sections)
            {
                Debug.WriteLine(string.Format("Writing Section: [{0}]", s.Name));

                oWriter.WriteLine(string.Format("[{0}]", s.Name));
                foreach (IniSection.IniKey k in s.Keys)
                {
                    if (k.Value != string.Empty)
                    {
                        Debug.WriteLine(string.Format("Writing Key: {0}={1}", k.Name, k.Value));

                        oWriter.WriteLine(string.Format("{0}={1}", k.Name, k.Value));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Writing Key: {0}", k.Name));

                        oWriter.WriteLine(string.Format("{0}", k.Name));
                    }
                }
            }
            oWriter.Close();
        }

        // Sets a KeyValuePair in a certain section
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.SetKeyValue(string, string, string)'

        public bool SetKeyValue(string sSection, string sKey, string sValue)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.SetKeyValue(string, string, string)'
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

        // IniSection class
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection'

        public class IniSection
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection'
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

            // Constuctor so objects are internally managed
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniSection(IniFile, string)'

            protected internal IniSection(IniFile parent, string sSection)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniSection(IniFile, string)'
            {
                m_pIniFile = parent;
                m_sSection = sSection;
                m_keys = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            }

            #endregion "Protected Internal Constructors"

            #region "Public Properties"

            // Returns all the keys in a section
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.Keys'

            public System.Collections.ICollection Keys
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.Keys'
            {
                get { return m_keys.Values; }
            }

            // Returns the section name
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.Name'

            public string Name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.Name'
            {
                get { return m_sSection; }
            }

            #endregion "Public Properties"

            #region "Public Methods"

            // Adds a key to the IniSection object, returns a IniKey object
            // to the new or existing object
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.AddKey(string)'

            public IniKey AddKey(string sKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.AddKey(string)'
            {
                sKey = sKey.Trim();
                IniSection.IniKey k = null;
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

            // Returns a IniKey object to the key by name, NULL if it was
            // not found
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.GetKey(string)'

            public IniKey GetKey(string sKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.GetKey(string)'
            {
                sKey = sKey.Trim();
                if (m_keys.ContainsKey(sKey))
                {
                    return (IniKey)m_keys[sKey];
                }
                return null;
            }

            // Returns the section name
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.GetName()'

            public string GetName()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.GetName()'
            {
                return m_sSection;
            }

            // Removes all the keys in the section
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.RemoveAllKeys()'

            public bool RemoveAllKeys()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.RemoveAllKeys()'
            {
                m_keys.Clear();
                return (m_keys.Count == 0);
            }

            // Removes a single key by string
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.RemoveKey(string)'

            public bool RemoveKey(string sKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.RemoveKey(string)'
            {
                return RemoveKey(GetKey(sKey));
            }

            // Removes a single key by IniKey object
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.RemoveKey(IniFile.IniSection.IniKey)'

            public bool RemoveKey(IniKey Key)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.RemoveKey(IniFile.IniSection.IniKey)'
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
                        Debug.WriteLine(ex.Message);
                    }
                }
                return false;
            }

            // Sets the section name, returns true on success, fails if the
            // section name sSection already exists
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.SetName(string)'

            public bool SetName(string sSection)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.SetName(string)'
            {
                sSection = sSection.Trim();
                if (sSection.Length != 0)
                {
                    // Get existing section if it even exists...
                    IniSection s = m_pIniFile.GetSection(sSection);
                    if (!object.ReferenceEquals(s, this) && s != null)
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
                        Debug.WriteLine(ex.Message);
                    }
                }
                return false;
            }

            #endregion "Public Methods"

            #region "Public Classes"

            // IniKey class
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey'

            public class IniKey
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey'
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

                // Constuctor so objects are internally managed
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.IniKey(IniFile.IniSection, string)'

                protected internal IniKey(IniSection parent, string sKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.IniKey(IniFile.IniSection, string)'
                {
                    m_section = parent;
                    m_sKey = sKey;
                }

                #endregion "Protected Internal Constructors"

                #region "Public Properties"

                // Returns the name of the Key
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.Name'

                public string Name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.Name'
                {
                    get { return m_sKey; }
                }

                // Sets or Gets the value of the key
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.Value'

                public string Value
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.Value'
                {
                    get { return m_sValue; }
                    set { m_sValue = value; }
                }

                #endregion "Public Properties"

                #region "Public Methods"

                // Returns the name of the Key
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.GetName()'

                public string GetName()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.GetName()'
                {
                    return m_sKey;
                }

                // Returns the value of the Key
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.GetValue()'

                public string GetValue()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.GetValue()'
                {
                    return m_sValue;
                }

                // Sets the key name Returns true on success, fails if the
                // section name sKey already exists
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.SetName(string)'

                public bool SetName(string sKey)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.SetName(string)'
                {
                    sKey = sKey.Trim();
                    if (sKey.Length != 0)
                    {
                        IniKey k = m_section.GetKey(sKey);
                        if (!object.ReferenceEquals(k, this) && k != null)
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
                            Debug.WriteLine(ex.Message);
                        }
                    }
                    return false;
                }

                // Sets the value of the key
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.SetValue(string)'

                public void SetValue(string sValue)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IniFile.IniSection.IniKey.SetValue(string)'
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