using System;
using System.Windows.Forms;

namespace AssessmentManager
{
    public partial class AssessmentInformationForm : Form
    {
        public AssessmentInformationForm()
        {
            InitializeComponent();
        }

        #region Properties

        public string AssessmentName
        {
            get
            {
                return tbAssessmentName.Text;
            }
            set
            {
                tbAssessmentName.Text = value;
            }
        }

        public string Author
        {
            get
            {
                return textBoxAuthor.Text;
            }
            set
            {
                textBoxAuthor.Text = value;
            }
        }

        public int AssessmentWeighting
        {
            get
            {
                return (int)nudAssessmentWeighting.Value;
            }
            set
            {
                nudAssessmentWeighting.Value = (decimal)value;
            }
        }

        public AssessmentInformation AssessmentInformation
        {
            get
            {
                return new AssessmentInformation()
                {
                    AssessmentName = this.AssessmentName,
                    Author = this.Author,
                    AssessmentWeighting = this.AssessmentWeighting
                };
            }
        }


        #endregion

        #region Events

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (AssessmentName.NullOrEmpty())
            {
                MessageBox.Show("Please enter the assessment name.","Assessment name required");
                
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion

        #region Methods

        public static AssessmentInformationForm FromAssessmentInfo(AssessmentInformation assessmentInfo)
        {
            AssessmentInformationForm aif = new AssessmentInformationForm();
            aif.AssessmentName = assessmentInfo.AssessmentName;
            aif.Author = assessmentInfo.Author;
            aif.AssessmentWeighting = assessmentInfo.AssessmentWeighting;
            return aif;
        }

        public static void PopulateAssessmentInformation(AssessmentInformation assessmentInfo, AssessmentInformationForm aif)
        {
            assessmentInfo.AssessmentName = aif.AssessmentName;
            assessmentInfo.Author = aif.Author;
            assessmentInfo.AssessmentWeighting = (int)aif.AssessmentWeighting;
        }

        #endregion

    }
}
