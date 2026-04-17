using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace alcwin
{
    public class Settings
    {
        private const string ConfigDirectory = "OakyTech\\alcwin";
        private const string ConfigFile = "Settings.config";

        public string Hostname;

        public static Settings Load()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string BasePath = Path.Combine(AppData, ConfigDirectory);
            string Filename = Path.Combine(BasePath, ConfigFile);

            Settings s = null;

            try
            {
                if (File.Exists(Filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    XmlReader reader = XmlReader.Create(Filename);
                    s = (Settings)serializer.Deserialize(reader);
                    reader.Close();
                }
            }
            catch
            {
                s = null;
            }
            
            if (s==null)
                s = new Settings();

            return s;
        }

        public void Save()
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string BasePath = Path.Combine(AppData, ConfigDirectory);
            string Filename = Path.Combine(BasePath, ConfigFile);

            try
            {
                DirectoryInfo di = new DirectoryInfo(BasePath);
                if (!di.Exists)
                    di.Create();

                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(Filename, settings);
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch
            {
            }
        }
    }
}
