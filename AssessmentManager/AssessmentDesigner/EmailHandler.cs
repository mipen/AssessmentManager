using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace AssessmentManager
{
    public partial class EmailHandler : Form
    {
        private const string ManukauMail = "@manukaumail.com";

        private AssessmentSession session = null;
        private List<StudentMarkingData> smd = null;
        private int labelNum = 0;
        private int sentCount = 0;
        private int attemptedEmails = 0;
        private List<string> erroredEmails = new List<string>();
        private const string label0 = "Sending email ... Please wait";
        private const string label1 = "Sending email ... Please wait .";
        private const string label2 = "Sending email ... Please wait ..";
        private const string label3 = "Sending email ... Please wait ...";

        public EmailHandler(AssessmentSession session, List<StudentMarkingData> smd)
        {
            InitializeComponent();
            this.session = session;
            this.smd = smd;

            if (smd == null || smd.Count == 0)
            {
                throw new ArgumentException("Given StudentMarkingData is null or contains no elements", "smd");
            }
            progress.Maximum = smd.Count;
            SentCount = 0;
        }

        public EmailHandler(AssessmentSession session, StudentMarkingData smd) : this(session, new List<StudentMarkingData> { smd })
        {
        }

        #region Properties
        public string Username
        {
            get
            {
                return Settings.Instance.Username;
            }
            set
            {
                Settings.Instance.Username = value;
            }
        }

        public string Password
        {
            get
            {
                return Settings.Instance.Password;
            }
            set
            {
                Settings.Instance.Password = value;
            }
        }

        public string Smtp
        {
            get
            {
                return Settings.Instance.Smtp;
            }
            set
            {
                Settings.Instance.Smtp = value;
            }
        }

        public string Message
        {
            get
            {
                return Settings.Instance.Message;
            }
            set
            {
                Settings.Instance.Message = value;
            }
        }

        public int Port
        {
            get
            {
                return Settings.Instance.Port;
            }
            set
            {
                Settings.Instance.Port = value;
            }
        }

        public bool SSL
        {
            get
            {
                return Settings.Instance.SSL;
            }
            set
            {
                Settings.Instance.SSL = value;
            }
        }

        private int Label
        {
            get
            {
                return labelNum;
            }
            set
            {
                labelNum = value;
                switch (labelNum)
                {
                    case 0:
                        {
                            lblDisplay.Text = label0;
                            break;
                        }
                    case 1:
                        {
                            lblDisplay.Text = label1;
                            break;
                        }
                    case 2:
                        {
                            lblDisplay.Text = label2;
                            break;
                        }
                    case 3:
                        {
                            lblDisplay.Text = label3;
                            break;
                        }
                    default:
                        {
                            Label = 0;
                            break;
                        }
                }
            }
        }

        private int SentCount
        {
            get
            {
                return sentCount;
            }
            set
            {
                sentCount = value;
                lblSentCount.Text = $"({sentCount}/{smd.Count})";
            }
        }
        #endregion

        #region Methods

        private void SendEmail()
        {
            //Check for any missing parts in the credentials
            if (Username.NullOrEmpty() || Password.NullOrEmpty() || Smtp.NullOrEmpty())
            {
                MessageBox.Show("Cannot send email - please make sure you have entered your Username, Password and smtp server correctly.", "Invalid Username, password or smtp server", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var s in smd)
            {
                try
                {
                    //Create the pdf to be attached
                    if (!Directory.Exists(CONSTANTS.TEMP_PDF_PATH))
                        Directory.CreateDirectory(CONSTANTS.TEMP_PDF_PATH);
                    string filePath = Path.Combine(CONSTANTS.TEMP_PDF_PATH, s.StudentData.UserName + CONSTANTS.PDF_EXT);
                    try
                    {
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    catch
                    {
                        int num = 0;
                        do
                        {
                            num++;
                            filePath = Path.Combine(CONSTANTS.TEMP_PDF_PATH, s.StudentData.UserName + $"({num})" + CONSTANTS.PDF_EXT);
                        } while (File.Exists(filePath));
                    }
                    AssessmentResultWriter arw = new AssessmentResultWriter(s);
                    arw.MakePDF(filePath);

                    NetworkCredential login = new NetworkCredential(Username, Password);
                    SmtpClient client = new SmtpClient(Smtp);
                    client.Port = Port;
                    client.EnableSsl = SSL;
                    client.Credentials = login;
                    MailAddress from = new MailAddress(Username, Username, Encoding.UTF8);
                    MailAddress to = new MailAddress(s.StudentData.UserName + ManukauMail);
                    MailMessage msg = new MailMessage(from, to);
                    msg.Subject = session.AssessmentInfo.AssessmentName + " Results";
                    msg.Body = Message;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.IsBodyHtml = true;
                    msg.Priority = MailPriority.Normal;
                    msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    msg.Attachments.Add(new Attachment(filePath));
                    client.SendCompleted += SendCompletedCallBack;
                    client.SendAsync(msg, s.StudentData.UserName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error emailing " + s.StudentData.UserName + "\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    attemptedEmails++;
                    erroredEmails.Add(s.StudentData.UserName);
                    continue;
                }
            }
        }

        private void SendCompletedCallBack(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show($"{e.UserState} send cancelled");
            if (e.Error != null)
            {
                MessageBox.Show($"Error sending email to {e.UserState} \nPlease ensure your login details and smtp server are correct. Other causes for this error could be SSL or the chosen port. \n\n Exception:\n" + e.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                erroredEmails.Add(e.UserState.ToString());
                attemptedEmails++;
            }
            else
            {
                progress.Value++;
                SentCount++;
                attemptedEmails++;
            }

        }

        #endregion

        #region Events

        private void EmailHandler_Shown(object sender, EventArgs e)
        {
            SendEmail();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Label++;
            if (attemptedEmails >= smd.Count)
            {
                StringBuilder sb = new StringBuilder();
                string title = "";
                if (erroredEmails.Count == 0)
                {
                    sb.AppendLine("All emails sent successfully!");
                    title = "Completed";
                }
                else
                {
                    sb.AppendLine($"Successfully sent {SentCount} email(s)");
                    sb.AppendLine();
                    sb.AppendLine("The following email(s) were not sent:");
                    foreach (var s in erroredEmails)
                    {
                        sb.AppendLine("     " + s);
                    }
                    title = "Partially completed";
                }
                timer.Enabled = false;
                MessageBox.Show(sb.ToString(), title);
                this.Close();
            }
        }

        #endregion

    }
}
