using System.Windows.Forms;

namespace AssessmentManager
{
    public partial class EmailConfigForm : Form
    {
        public EmailConfigForm()
        {
            InitializeComponent();

            tbPassword.PasswordChar = '*';
        }

        #region Properties

        public string UserName
        {
            get
            {
                return tbUserName.Text;
            }
            set
            {
                tbUserName.Text = value;
            }
        }

        public string Password
        {
            get
            {
                return tbPassword.Text;
            }
            set
            {
                tbPassword.Text = value;
            }
        }

        public string Smtp
        {
            get
            {
                return tbSmtp.Text;
            }
            set
            {
                tbSmtp.Text = value;
            }
        }

        public int Port
        {
            get
            {
                int num = 0;
                if (int.TryParse(tbPort.Text, out num))
                    return num;
                else
                    return 587;
            }
            set
            {
                tbPort.Text = value.ToString();
            }
        }

        public bool SSL
        {
            get
            {
                return chkSSL.Checked;
            }
            set
            {
                chkSSL.Checked = value;
            }
        }

        public string Message
        {
            get
            {
                return rtbMessage.Text;
            }
            set
            {
                rtbMessage.Text = value;
            }
        }

        #endregion

        #region Events

        private void tbPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        #endregion
    }
}
