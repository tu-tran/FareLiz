namespace SkyDean.FareLiz.Data.Config
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;

    // IniConfig class used to read and write ini files by loading the file into memory
    /// <summary>The ini config.</summary>
    public class IniConfig
    {
        // List of IniSection objects keeps track of all the sections in the INI file
        /// <summary>The m_sections.</summary>
        private readonly Hashtable m_sections;

        // Public constructor
        /// <summary>Initializes a new instance of the <see cref="IniConfig" /> class.</summary>
        public IniConfig()
        {
            this.m_sections = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        }

        // Gets all the sections names
        /// <summary>Gets the sections.</summary>
        public ICollection Sections
        {
            get
            {
                return this.m_sections.Values;
            }
        }

        // Loads the Reads the data in the ini file into the IniConfig object
        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="sFileName">
        /// The s file name.
        /// </param>
        public void Load(string sFileName)
        {
            this.Load(sFileName, false);
        }

        // Loads the Reads the data in the ini file into the IniConfig object
        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="sFileName">
        /// The s file name.
        /// </param>
        /// <param name="bMerge">
        /// The b merge.
        /// </param>
        public void Load(string sFileName, bool bMerge)
        {
            if (!bMerge)
            {
                this.RemoveAllSections();
            }

            // Clear the object... 
            IniSection tempsection = null;
            using (var oReader = new StreamReader(sFileName))
            {
                var regexcomment = new Regex("^([\\s]*#.*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // ^[\\s]*\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\][\\s]*$
                var regexsection = new Regex(
                    "^[\\s]*\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\][\\s]*$", 
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);

                // Regex regexsection = new Regex("\\[[\\s]*([^\\[\\s].*[^\\s\\]])[\\s]*\\]", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
                var regexkey = new Regex("^\\s*([^=\\s]*)[^=]*=(.*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                while (!oReader.EndOfStream)
                {
                    var line = oReader.ReadLine();
                    if (line != string.Empty)
                    {
                        Match m = null;
                        if (regexcomment.Match(line).Success)
                        {
                            m = regexcomment.Match(line);
                            Trace.WriteLine(string.Format("Skipping Comment: {0}", m.Groups[0].Value));
                        }
                        else if (regexsection.Match(line).Success)
                        {
                            m = regexsection.Match(line);
                            Trace.WriteLine(string.Format("Adding section [{0}]", m.Groups[1].Value));
                            tempsection = this.AddSection(m.Groups[1].Value);
                        }
                        else if (regexkey.Match(line).Success && tempsection != null)
                        {
                            m = regexkey.Match(line);
                            Trace.WriteLine(string.Format("Adding configured type [{0}]=[{1}]", m.Groups[1].Value, m.Groups[2].Value));
                            tempsection.AddKey(m.Groups[1].Value).Value = m.Groups[2].Value;
                        }
                        else if (tempsection != null)
                        {
                            // Handle ConfiguredType without value
                            Trace.WriteLine(string.Format("Adding ConfiguredType [{0}]", line));
                            tempsection.AddKey(line);
                        }
                        else
                        {
                            // This should not occur unless the tempsection is not created yet...
                            Trace.WriteLine(string.Format("Skipping unknown type of data: {0}", line));
                        }
                    }
                }
            }
        }

        // Used to save the data back to the file or your choice
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="sFileName">
        /// The s file name.
        /// </param>
        public void Save(string sFileName)
        {
            using (var oWriter = new StreamWriter(sFileName, false))
            {
                foreach (IniSection s in this.Sections)
                {
                    Trace.WriteLine(string.Format("Writing Section: [{0}]", s.Name));
                    oWriter.WriteLine("[{0}]", s.Name);
                    foreach (IniSection.IniKey k in s.Keys)
                    {
                        if (k.Value != string.Empty)
                        {
                            Trace.WriteLine(string.Format("Writing ConfiguredType: {0}={1}", k.Name, k.Value));
                            oWriter.WriteLine("{0}={1}", k.Name, k.Value);
                        }
                        else
                        {
                            Trace.WriteLine(string.Format("Writing ConfiguredType: {0}", k.Name));
                            oWriter.WriteLine("{0}", k.Name);
                        }
                    }
                }
            }
        }

        // Adds a section to the IniConfig object, returns a IniSection object to the new or existing object
        /// <summary>
        /// The add section.
        /// </summary>
        /// <param name="sSection">
        /// The s section.
        /// </param>
        /// <returns>
        /// The <see cref="IniSection"/>.
        /// </returns>
        public IniSection AddSection(string sSection)
        {
            IniSection s = null;
            sSection = sSection.Trim();

            // Trim spaces
            if (this.m_sections.ContainsKey(sSection))
            {
                s = (IniSection)this.m_sections[sSection];
            }
            else
            {
                s = new IniSection(this, sSection);
                this.m_sections[sSection] = s;
            }

            return s;
        }

        // Removes a section by its name sSection, returns trus on success
        /// <summary>
        /// The remove section.
        /// </summary>
        /// <param name="sSection">
        /// The s section.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveSection(string sSection)
        {
            sSection = sSection.Trim();
            return this.RemoveSection(this.GetSection(sSection));
        }

        // Removes section by object, returns trus on success
        /// <summary>
        /// The remove section.
        /// </summary>
        /// <param name="Section">
        /// The section.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveSection(IniSection Section)
        {
            if (Section != null)
            {
                try
                {
                    this.m_sections.Remove(Section.Name);
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }

            return false;
        }

        // Removes all existing sections, returns trus on success
        /// <summary>The remove all sections.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool RemoveAllSections()
        {
            this.m_sections.Clear();
            return this.m_sections.Count == 0;
        }

        // Returns an IniSection to the section by name, NULL if it was not found
        /// <summary>
        /// The get section.
        /// </summary>
        /// <param name="sSection">
        /// The s section.
        /// </param>
        /// <returns>
        /// The <see cref="IniSection"/>.
        /// </returns>
        public IniSection GetSection(string sSection)
        {
            sSection = sSection.Trim();

            // Trim spaces
            if (this.m_sections.ContainsKey(sSection))
            {
                return (IniSection)this.m_sections[sSection];
            }

            return null;
        }

        // Returns a KeyValue in a certain section
        /// <summary>
        /// The get key value.
        /// </summary>
        /// <param name="sSection">
        /// The s section.
        /// </param>
        /// <param name="sKey">
        /// The s key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetKeyValue(string sSection, string sKey)
        {
            var s = this.GetSection(sSection);
            if (s != null)
            {
                var k = s.GetKey(sKey);
                if (k != null)
                {
                    return k.Value;
                }
            }

            return string.Empty;
        }

        // Sets a KeyValuePair in a certain section
        /// <summary>
        /// The set key value.
        /// </summary>
        /// <param name="sSection">
        /// The s section.
        /// </param>
        /// <param name="sKey">
        /// The s key.
        /// </param>
        /// <param name="sValue">
        /// The s value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetKeyValue(string sSection, string sKey, string sValue)
        {
            var s = this.AddSection(sSection);
            if (s != null)
            {
                var k = s.AddKey(sKey);
                if (k != null)
                {
                    k.Value = sValue;
                    return true;
                }
            }

            return false;
        }

        // Renames an existing section returns true on success, false if the section didn't exist or there was another section with the same sNewSection
        /// <summary>
        /// The rename section.
        /// </summary>
        /// <param name="sSection">
        /// The s section.
        /// </param>
        /// <param name="sNewSection">
        /// The s new section.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RenameSection(string sSection, string sNewSection)
        {
            // Note string trims are done in lower calls.
            var bRval = false;
            var s = this.GetSection(sSection);
            if (s != null)
            {
                bRval = s.SetName(sNewSection);
            }

            return bRval;
        }

        // Renames an existing key returns true on success, false if the key didn't exist or there was another section with the same sNewKey
        /// <summary>
        /// The rename key.
        /// </summary>
        /// <param name="sSection">
        /// The s section.
        /// </param>
        /// <param name="sKey">
        /// The s key.
        /// </param>
        /// <param name="sNewKey">
        /// The s new key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RenameKey(string sSection, string sKey, string sNewKey)
        {
            // Note string trims are done in lower calls.
            var s = this.GetSection(sSection);
            if (s != null)
            {
                var k = s.GetKey(sKey);
                if (k != null)
                {
                    return k.SetName(sNewKey);
                }
            }

            return false;
        }

        // IniSection class 
        /// <summary>The ini section.</summary>
        public class IniSection
        {
            /// <summary>The m_keys.</summary>
            private readonly Hashtable m_keys; // List of IniKeys in the section

            /// <summary>The m_p ini config.</summary>
            private readonly IniConfig m_pIniConfig;

            /// <summary>The m_s section.</summary>
            private string m_sSection; // Name of the section

            /// <summary>
            /// Initializes a new instance of the <see cref="IniSection"/> class.
            /// </summary>
            /// <param name="parent">
            /// The parent.
            /// </param>
            /// <param name="sSection">
            /// The s section.
            /// </param>
            internal IniSection(IniConfig parent, string sSection)
            {
                this.m_pIniConfig = parent;
                this.m_sSection = sSection;
                this.m_keys = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
            }

            /// <summary>Gets the keys.</summary>
            public ICollection Keys
            {
                get
                {
                    return this.m_keys.Values;
                }
            } // Returns and hashtable of keys associated with the section

            /// <summary>Gets the name.</summary>
            public string Name
            {
                get
                {
                    return this.m_sSection;
                }
            } // Returns the section name
            // Adds a key to the IniSection object, returns a IniKey object to the new or existing object
            /// <summary>
            /// The add key.
            /// </summary>
            /// <param name="sKey">
            /// The s key.
            /// </param>
            /// <returns>
            /// The <see cref="IniKey"/>.
            /// </returns>
            public IniKey AddKey(string sKey)
            {
                sKey = sKey.Trim();
                IniKey k = null;
                if (sKey.Length != 0)
                {
                    if (this.m_keys.ContainsKey(sKey))
                    {
                        k = (IniKey)this.m_keys[sKey];
                    }
                    else
                    {
                        k = new IniKey(this, sKey);
                        this.m_keys[sKey] = k;
                    }
                }

                return k;
            }

            // Removes a single key by string
            /// <summary>
            /// The remove key.
            /// </summary>
            /// <param name="sKey">
            /// The s key.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public bool RemoveKey(string sKey)
            {
                return this.RemoveKey(this.GetKey(sKey));
            }

            // Removes a single key by IniKey object
            /// <summary>
            /// The remove key.
            /// </summary>
            /// <param name="Key">
            /// The key.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public bool RemoveKey(IniKey Key)
            {
                if (Key != null)
                {
                    try
                    {
                        this.m_keys.Remove(Key.Name);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }

                return false;
            }

            // Removes all the keys in the section
            /// <summary>The remove all keys.</summary>
            /// <returns>The <see cref="bool" />.</returns>
            public bool RemoveAllKeys()
            {
                this.m_keys.Clear();
                return this.m_keys.Count == 0;
            }

            // Returns a IniKey object to the key by name, NULL if it was not found
            /// <summary>
            /// The get key.
            /// </summary>
            /// <param name="sKey">
            /// The s key.
            /// </param>
            /// <returns>
            /// The <see cref="IniKey"/>.
            /// </returns>
            public IniKey GetKey(string sKey)
            {
                sKey = sKey.Trim();
                if (this.m_keys.ContainsKey(sKey))
                {
                    return (IniKey)this.m_keys[sKey];
                }

                return null;
            }

            // Sets the section name, returns true on success, fails if the section
            // name sSection already exists
            /// <summary>
            /// The set name.
            /// </summary>
            /// <param name="sSection">
            /// The s section.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            public bool SetName(string sSection)
            {
                sSection = sSection.Trim();
                if (sSection.Length != 0)
                {
                    // Get existing section if it even exists...
                    var s = this.m_pIniConfig.GetSection(sSection);
                    if (s != this && s != null)
                    {
                        return false;
                    }

                    try
                    {
                        // Remove the current section
                        this.m_pIniConfig.m_sections.Remove(this.m_sSection);

                        // Set the new section name to this object
                        this.m_pIniConfig.m_sections[sSection] = this;

                        // Set the new section name
                        this.m_sSection = sSection;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }

                return false;
            }

            // Returns the section name
            /// <summary>The get name.</summary>
            /// <returns>The <see cref="string" />.</returns>
            public string GetName()
            {
                return this.m_sSection;
            }

            // IniKey class
            /// <summary>The ini key.</summary>
            public class IniKey
            {
                // Pointer to the parent CIniSection
                /// <summary>The m_section.</summary>
                private readonly IniSection m_section;

                // Name of the ConfiguredType
                /// <summary>The m_s key.</summary>
                private string m_sKey;

                // TypeConfiguration associated
                /// <summary>The m_s value.</summary>
                private string m_sValue;

                /// <summary>
                /// Initializes a new instance of the <see cref="IniKey"/> class.
                /// </summary>
                /// <param name="parent">
                /// The parent.
                /// </param>
                /// <param name="sKey">
                /// The s key.
                /// </param>
                public IniKey(IniSection parent, string sKey)
                {
                    this.m_section = parent;
                    this.m_sKey = sKey;
                }

                // Returns the name of the ConfiguredType
                /// <summary>Gets the name.</summary>
                public string Name
                {
                    get
                    {
                        return this.m_sKey;
                    }
                }

                // Sets or Gets the value of the key
                /// <summary>Gets or sets the value.</summary>
                public string Value
                {
                    get
                    {
                        return this.m_sValue;
                    }

                    set
                    {
                        this.m_sValue = value;
                    }
                }

                // Sets the value of the key
                /// <summary>
                /// The set value.
                /// </summary>
                /// <param name="sValue">
                /// The s value.
                /// </param>
                public void SetValue(string sValue)
                {
                    this.m_sValue = sValue;
                }

                // Returns the value of the ConfiguredType
                /// <summary>The get value.</summary>
                /// <returns>The <see cref="string" />.</returns>
                public string GetValue()
                {
                    return this.m_sValue;
                }

                // Sets the key name
                // Returns true on success, fails if the section name sKey already exists
                /// <summary>
                /// The set name.
                /// </summary>
                /// <param name="sKey">
                /// The s key.
                /// </param>
                /// <returns>
                /// The <see cref="bool"/>.
                /// </returns>
                public bool SetName(string sKey)
                {
                    sKey = sKey.Trim();
                    if (sKey.Length != 0)
                    {
                        var k = this.m_section.GetKey(sKey);
                        if (k != this && k != null)
                        {
                            return false;
                        }

                        try
                        {
                            // Remove the current key
                            this.m_section.m_keys.Remove(this.m_sKey);

                            // Set the new key name to this object
                            this.m_section.m_keys[sKey] = this;

                            // Set the new key name
                            this.m_sKey = sKey;
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message);
                        }
                    }

                    return false;
                }

                // Returns the name of the ConfiguredType
                /// <summary>The get name.</summary>
                /// <returns>The <see cref="string" />.</returns>
                public string GetName()
                {
                    return this.m_sKey;
                }
            } // End of IniKey class
        } // End of IniSection class
    } // End of IniConfig class
}