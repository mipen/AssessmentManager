using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AssessmentManager
{
    public sealed class Settings
    {
        private static Settings instance;
        private List<string> recentFiles = new List<string>();
        private static readonly string FILE_NAME = "settings.xml";

        //Email stuff
        private string username = "";
        private string password = "";
        private string smtp = "";
        private string message = "";
        private int port = 587;
        private bool ssl = true;

        public List<string> RecentFiles => recentFiles;

        private Settings()
        {

        }

        #region Email Properties
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public string Password
        {
            get
            {
                return password;
            }

            set
            {
                password = value;
            }
        }

        public string Smtp
        {
            get
            {
                return smtp;
            }

            set
            {
                smtp = value;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        public int Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public bool SSL
        {
            get
            {
                return ssl;
            }

            set
            {
                ssl = value;
            }
        }
        #endregion

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

        public void SetFromConfigForm(EmailConfigForm ecf)
        {
            Username = ecf.UserName;
            Password = ecf.Password;
            Smtp = ecf.Smtp;
            Message = ecf.Message;
            Port = ecf.Port;
            SSL = ecf.SSL;
        }
    }
}
