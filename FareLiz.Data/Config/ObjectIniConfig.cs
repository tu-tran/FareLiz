namespace SkyDean.FareLiz.Data.Config
{
    using SkyDean.FareLiz.Core.Utils;
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>Class for storing object data in INI Config file</summary>
    public class ObjectIniConfig : IniConfig, IObjectPersist
    {
        /// <summary>
        /// The bind flgas.
        /// </summary>
        private BindingFlags bindFlgas = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIniConfig"/> class.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public ObjectIniConfig(string filePath, ILogger logger)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("INI Configuration filename cannot be empty");
            }

            this.INIFilePath = filePath;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets or sets the ini file path.
        /// </summary>
        public string INIFilePath { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The apply data.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        public void ApplyData(object targetObject)
        {
            if (this.Sections.Count < 1)
            {
                return;
            }

            var type = targetObject.GetType();
            var section = this.GetSection(type.FullName);
            if (section == null)
            {
                return;
            }

            var properties = type.GetPropertiesRecursively(this.bindFlgas);

            foreach (var prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var attributes = prop.GetCustomAttributes(true);
                    foreach (var attrib in attributes)
                    {
                        var configAttrib = attrib as IniConfigurableAttribute;
                        if (configAttrib != null)
                        {
                            if (configAttrib.IsConfigurable)
                            {
                                string propKey = string.IsNullOrEmpty(configAttrib.ConfigurationKey) ? prop.Name : configAttrib.ConfigurationKey;
                                string storedValue = this.GetKeyValue(section.Name, propKey);

                                try
                                {
                                    object storedObj = null;
                                    if (!IniConfigHandler.FromString(storedValue, prop.PropertyType, out storedObj))
                                    {
                                        Convert.ChangeType(storedValue, prop.PropertyType);
                                    }

                                    prop.SetValue(targetObject, storedObj, null);
                                }
                                catch (Exception ex)
                                {
                                    this.Logger.WarnFormat(
                                        "Failed to load INI config [{0}][{1}] [{2}][{3}]: {4}",
                                        type.Name,
                                        prop.Name,
                                        prop.PropertyType.Name,
                                        storedValue,
                                        ex.Message);
                                }
                            }

                            break; // No need to look further attributes
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The save data.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        public void SaveData(object targetObject)
        {
            this.RemoveAllSections();

            var type = targetObject.GetType();
            var section = this.GetSection(type.FullName);
            if (section == null)
            {
                section = this.AddSection(type.FullName);
            }

            var properties = type.GetPropertiesRecursively(this.bindFlgas);

            foreach (var prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var attributes = prop.GetCustomAttributes(true);
                    foreach (var attrib in attributes)
                    {
                        var configAttrib = attrib as IniConfigurableAttribute;
                        if (configAttrib != null)
                        {
                            if (configAttrib.IsConfigurable)
                            {
                                string keyStr = string.IsNullOrEmpty(configAttrib.ConfigurationKey) ? prop.Name : configAttrib.ConfigurationKey;
                                var curKey = section.GetKey(keyStr);
                                if (curKey == null)
                                {
                                    curKey = section.AddKey(keyStr);
                                }

                                object val = prop.GetValue(targetObject, null);
                                if (val != null)
                                {
                                    string exportedStr = null;
                                    if (!IniConfigHandler.ToStorageString(val, out exportedStr))
                                    {
                                        exportedStr = Convert.ChangeType(val, typeof(string)) as string;
                                    }

                                    curKey.Value = exportedStr;
                                }
                            }

                            break; // No need to look further attributes
                        }
                    }
                }
            }

            this.Save(this.INIFilePath);
        }

        /// <summary>
        /// The load ini.
        /// </summary>
        public void LoadINI()
        {
            this.RemoveAllSections();
            if (File.Exists(this.INIFilePath))
            {
                this.Load(this.INIFilePath);
            }
        }
    }
}