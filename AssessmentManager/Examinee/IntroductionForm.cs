using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AssessmentManager.CONSTANTS;

namespace AssessmentManager
{
    public partial class IntroductionForm : Form
    {

        private AssessmentScript script = null;
        private string filePath = null;
        private View CurView = View.Open;

        public IntroductionForm(string[] args)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            DateTime dt = DateTime.Now;
            pnlInformation.Visible = false;
            lblDateTimeDisp.Text = dt.ToLongDateString() + " " + dt.ToLongTimeString();
            if (args.Count() > 0)
            {
                OpenFromFile(args[0]);
            }
            else
            {
                NotifyNothingOpened();
            }
        }

        #region Events

        private void timer_Tick(object sender, EventArgs e)
        {
            //Update the current time and date.
            DateTime dt = DateTime.Now;
            lblDateTimeDisp.Text = dt.ToLongDateString() + " " + dt.ToLongTimeString();

            if (CurView == View.Continue)
                CheckTimeRemainingForContinue();
            if (CurView == View.Assessment)
                AssessmentTimerAction();

        }

        #endregion

        #region Methods

        public void OpenFromFile()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = COMBINED_FILTER;
            o.DefaultExt = ASSESSMENT_SCRIPT_EXT;
            o.InitialDirectory = DESKTOP_PATH;

            if (o.ShowDialog() == DialogResult.OK)
            {
                OpenFromFile(o.FileName);
            }
        }

        public void OpenFromFile(string path)
        {
            if (File.Exists(path))
            {
                string ext = Path.GetExtension(path);
                if (ext == ASSESSMENT_EXT)
                {
                    try
                    {
                        using (FileStream s = File.Open(path, FileMode.Open))
                        {
                            BinaryFormatter f = new BinaryFormatter();
                            Assessment assessment = (Assessment)f.Deserialize(s);
                            script = AssessmentScript.BuildFromAssessment(assessment);
                            string scriptPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ASSESSMENT_SCRIPT_EXT);
                            NotifyAssessmentScriptOpened(scriptPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to load file " + path + "\n\n" + ex.Message);
                    }
                }
                else if (ext == ASSESSMENT_SCRIPT_EXT)
                {
                    try
                    {
                        using (FileStream s = File.Open(path, FileMode.Open))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            script = (AssessmentScript)formatter.Deserialize(s);
                            NotifyAssessmentScriptOpened(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to load file " + path + "\n\n" + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Unable to open file: " + path + "\n\n File has incorrect extension.");
                }
            }
        }

        private void NotifyAssessmentScriptOpened(string path)
        {
            //Record the path
            filePath = path;

            //Show the assessment information
            DisplayInformation(script);

            //Determine the view to show for the script
            if (script.Published)
            {
                if (script.Started)
                {
                    if (script.TimeData.Finished)
                        ChangeView(View.Finished);
                    else
                        ChangeView(View.Continue);
                }
                else
                    ChangeView(View.Assessment);
            }
            else
            {
                //Practice mode
                if (script.Started)
                {
                    if (script.TimeData.Finished)
                        ChangeView(View.Finished);
                    else
                        ChangeView(View.Continue);
                }
                else
                    ChangeView(View.Practice);
            }
        }

        private void NotifyNothingOpened()
        {
            ChangeView(View.Open);
        }

        private void ChangeView(View v)
        {
            switch (v)
            {
                case View.Open:
                    {
                        CurView = View.Open;
                        //continue panel
                        pnlContinueAssessment.Enabled = false;
                        pnlContinueAssessment.Visible = false;

                        //practice panel
                        pnlPractise.Enabled = false;
                        pnlPractise.Visible = false;

                        //open assessment panel
                        pnlOpenAssessment.Enabled = true;
                        pnlOpenAssessment.Visible = true;

                        //finished panel
                        pnlAssessmentFinished.Enabled = false;
                        pnlAssessmentFinished.Visible = false;

                        //Published panel
                        pnlPublished.Visible = false;
                        pnlPublished.Enabled = false;

                        break;
                    }
                case View.Practice:
                    {
                        CurView = View.Practice;
                        //continue panel
                        pnlContinueAssessment.Enabled = false;
                        pnlContinueAssessment.Visible = false;

                        //practice panel
                        pnlPractise.Enabled = true;
                        pnlPractise.Visible = true;

                        //open assessment panel
                        pnlOpenAssessment.Enabled = false;
                        pnlOpenAssessment.Visible = false;

                        //finished panel
                        pnlAssessmentFinished.Enabled = false;
                        pnlAssessmentFinished.Visible = false;

                        //Published panel
                        pnlPublished.Visible = false;
                        pnlPublished.Enabled = false;

                        break;
                    }
                case View.Continue:
                    {
                        CurView = View.Continue;
                        //continue panel
                        pnlContinueAssessment.Enabled = true;
                        pnlContinueAssessment.Visible = true;

                        //practice panel
                        pnlPractise.Enabled = false;
                        pnlPractise.Visible = false;

                        //open assessment panel
                        pnlOpenAssessment.Enabled = false;
                        pnlOpenAssessment.Visible = false;

                        //finished panel
                        pnlAssessmentFinished.Enabled = false;
                        pnlAssessmentFinished.Visible = false;

                        //Published panel
                        pnlPublished.Visible = false;
                        pnlPublished.Enabled = false;

                        //Show the message
                        if (script.Published)
                        {
                            lblContinueDescription.Text = IntroductionConstants.PublishedContinueMessage;
                            //Show start and finish times
                            lblTimeStarted.Text = "Start: " + script.TimeData.PlannedStartTime.ToShortDateString() + " " + script.TimeData.PlannedStartTime.ToLongTimeString();
                            lblFinishingTime.Text = "Finish: " + script.TimeData.PlannedFinishTime.ToShortDateString() + " " + script.TimeData.PlannedFinishTime.ToLongTimeString();
                        }
                        else
                        {
                            lblContinueDescription.Text = IntroductionConstants.PracticeContinueMessage;
                            //Show start and finish times
                            lblTimeStarted.Text = "Start: " + script.TimeData.TimeStarted.ToShortDateString() + " " + script.TimeData.TimeStarted.ToLongTimeString();
                            lblFinishingTime.Text = "Finish: " + script.TimeData.PlannedFinishTime.ToShortDateString() + " " + script.TimeData.PlannedFinishTime.ToLongTimeString();
                        }

                        TimeSpan ts = script.TimeData.TimeRemaining;
                        lblTimeRemaining.Text = $"Time remaining: {ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";

                        break;
                    }
                case View.Assessment:
                    {
                        CurView = View.Assessment;
                        //continue panel
                        pnlContinueAssessment.Enabled = false;
                        pnlContinueAssessment.Visible = false;

                        //practice panel
                        pnlPractise.Enabled = false;
                        pnlPractise.Visible = false;

                        //open assessment panel
                        pnlOpenAssessment.Enabled = false;
                        pnlOpenAssessment.Visible = false;

                        //finished panel
                        pnlAssessmentFinished.Enabled = false;
                        pnlAssessmentFinished.Visible = false;

                        //Published panel
                        pnlPublished.Visible = true;
                        pnlPublished.Enabled = true;

                        //Set the information
                        SetPublishedInformation(script.TimeData.IsAvailable);

                        break;
                    }
                case View.Finished:
                    {
                        CurView = View.Finished;
                        //continue panel
                        pnlContinueAssessment.Enabled = false;
                        pnlContinueAssessment.Visible = false;

                        //practice panel
                        pnlPractise.Enabled = false;
                        pnlPractise.Visible = false;

                        //open assessment panel
                        pnlOpenAssessment.Enabled = false;
                        pnlOpenAssessment.Visible = false;

                        //finished panel
                        pnlAssessmentFinished.Enabled = true;
                        pnlAssessmentFinished.Visible = true;

                        //Published panel
                        pnlPublished.Visible = false;
                        pnlPublished.Enabled = false;

                        //Show start and finish times
                        lblFinishedTimeStarted.Text = "Started: " + script.TimeData.PlannedStartTime.ToString("F");
                        lblFinishedTimeFinished.Text = "Finished: " + script.TimeData.PlannedFinishTime.ToString("F");

                        break;
                    }
            }
        }

        private void DisplayInformation(AssessmentScript a)
        {
            //Show the assessment info. This will always be present
            if (a.AssessmentInfo != null)
            {
                AssessmentInformation ai = a.AssessmentInfo;

                if (!ai.AssessmentName.NullOrEmpty())
                    lblAssessmentName.Text = ai.AssessmentName;
                else
                    lblAssessmentName.Text = "Assessment";

                if (!ai.Author.NullOrEmpty())
                    lblAuthor.Text = $"Author: {ai.Author}";
                else
                    lblAuthor.Text = "";

                lblWeighting.Text = ai.AssessmentWeighting < 0 ? $"{ai.AssessmentWeighting}%" : "";
            }

            //Show the course info. This will only be present if the assessment has been published.
            if (a.CourseInformation != null)
            {
                CourseInformation c = a.CourseInformation;
                if (!c.CourseCodeFull.NullOrEmpty())
                    lblCourseCode.Text = c.CourseCodeFull;
                else
                    lblCourseCode.Text = "";

                if (!c.CourseName.NullOrEmpty())
                    lblCourseName.Text = c.CourseName;
                else
                    lblCourseName.Text = "Unkown course";
            }
            //Enable the information panel
            pnlInformation.Enabled = true;
            pnlInformation.Visible = true;
        }

        private void SetPublishedInformation(bool available)
        {
            if (available)
            {
                lblStartTime.Text = "Time Started:";
                lblTimeUntilStart.Text = "Time remaining:";
                TimeSpan ts = script.TimeData.TimeRemaining;
                lblTimeUntilStartInt.Text = $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
                lblPublishedMessage.Text = "Assessment has begun";
                lblPublishedMessage.ForeColor = Color.Green;
                btnPublished.Text = "Start";
            }
            else
            {
                lblStartTime.Text = "Start Time:";
                lblTimeUntilStart.Text = "Time until start:";
                TimeSpan ts = script.TimeData.TimeUntilBegin;
                lblTimeUntilStartInt.Text = $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
                lblPublishedMessage.Text = "Assessment has not yet begun";
                lblPublishedMessage.ForeColor = Color.Blue;
                btnPublished.Text = "OK";
            }
            lblStartTimeInt.Text = script.TimeData.PlannedStartTime.ToString("hh:mm:ss tt");
            lblFinishTimeInt.Text = script.TimeData.PlannedFinishTime.ToString("hh:mm:ss tt");
        }

        private void CheckTimeRemainingForContinue()
        {
            TimeSpan ts = script.TimeData.TimeRemaining;
            lblTimeRemaining.Text = $"Time remaining: {ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
            if (script.TimeData.Finished && CurView == View.Continue)
            {
                ChangeView(View.Finished);
            }
        }

        private void AssessmentTimerAction()
        {
            //Refresh timer
            TimeSpan ts;
            if (script.TimeData.IsAvailable)
                ts = script.TimeData.TimeRemaining;
            else
                ts = script.TimeData.TimeUntilBegin;
            lblTimeUntilStartInt.Text = $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";

            //If assessment has begun, change the display
            if (script.TimeData.IsAvailable)
                SetPublishedInformation(true);
            if (script.TimeData.Finished)
                ChangeView(View.Finished);
        }

        #endregion

        #region Buttons

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFromFile();
        }

        private void btnYesPractice_Click(object sender, EventArgs e)
        {
            //Ask for the time allowed for the assessment. TimeData should never be null here
            TimeConfig tc = new TimeConfig(false);
            tc.AssessmentTime = script.TimeData.Minutes;
            tc.ReadingTime = script.TimeData.ReadingMinutes;

            if (tc.ShowDialog() == DialogResult.OK)
            {
                script.TimeData.Minutes = tc.AssessmentTime;
                script.TimeData.ReadingMinutes = tc.ReadingTime;
            }

            //Set the start time
            script.TimeData.PlannedStartTime = DateTime.Now;
            script.TimeData.TimeStarted = DateTime.Now;
            //Set the finish time
            script.TimeData.PlannedFinishTime = script.TimeData.TimeStarted.AddMinutes(script.TimeData.Minutes + script.TimeData.ReadingMinutes);
            //Set reading finish time
            script.TimeData.ReadingFinishTime = script.TimeData.TimeStarted.AddMinutes(script.TimeData.ReadingMinutes);

            Examinee ex = new Examinee(script, filePath);
            ex.Show();
            timer.Enabled = false;
            Hide();
        }

        private void btnNoPractice_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnContinueNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnContinueYes_Click(object sender, EventArgs e)
        {
            //If assessment is published, ask for password before opening
            if (script.Published)
            {
                RestartPasswordForm rpf = new RestartPasswordForm();
                if(rpf.ShowDialog()==DialogResult.OK)
                {
                    if(rpf.EnteredPassword == script.StudentData.RestartPassword)
                    {
                        Examinee ex = new Examinee(script, filePath);
                        ex.Show();
                        timer.Enabled = false;
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password", "Incorrect password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                Examinee ex = new Examinee(script, filePath);
                ex.Show();
                timer.Enabled = false;
                Hide();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPublished_Click(object sender, EventArgs e)
        {
            if (script.TimeData.IsAvailable)
            {
                Examinee ex = new Examinee(script, filePath);
                ex.Show();
                timer.Enabled = false;
                Hide();
            }
            else
            {
                Close();
            }
        }

        #endregion

    }
}
