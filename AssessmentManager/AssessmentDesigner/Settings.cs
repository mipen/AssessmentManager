using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AssessmentManager
{
    public sealed class Settings
    {
        private static Settings instance;
        private List<string> recentFiles = new List<string>();
        private static readonly string FILE_NAME = "settings.xml";

        public List<string> RecentFiles => recentFiles;

        private Settings()
        {

        }

        public static void Init()
        {
            string startupPath = Application.StartupPath;
            string filePath = startupPath + "\\" + FILE_NAME;
            if (!File.Exists(filePath))
            {
                instance = new Settings();
            }
            else
            {
                using (var stream = File.Open(filePath,FileMode.Open))
                {
                    try
                    {
                        XmlSerializer x = new XmlSerializer(typeof(Settings));
                        instance = (Settings)x.Deserialize(stream);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("There was an error loading settings, reverting to default. \n\n " + ex.Message);
                        instance = new Settings();
                    }
                }
            }
        }

        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        public void Save()
        {
            //Save the settings in the instance here
            string startupPath = Application.StartupPath;
            string filePath = startupPath + "\\" + FILE_NAME;

            try
            {
                using (var stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
                {
                    XmlSerializer x = new XmlSerializer(typeof(Settings));
                    x.Serialize(stream, instance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AddRecentFile(string path)
        {
            if (RecentFiles.Contains(path))
            {
                RecentFiles.Remove(path);
            }
                RecentFiles.Insert(0, path);
                while (RecentFiles.Count > 5)
                {
                    try
                    {
                        RecentFiles.Remove(RecentFiles[5]);
                    }
                    catch
                    {
                    }
                }
        }
    }
}
