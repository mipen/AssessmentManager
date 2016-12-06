using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using static AssessmentManager.CONSTANTS;

namespace AssessmentManager
{
    [Serializable]
    public sealed class Settings
    {
        private static Settings instance;
        private List<string> recentFiles = new List<string>();

        //Email stuff
        private string username = "";
        private string password = "";
        private string smtp = "smtp.office365.com";
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
            //Create the settings file if it doesn't exist
            if (!File.Exists(SETTINGS_FILE_PATH))
            {
                instance = new Settings();
            }
            else
            {
                //Otherwise try read the values from the file
                using (var stream = File.Open(SETTINGS_FILE_PATH, FileMode.Open))
                {
                    try
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        instance = (Settings)bf.Deserialize(stream);
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
            try
            {
                using (var stream = File.Open(SETTINGS_FILE_PATH, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, instance);
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
