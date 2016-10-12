using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssessmentManager
{
    public class EmailHandler
    {
        private const string ManukauMail = "@manukaumail.com";

        private AssessmentSession session = null;
        private StudentMarkingData smd = null;
        private string pdfPath = "";
        private bool deletePdf = false;

        public EmailHandler(AssessmentSession session, StudentMarkingData smd, string pdfPath, bool deletePdf)
        {
            this.session = session;
            this.smd = smd;
            this.pdfPath = pdfPath;
            this.deletePdf = deletePdf;
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
        #endregion

        #region Methods

        public void SendEmail()
        {
            NetworkCredential login = new NetworkCredential(Username, Password);
            SmtpClient client = new SmtpClient(Smtp);
            client.Port = Port;
            client.EnableSsl = SSL;
            client.Credentials = login;
            MailAddress from = new MailAddress(Username, Username, Encoding.UTF8);
            MailAddress to = new MailAddress(smd.StudentData.UserName + ManukauMail);
            MailMessage msg = new MailMessage(from, to);
            msg.Subject = session.AssessmentInfo.AssessmentName + " Results";
            msg.Body = Message;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.Normal;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            msg.Attachments.Add(new Attachment(pdfPath));
            client.SendCompleted += SendCompletedCallBack;
            client.SendAsync(msg, "Sending...");
            //client.Send(msg);
        }

        private void SendCompletedCallBack(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show($"{e.UserState} send cancelled");
            if (e.Error != null)
                MessageBox.Show("Error sending email: \n" + e.Error.ToString());
            else
                MessageBox.Show("Message sent successfully");
        }

        #endregion
    }
}
