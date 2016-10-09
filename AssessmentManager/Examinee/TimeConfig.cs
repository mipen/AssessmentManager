using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssessmentManager
{
    public partial class TimeConfig : Form
    {
        public TimeConfig(bool showCancel = true)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            if (!showCancel)
            {
                btnCancel.Enabled = false;
                btnCancel.Visible = false;
            }
        }

        public int ReadingTime
        {
            get
            {
                return (int)nudReadingTime.Value;
            }
            set
            {
                nudReadingTime.Value = value;
            }
        }

        public int AssessmentTime
        {
            get
            {
                return (int)nudAssessmentTime.Value;
            }
            set
            {
                nudAssessmentTime.Value = value;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //Close();
        }
    }
}
