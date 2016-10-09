using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using static AssessmentManager.CONSTANTS;

namespace AssessmentManager
{
    public partial class Examinee : Form
    {
        private AssessmentScript script;
        private Stage curStage;
        private Mode curMode;
        private bool changesMade = false;
        private bool submitButtonPushed = false;
        private int marksAttempted = 0;
        private int TotalMarksCache = 0;
        private string filePath = null;

        public Examinee(AssessmentScript script, string path)
        {
            InitializeComponent();
            filePath = path;
            buttonSubmitAssessment.UseCompatibleTextRendering = true;
            StartPosition = FormStartPosition.CenterScreen;
            Script = script;
            NotifyAssessmentOpened();
        }

        #region Properties

        public AssessmentScript Script
        {
            get { return script; }
            private set { script = value; }
        }

        public Question SelectedQuestion
        {
            get
            {
                return SelectedNode.Question;
            }
        }

        public QuestionNode SelectedNode
        {
            get
            {
                return (QuestionNode)treeViewQuestionDisplay.SelectedNode;
            }
        }

        public Answer SelectedQuestionAnswer
        {
            get
            {
                try
                {
                    return script.Answers[SelectedQuestion.Name];
                }
                catch (Exception)
                {
                    MessageBox.Show($"An error has occured: {SelectedQuestion.Name} tried to access its answer, but it doesn't have one.");
                    return null;
                }
            }
        }

        public List<string> UnattemptedQuestions
        {
            get
            {
                List<string> list = new List<string>();
                foreach (var kvp in Script.Answers)
                {
                    if (!kvp.Value.Attempted)
                        list.Add(kvp.Key);
                }
                return list;
            }
        }

        public Mode CurMode
        {
            get
            {
                return curMode;
            }
            set
            {
                string assessmentName = (Script.AssessmentInfo != null && !Script.AssessmentInfo.AssessmentName.NullOrEmpty()) ? Script.AssessmentInfo.AssessmentName : Path.GetFileNameWithoutExtension(filePath);
                switch (value)
                {
                    case Mode.Assessment:
                        {
                            curMode = Mode.Assessment;
                            Text = $"Examinee - {assessmentName}";
                            break;
                        }
                    case Mode.Practice:
                        {
                            curMode = Mode.Practice;
                            Text = $"Examinee - {assessmentName} [Practice]";
                            buttonSubmitAssessment.Text = "Save";
                            break;
                        }
                }
            }
        }

        public Stage CurStage
        {
            get
            {
                return curStage;
            }
            set
            {
                switch (value)
                {
                    case Stage.Reading:
                        {
                            //Disable all answer entry points
                            panelAnswerContainer.Enabled = false;
                            //Disable submit button
                            buttonSubmitAssessment.Enabled = false;
                            timer.Enabled = true;
                            curStage = Stage.Reading;
                            lblTimeRemaining.Text = "Reading time:";

                            lblMotivational.ForeColor = Color.Orange;
                            lblMotivational.Text = "Reading time";
                            break;
                        }
                    case Stage.Running:
                        {
                            //Enable all answer entry points
                            panelAnswerContainer.Enabled = true;
                            timer.Enabled = true;
                            buttonSubmitAssessment.Enabled = true;
                            curStage = Stage.Running;
                            lblTimeRemaining.Text = "Time remaining:";
                            break;
                        }
                    case Stage.Completed:
                        {
                            //Disable all answer entry points
                            panelAnswerContainer.Enabled = false;
                            curStage = Stage.Completed;
                            timer.Enabled = false;
                            lblTimeRemaining.Text = "";
                            lblTimeRemainingTimer.Text = "";
                            lblMotivational.ForeColor = Color.Blue;
                            lblMotivational.Text = "Completed. Please Submit.";
                            break;
                        }
                }
            }
        }

        #endregion

        #region Methods

        public void NotifyAssessmentOpened()
        {
            //Reset the image tracker
            ImageTracker.Reset();

            //Load questions into tree view
            Util.PopulateTreeView(treeViewQuestionDisplay, Script);

            //Select the first question
            if (treeViewQuestionDisplay.Nodes.Count > 0)
                treeViewQuestionDisplay.SelectedNode = treeViewQuestionDisplay.Nodes[0];

            //Set the mode:
            if (Script.Published)
                CurMode = Mode.Assessment;
            else
                CurMode = Mode.Practice;

            //Set the stage:
            if (Script.Started)
            {
                //Handle an already started assessment
                if (Script.TimeData.HasReadingTime && DateTime.Now < Script.TimeData.ReadingFinishTime)
                    CurStage = Stage.Reading;
                else if (DateTime.Now < Script.TimeData.PlannedFinishTime)
                    CurStage = Stage.Running;
                else
                    CurStage = Stage.Completed;
            }
            else
            {
                //AssessmentScript has not been started yet:
                Script.Started = true;
                Script.TimeData.TimeStarted = DateTime.Now;
                if (Script.TimeData.HasReadingTime)
                    CurStage = Stage.Reading;
                else
                    CurStage = Stage.Running;
            }

            //Cache the total number of marks
            TotalMarksCache = Script.TotalMarks;

            //Set the maximum for the progress bar
            progressBarMarksAttempted.Maximum = TotalMarksCache;

            //Update the unattempted list
            listBoxUnansweredQuestions.Items.Clear();
            listBoxUnansweredQuestions.Items.AddRange(UnattemptedQuestions.ToArray());

            //Display the times
            UpdateTimeDisplay(true);

        }

        public void NotifyAssessmentClosed()
        {
            //Disable all ui
            panelQuestionAnswer.Enabled = false;
            panelQuestionAnswer.Visible = false;

            panelLeft.Enabled = false;
            panelLeft.Visible = false;

            panelTop.Enabled = false;
            panelTop.Visible = false;
        }

        private void UpdateMarksDisplay()
        {
            //Update the marks
            labelQuestionMarks.Text = SelectedQuestion?.Name;
            labelQuestionMarksNum.Text = $"{SelectedQuestion?.Marks} ({SelectedQuestion?.TotalMarks} total)";

            if (SelectedNode.Parent != null)
            {
                Question pq = ((QuestionNode)SelectedNode.Parent).Question;
                labelSubQuestionMarks.Text = pq.Name;
                labelSubQuestionMarksNum.Text = $"{pq?.Marks} ({pq?.TotalMarks} total)";

                labelSubQuestionMarks.Visible = true;
                labelSubQuestionMarksNum.Visible = true;

                if (SelectedNode.Parent.Parent != null)
                {
                    Question ppq = ((QuestionNode)SelectedNode.Parent.Parent).Question;
                    labelSubSubQuestionMarks.Text = ppq.Name;
                    labelSubSubQuestionMarksNum.Text = $"{ppq?.Marks} ({ppq?.TotalMarks} total)";

                    labelSubSubQuestionMarks.Visible = true;
                    labelSubSubQuestionMarksNum.Visible = true;
                }
                else
                {
                    labelSubSubQuestionMarks.Visible = false;
                    labelSubSubQuestionMarksNum.Visible = false;
                }
            }
            else
            {
                //Display sub question display
                labelSubQuestionMarks.Visible = false;
                labelSubQuestionMarksNum.Visible = false;

                //Disable sub sub question display
                labelSubSubQuestionMarks.Visible = false;
                labelSubSubQuestionMarksNum.Visible = false;
            }

            //Update the progress display
            UpdateAttemptedMarks();
        }

        private void UpdateAttemptedMarks()
        {
            marksAttempted = 0;
            foreach (var q in Script.Questions)
                marksAttempted += q.AttemptedMarks(script.Answers);

            labelMarksAttempted.Text = $"{marksAttempted}/{TotalMarksCache} marks attempted";
            progressBarMarksAttempted.Value = marksAttempted;
        }

        private void UpdateTimeDisplay(bool initial = false)
        {
            //Show the start and end times
            if (initial)
            {
                lblTimeBegan.Text = Script.TimeData.TimeStarted.ToString("hh:mm:ss");
                lblFinishTime.Text = Script.TimeData.PlannedFinishTime.ToString("hh:mm:ss");
            }

            switch (CurStage)
            {
                case Stage.Reading:
                    {
                        //Will only get here if assessment has reading time, so ReadingFinishTime will never be null.
                        TimeSpan ts = Script.TimeData.ReadingFinishTime - DateTime.Now;
                        if (ts.Hours <= 0 && ts.Minutes <= 0 && ts.Seconds <= 0)
                        {
                            CurStage = Stage.Running;
                            break;
                        }
                        lblTimeRemainingTimer.Text = $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
                        break;
                    }
                case Stage.Running:
                    {
                        TimeSpan ts = Script.TimeData.TimeRemaining;
                        if (ts.Hours <= 0 && ts.Minutes <= 0 && ts.Seconds <= 0)
                        {
                            CurStage = Stage.Completed;
                            break;
                        }
                        lblTimeRemainingTimer.Text = $"{ts.Hours.ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";

                        //Update the motivational display
                        double marksPercentage = (double)marksAttempted / (double)TotalMarksCache;
                        double timePercentage = 1 - (ts.TotalMinutes / Script.TimeData.Minutes);
                        if (marksPercentage >= timePercentage)
                        {
                            //Give good message
                            lblMotivational.ForeColor = Color.Green;
                            lblMotivational.Text = "You are ahead of time";
                        }
                        else
                        {
                            //Give bad message
                            lblMotivational.ForeColor = Color.Red;
                            lblMotivational.Text = "You are falling behind";
                        }
                        break;
                    }
            }
        }

        private void SaveToFile(bool asBackup = false)
        {
            if (CurMode == Mode.Assessment)
            {
                string path = "";
                FileMode fileMode;

                //Get the path to save the file
                if (asBackup)
                {
                    //If the assessment is being autosaved, then save it in the autosave folder and give it an incremental number
                    string dir = Path.GetDirectoryName(filePath);
                    string name = Script.AssessmentInfo.AssessmentName;
                    string dir2 = Path.Combine(dir, AUTOSAVE_FOLDER_NAME(name));

                    //If the autosave directory does not exist, then create it.
                    if (!Directory.Exists(dir2))
                    {
                        Directory.CreateDirectory(dir2);
                    }
                    //Find the number to append to end of file
                    int number = (from t in Directory.GetFiles(dir2)
                                  where Path.GetExtension(t) == ASSESSMENT_SCRIPT_EXT
                                  select t).ToList().Count + 1;
                    string fileName = name + number.ToString("000");

                    path = Path.Combine(dir2, fileName + ASSESSMENT_SCRIPT_EXT);
                    fileMode = FileMode.Create;
                }
                else
                {
                    //Otherwise, save it as the final
                    path = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ASSESSMENT_SCRIPT_EXT;
                    fileMode = FileMode.OpenOrCreate;
                }
                try
                {
                    using (FileStream s = File.Open(path, fileMode, FileAccess.Write))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(s, Script);
                        changesMade = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Failed to save \n\n" + ex.Message);
                }
            }
            else
            {
                //Save as practice
                string filePath2 = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string path = filePath2 + "\\" + fileName + ASSESSMENT_SCRIPT_EXT;
                try
                {
                    using (FileStream s = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(s, Script);
                        changesMade = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Failed to save \n\n" + ex.Message);
                }
            }
        }

        public DialogResult DoSubmit()
        {
            string message;
            string title;
            MessageBoxButtons buttons;
            if (CurMode == Mode.Assessment)
            {
                message = "You are about to submit your assessment. Once you have done so, you will not be able to open it again without talking to your supervisor. Once you have submitted it, the assessment is over and the application will close. Would you like to continue?";
                title = "Submit assessment";
                buttons = MessageBoxButtons.YesNo;
            }
            else
            {
                message = $"Would you like to save this assessment? This will allow you to open it later and review your answers. This will close the application, but if there is time remaining you will be able to continue the practice assessment by opening the created {ASSESSMENT_SCRIPT_EXT} file.";
                title = "Save assessment";
                buttons = MessageBoxButtons.YesNoCancel;
            }

            DialogResult res = MessageBox.Show(message, title, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (res == DialogResult.Yes)
            {
                //User wants to submit assessment
                if (CurMode == Mode.Assessment)
                {
                    MessageBox.Show($"Thank you for using Examinee. \n\n Your assessment has been saved as {Path.GetDirectoryName(filePath)}\\{Path.GetFileNameWithoutExtension(filePath)}{ASSESSMENT_SCRIPT_EXT} \n\n The application will now close.", "Assessment submitted");
                    SaveToFile();
                }
                else
                {
                    //Practice mode
                    MessageBox.Show($"Thank you for using Examinee. \n\n Your practice assessment has been saved as {Path.GetDirectoryName(filePath)}\\{Path.GetFileNameWithoutExtension(filePath)}{ASSESSMENT_SCRIPT_EXT} \n\n The application will now close.", "Assessment saved");
                    Script.TimeData.TimeFinished = DateTime.Now;
                    SaveToFile();
                }
            }
            return res;
        }

        #endregion

        #region Control Events

        #region Radio Buttons

        private void rbOptionA_Click(object sender, EventArgs e)
        {
            rbOptionA.Checked = true;
            rbOptionB.Checked = false;
            rbOptionC.Checked = false;
            rbOptionD.Checked = false;
            changesMade = true;

            if (SelectedQuestion != null && SelectedQuestion.AnswerType != AnswerType.None)
            {
                SelectedQuestionAnswer.SelectedOption = MultiChoiceOption.A;
            }
        }

        private void rbOptionB_Click(object sender, EventArgs e)
        {
            rbOptionA.Checked = false;
            rbOptionB.Checked = true;
            rbOptionC.Checked = false;
            rbOptionD.Checked = false;
            changesMade = true;

            if (SelectedQuestion != null && SelectedQuestion.AnswerType != AnswerType.None)
            {
                SelectedQuestionAnswer.SelectedOption = MultiChoiceOption.B;
            }
        }

        private void rbOptionC_Click(object sender, EventArgs e)
        {
            rbOptionA.Checked = false;
            rbOptionB.Checked = false;
            rbOptionC.Checked = true;
            rbOptionD.Checked = false;
            changesMade = true;

            if (SelectedQuestion != null && SelectedQuestion.AnswerType != AnswerType.None)
            {
                SelectedQuestionAnswer.SelectedOption = MultiChoiceOption.C;
            }
        }

        private void rbOptionD_Click(object sender, EventArgs e)
        {
            rbOptionA.Checked = false;
            rbOptionB.Checked = false;
            rbOptionC.Checked = false;
            rbOptionD.Checked = true;
            changesMade = true;

            if (SelectedQuestion != null && SelectedQuestion.AnswerType != AnswerType.None)
            {
                SelectedQuestionAnswer.SelectedOption = MultiChoiceOption.D;
            }
        }

        #endregion

        private void buttonExpand_Click(object sender, EventArgs e)
        {
            treeViewQuestionDisplay.ExpandAll();
        }

        private void buttonCollapse_Click(object sender, EventArgs e)
        {
            treeViewQuestionDisplay.CollapseAll();
        }

        private void buttonPrevQuestion_Click(object sender, EventArgs e)
        {
            QuestionNode node = SelectedNode;
            if (node != null)
            {
                if (node.PrevVisibleNode != null)
                    treeViewQuestionDisplay.SelectedNode = node.PrevVisibleNode;
            }
            treeViewQuestionDisplay.Focus();
        }

        private void buttonNextQuestion_Click(object sender, EventArgs e)
        {
            QuestionNode node = SelectedNode;
            if (node != null)
            {
                if (node.NextVisibleNode != null)
                    treeViewQuestionDisplay.SelectedNode = node.NextVisibleNode;
            }
            treeViewQuestionDisplay.Focus();
        }

        private void btnQuestionImage_Click(object sender, EventArgs e)
        {
            Question question = SelectedQuestion;
            if (question != null && question.Image != null)
            {
                //Show the image if not already shown
                if (!ImageTracker.ImageDisplayShown(question.Name))
                {
                    ImageDisplay id = new ImageDisplay(question.Name, question.Image);
                    id.Show();
                    id.TopMost = true;
                    id.BringToFront();
                }
                else
                {
                    ImageTracker.FindImageDisplay(question.Name).Focus();
                }
            }
        }

        private void trackBarMagnification_Scroll(object sender, EventArgs e)
        {
            rtbQuestion.ZoomFactor = trackBarMagnification.Value;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void rtbAnswerLong_TextChanged(object sender, EventArgs e)
        {
            if (SelectedQuestion != null && SelectedQuestion.AnswerType != AnswerType.None)
            {
                SelectedQuestionAnswer.LongAnswer = rtbAnswerLong.Text;
                changesMade = true;
            }
        }

        private void textBoxAnswerShort_TextChanged(object sender, EventArgs e)
        {
            if (SelectedQuestion != null && SelectedQuestion.AnswerType != AnswerType.None)
            {
                SelectedQuestionAnswer.ShortAnswer = textBoxAnswerShort.Text;
                changesMade = true;
            }
        }

        private void listBoxUnansweredQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeNode[] nodes = treeViewQuestionDisplay.Nodes.Find((string)listBoxUnansweredQuestions.SelectedItem, true);
            if (nodes.Count() > 0)
            {
                TreeNode match = nodes[0];
                treeViewQuestionDisplay.SelectedNode = match;
            }
        }

        private void buttonSubmitAssessment_Click(object sender, EventArgs e)
        {
            DialogResult res = DoSubmit();
            if (CurMode == Mode.Assessment && res == DialogResult.Yes)
            {
                submitButtonPushed = true;
                Close();
            }
            else if (CurMode == Mode.Practice && (res == DialogResult.No || res == DialogResult.Yes))
            {
                submitButtonPushed = true;
                Close();
            }
            submitButtonPushed = false;
        }

        private void treeViewQuestionDisplay_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (SelectedQuestion == null)
                return;

            //Display the question text
            rtbQuestion.Rtf = SelectedQuestion.QuestionText;

            //Update the marks display
            UpdateMarksDisplay();

            //Show the question name
            lblQuestionNumber.Text = SelectedQuestion.Name;

            //Show the correct answer page and any answer already entered
            #region Answer
            switch (SelectedQuestion.AnswerType)
            {
                case AnswerType.None:
                    {
                        panelAnswerLongContainer.Enabled = false;
                        panelAnswerLongContainer.Visible = false;

                        panelAnswerShortContainer.Enabled = false;
                        panelAnswerShortContainer.Visible = false;

                        tlpMultiAnswerContainer.Enabled = false;
                        tlpMultiAnswerContainer.Visible = false;

                        labelAnswerText.Visible = false;
                        break;
                    }
                case AnswerType.Multi:
                    {
                        panelAnswerLongContainer.Enabled = false;
                        panelAnswerLongContainer.Visible = false;

                        panelAnswerShortContainer.Enabled = false;
                        panelAnswerShortContainer.Visible = false;

                        tlpMultiAnswerContainer.Enabled = true;
                        tlpMultiAnswerContainer.Visible = true;

                        labelAnswerText.Visible = true;

                        labelOptionA.Text = SelectedQuestion?.OptionA;
                        labelOptionB.Text = SelectedQuestion?.OptionB;
                        labelOptionC.Text = SelectedQuestion?.OptionC;
                        labelOptionD.Text = SelectedQuestion?.OptionD;

                        //Select the answer that has been selected (if so)
                        switch (SelectedQuestionAnswer.SelectedOption)
                        {
                            case MultiChoiceOption.A:
                                {
                                    bool flag = changesMade;
                                    rbOptionA_Click(sender, e);
                                    changesMade = flag;
                                    break;
                                }
                            case MultiChoiceOption.B:
                                {
                                    bool flag = changesMade;
                                    rbOptionB_Click(sender, e);
                                    changesMade = flag;
                                    break;
                                }
                            case MultiChoiceOption.C:
                                {
                                    bool flag = changesMade;
                                    rbOptionC_Click(sender, e);
                                    changesMade = flag;
                                    break;
                                }
                            case MultiChoiceOption.D:
                                {
                                    bool flag = changesMade;
                                    rbOptionD_Click(sender, e);
                                    changesMade = flag;
                                    break;
                                }
                            default:
                                rbOptionA.Checked = false;
                                rbOptionB.Checked = false;
                                rbOptionC.Checked = false;
                                rbOptionD.Checked = false;
                                break;
                        }
                        break;
                    }
                case AnswerType.Open:
                    {
                        panelAnswerLongContainer.Enabled = true;
                        panelAnswerLongContainer.Visible = true;

                        panelAnswerShortContainer.Enabled = false;
                        panelAnswerShortContainer.Visible = false;

                        tlpMultiAnswerContainer.Enabled = false;
                        tlpMultiAnswerContainer.Visible = false;

                        labelAnswerText.Visible = true;

                        //Stop the changesMade flag from being flipped by changing the answer text, this stops it autosaving every time the user goes to another question.
                        bool flag = changesMade;
                        //Show the entered answer
                        rtbAnswerLong.Text = SelectedQuestionAnswer.LongAnswer;
                        changesMade = flag;
                        break;
                    }
                case AnswerType.Single:
                    {
                        panelAnswerLongContainer.Enabled = false;
                        panelAnswerLongContainer.Visible = false;

                        panelAnswerShortContainer.Enabled = true;
                        panelAnswerShortContainer.Visible = true;

                        tlpMultiAnswerContainer.Enabled = false;
                        tlpMultiAnswerContainer.Visible = false;

                        labelAnswerText.Visible = true;

                        bool flag = changesMade;
                        // Show the entered answer
                        textBoxAnswerShort.Text = SelectedQuestionAnswer.ShortAnswer;
                        changesMade = flag;
                        break;
                    }
            }
            #endregion

            //Show or hide the image button
            if (SelectedQuestion.Image != null)
            {
                btnQuestionImage.Enabled = true;
                btnQuestionImage.Visible = true;

                //Show the image if not already shown
                if (!ImageTracker.ImageDisplayShown(SelectedQuestion.Name))
                {
                    ImageDisplay id = new ImageDisplay(SelectedQuestion.Name, SelectedQuestion.Image);
                    id.Show();
                    id.TopMost = true;
                    id.BringToFront();
                }
                else
                {
                    ImageTracker.FindImageDisplay(SelectedQuestion.Name).Focus();
                }
            }
            else
            {
                btnQuestionImage.Enabled = false;
                btnQuestionImage.Visible = false;
            }

            //Update unanswered questions
            listBoxUnansweredQuestions.Items.Clear();
            listBoxUnansweredQuestions.Items.AddRange(UnattemptedQuestions.ToArray());

            //Do an autosave if changes have been made and the assessment is a proper assessment (not a practice)
            if (Script.Published && changesMade)
                SaveToFile(true);
        }

        private void Examinee_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CurMode == Mode.Assessment)
            {
                if (!submitButtonPushed && DoSubmit() == DialogResult.No)
                    e.Cancel = true;
            }
            else
            {
                if (!submitButtonPushed && DoSubmit() == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void Examinee_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Examinee_KeyDown(object sender, KeyEventArgs e)
        {
            //Shortcut to previous question
            if (e.KeyCode == Keys.F4)
            {
                QuestionNode node = SelectedNode;
                if (node != null)
                {
                    if (node.PrevVisibleNode != null)
                        treeViewQuestionDisplay.SelectedNode = node.PrevVisibleNode;
                }
                treeViewQuestionDisplay.Focus();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                QuestionNode node = SelectedNode;
                if (node != null)
                {
                    if (node.NextVisibleNode != null)
                        treeViewQuestionDisplay.SelectedNode = node.NextVisibleNode;
                }
                treeViewQuestionDisplay.Focus();
                e.Handled = true;
            }
        }

        #endregion
    }
}
