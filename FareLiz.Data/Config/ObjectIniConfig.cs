using System;
using System.IO;
using System.Reflection;
using log4net;
using SkyDean.FareLiz.Core.Utils;

namespace SkyDean.FareLiz.Data.Config
{
    /// <summary>
    /// Class for storing object data in INI Config file
    /// </summary>
    public class ObjectIniConfig : IniConfig, IObjectPersist
    {
        private BindingFlags bindFlgas = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public ILog Logger { get; set; }
        public string INIFilePath { get; set; }

        public ObjectIniConfig(string filePath, ILog logger)
            : base()
        {
            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentException("INI Configuration filename cannot be empty");
            INIFilePath = filePath;
            Logger = logger;
        }

        public void LoadINI()
        {
            RemoveAllSections();
            if (File.Exists(INIFilePath))
                Load(INIFilePath);
        }

        public void ApplyData(object targetObject)
        {
            if (Sections.Count < 1)
                return;

            var type = targetObject.GetType();
            var section = GetSection(type.FullName);
            if (section == null)
                return;

            var properties = type.GetPropertiesRecursively(bindFlgas);

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
                                string propKey = String.IsNullOrEmpty(configAttrib.ConfigurationKey) ? prop.Name : configAttrib.ConfigurationKey;
                                string storedValue = GetKeyValue(section.Name, propKey);

                                try
                                {
                                    object storedObj = null;
                                    if (!IniConfigHandler.FromString(storedValue, prop.PropertyType, out storedObj))
                                        Convert.ChangeType(storedValue, prop.PropertyType);
                                    prop.SetValue(targetObject, storedObj, null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.WarnFormat("Failed to load INI config [{0}][{1}] [{2}][{3}]: {4}",
                                        type.Name, prop.Name, prop.PropertyType.Name, storedValue, ex.Message);
                                }
                            }
                            break;  // No need to look further attributes
                        }
                    }
                }
            }
        }

        public void SaveData(object targetObject)
        {
            RemoveAllSections();

            var type = targetObject.GetType();
            var section = GetSection(type.FullName);
            if (section == null)
                section = AddSection(type.FullName);

            var properties = type.GetPropertiesRecursively(bindFlgas);

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
                                string keyStr = String.IsNullOrEmpty(configAttrib.ConfigurationKey) ? prop.Name : configAttrib.ConfigurationKey;
                                var curKey = section.GetKey(keyStr);
                                if (curKey == null)
                                    curKey = section.AddKey(keyStr);

                                object val = prop.GetValue(targetObject, null);
                                if (val != null)
                                {
                                    string exportedStr = null;
                                    if (!IniConfigHandler.ToStorageString(val, out exportedStr))
                                        exportedStr = Convert.ChangeType(val, typeof(string)) as string;
                                    curKey.Value = exportedStr;
                                }
                            }
                            break;  // No need to look further attributes
                        }
                    }
                }
            }

            Save(INIFilePath);
        }
    }
}
