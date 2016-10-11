using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static AssessmentManager.CONSTANTS;

namespace AssessmentManager
{
    public partial class MainForm : Form
    {
        private Assessment assessment;
        private FileInfo assessmentFile;
        private ColorDialog colorDialog = new ColorDialog();
        private SaveFileDialog xmlSaveFileDialog = new SaveFileDialog();
        private SaveFileDialog mainSaveFileDialog = new SaveFileDialog();
        private SaveFileDialog pdfSaveFileDialog = new SaveFileDialog();
        private OpenFileDialog openFileDialog = new OpenFileDialog();
        private OpenFileDialog addFilesDialog = new OpenFileDialog();
        private FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
        private CourseManager CourseManager = new CourseManager();

        private string DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public const int MaxNumSubQuestionLevels = 3;

        private bool designerChangesMade = false;
        private bool courseEdited = false;
        private bool reloadCourses = false;
        private bool publishPrepared = false;
        private bool suppressCBEvent = false;
        private bool markingChangesMade = false;
        private bool suppressMarkingSave = false;
        private Course courseRevertPoint;
        private CourseNode prevNode;
        private AssessmentSession markSession = null;

        private DateTimePicker dtpPublishTimeStudent;
        private NumericUpDown nudAssessmentTimeStudent;

        public MainForm()
        {
            InitializeComponent();

            NotifyAssessmentClosed();

            //Initialise the xml save file dialog
            xmlSaveFileDialog.Filter = XML_FILTER;
            xmlSaveFileDialog.DefaultExt = XML_EXT.Remove(0, 1);
            xmlSaveFileDialog.InitialDirectory = DESKTOP_PATH;

            //Initialise main save file dialog
            mainSaveFileDialog.Filter = ASSESSMENT_FILTER;
            mainSaveFileDialog.DefaultExt = ASSESSMENT_EXT.Remove(0, 1);

            //Initialise the pdf save file dialog
            pdfSaveFileDialog.Filter = PDF_FILTER;
            pdfSaveFileDialog.DefaultExt = PDF_EXT.Remove(0, 1);
            pdfSaveFileDialog.InitialDirectory = DESKTOP_PATH;

            //Initialise open file dialog
            openFileDialog.InitialDirectory = DESKTOP_PATH;
            openFileDialog.Filter = ASSESSMENT_FILTER;
            openFileDialog.DefaultExt = ASSESSMENT_EXT.Remove(0, 1);

            //Initialise the recent files menu
            UpdateRecentFiles();

            //Initialise the font combo boxes
            InitialiseFontComboBoxes();

            //Do the initialisation for the course tab
            InitialiseCourseTab();

            //Initialise publishing tab
            InitialisePublishTab();

            //Initialise the mark tab
            MarkSession = null;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dtpPublishTimeStudent = new DateTimePicker();
            dtpPublishTimeStudent.Format = DateTimePickerFormat.Time;
            dtpPublishTimeStudent.Visible = false;
            dtpPublishTimeStudent.ShowUpDown = true;
            dtpPublishTimeStudent.ValueChanged += dtpPublishTimeStudent_ValueChanged;
            dgvPublishStudents.Controls.Add(dtpPublishTimeStudent);

            nudAssessmentTimeStudent = new NumericUpDown();
            nudAssessmentTimeStudent.Minimum = 0;
            nudAssessmentTimeStudent.Maximum = 1000;
            nudAssessmentTimeStudent.Visible = false;
            nudAssessmentTimeStudent.ValueChanged += nudAssessmentTimeStudent_ValueChanged;
            dgvPublishStudents.Controls.Add(nudAssessmentTimeStudent);
        }

        public Assessment Assessment
        {
            get { return assessment; }
            private set { assessment = value; }
        }

        public bool HasAssessmentOpen
        {
            get { return Assessment != null; }
        }

        #region Designer

        public bool DesignerChangesMade
        {
            get
            {
                return HasAssessmentOpen && designerChangesMade;
            }
            set
            {
                designerChangesMade = value;
                if (DesignerChangesMade)
                {
                    if (!Text.Contains("*"))
                        this.Text = this.Text + "*";
                }
                else
                    this.Text = this.Text.Replace("*", "");
            }
        }

        #region Toolstrip buttons
        private void toolStripButtonColour_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                toolStripButtonColour.BackColor = colorDialog.Color;
                richTextBoxQuestion.SelectionColor = colorDialog.Color;
            }
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxQuestion.Cut();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxQuestion.Copy();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBoxQuestion.Paste();
        }

        private void toolStripButtonBold_Click(object sender, EventArgs e)
        {
            FontStyle newStyle;

            if (richTextBoxQuestion.SelectionFont.Style.HasFlag(FontStyle.Bold))
            {
                newStyle = richTextBoxQuestion.SelectionFont.Style & ~FontStyle.Bold;
            }
            else
            {
                newStyle = richTextBoxQuestion.SelectionFont.Style | FontStyle.Bold;
            }
            richTextBoxQuestion.SelectionFont = new Font(richTextBoxQuestion.SelectionFont, newStyle);
        }

        private void toolStripButtonItalic_Click(object sender, EventArgs e)
        {
            FontStyle newStyle;

            if (richTextBoxQuestion.SelectionFont.Style.HasFlag(FontStyle.Italic))
            {
                newStyle = richTextBoxQuestion.SelectionFont.Style & ~FontStyle.Italic;
            }
            else
            {
                newStyle = richTextBoxQuestion.SelectionFont.Style | FontStyle.Italic;
            }
            richTextBoxQuestion.SelectionFont = new Font(richTextBoxQuestion.SelectionFont, newStyle);
        }

        private void toolStripButtonUnderline_Click(object sender, EventArgs e)
        {
            FontStyle newStyle;

            if (richTextBoxQuestion.SelectionFont.Style.HasFlag(FontStyle.Underline))
            {
                newStyle = richTextBoxQuestion.SelectionFont.Style & ~FontStyle.Underline;
            }
            else
            {
                newStyle = richTextBoxQuestion.SelectionFont.Style | FontStyle.Underline;
            }
            richTextBoxQuestion.SelectionFont = new Font(richTextBoxQuestion.SelectionFont, newStyle);
        }

        private void toolStripButtonAlignLeft_Click(object sender, EventArgs e)
        {
            richTextBoxQuestion.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void toolStripButtonAlignCentre_Click(object sender, EventArgs e)
        {
            richTextBoxQuestion.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void toolStripButtonAlignRight_Click(object sender, EventArgs e)
        {
            richTextBoxQuestion.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void toolStripButtonBulletList_Click(object sender, EventArgs e)
        {
            richTextBoxQuestion.SelectionBullet = !richTextBoxQuestion.SelectionBullet;
        }

        private void toolStripComboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBoxQuestion.SelectionFont = new Font(toolStripComboBoxFont.Text, richTextBoxQuestion.SelectionFont.Size, richTextBoxQuestion.SelectionFont.Style);
            }
            catch
            {
            }
        }

        private void toolStripComboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                richTextBoxQuestion.SelectionFont = new Font(richTextBoxQuestion.SelectionFont.Name, float.Parse(toolStripComboBoxSize.Text), richTextBoxQuestion.SelectionFont.Style);
            }
            catch
            {
            }
        }
        #endregion

        #region MenuStripItems

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseAssessment() == DialogResult.OK)
            {
                //Prompt to do initial save here
                Assessment = new Assessment();
                Assessment.AddQuestion("Question 1");
                //Prompt the user to do an initial save here. This is to set up the path and allow for autosaving
                MessageBox.Show("Please do an initial save. This will allow the program to perform autosaves.", "Initial save");
                if (SaveToFile() == DialogResult.OK)
                {
                    NotifyAssessmentOpened();
                    DesignerChangesMade = true;
                }
                else
                {
                    Assessment = null;
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAssessment();
        }

        private void exportToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasAssessmentOpen && xmlSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var stream = new FileStream(xmlSaveFileDialog.FileName, FileMode.Create))
                {
                    var xml = new XmlSerializer(typeof(Assessment));
                    xml.Serialize(stream, Assessment);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasAssessmentOpen)
            {
                if (assessmentFile == null)
                {
                    mainSaveFileDialog.InitialDirectory = DefaultPath;
                    SaveToFile();
                }
                else
                {
                    SaveToFile(assessmentFile.FullName);
                }
            }
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasAssessmentOpen)
            {
                SaveToFile();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseAssessment() == DialogResult.OK)
            {
                OpenFromFile();
            }
        }

        private void checkForQuestionsWithoutMarksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasAssessmentOpen)
            {
                List<Question> list = Assessment.CheckMissingMarks();
                if (list.Count > 0)
                {
                    list.Sort((a, b) => a.Name.CompareTo(b.Name));

                    string questions = "";
                    foreach (var q in list)
                        questions += q.Name + "\n";

                    MessageBox.Show("These questions do not have any marks assigned: \n\n" + questions, "Unassigned marks");
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseAssessment() == DialogResult.OK)
                Close();
        }

        private void withAnswersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakePdf(true);
        }

        private void withoutAnswersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MakePdf(false);
        }

        #endregion

        #region TreeViewButtons
        private void buttonAddMajorQuestion_Click(object sender, EventArgs e)
        {
            QuestionNode node = new QuestionNode(new Question("unnamed"));
            treeViewQuestionList.Nodes.Add(node);
            Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
            DesignerChangesMade = true;
            treeViewQuestionList.SelectedNode = node;
            treeViewQuestionList.Focus();
        }

        private void buttonAddSubQuestion_Click(object sender, EventArgs e)
        {
            if (treeViewQuestionList.SelectedNode != null)
            {
                QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;

                //Check the node is able to have sub nodes(sub questions)
                if (node.Level >= MaxNumSubQuestionLevels - 1)
                {
                    treeViewQuestionList.Focus();
                    return;
                }

                Question subQ = new Question("unnamed");

                node.Nodes.Add(new QuestionNode(subQ));
                node.Expand();
                Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                DesignerChangesMade = true;
            }
            treeViewQuestionList.Focus();
        }

        private void buttonExpandAll_Click(object sender, EventArgs e)
        {
            treeViewQuestionList.ExpandAll();
            treeViewQuestionList.Focus();
        }

        private void buttonCollapseAll_Click(object sender, EventArgs e)
        {
            treeViewQuestionList.CollapseAll();
            treeViewQuestionList.Focus();
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null && node.CanMoveUp)
            {
                try
                {
                    int indexToInsertTo = node.Index - 1;
                    if (indexToInsertTo < 0) indexToInsertTo = 0;
                    TreeNodeCollection collection = node.Parent != null ? node.Parent.Nodes : node.TreeView.Nodes;
                    node.Remove();
                    collection.Insert(indexToInsertTo, node);
                    Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                    treeViewQuestionList.SelectedNode = node;
                    DesignerChangesMade = true;
                }
                catch { }
            }
            treeViewQuestionList.Focus();
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null && node.CanMoveDown)
            {
                try
                {
                    int indexToInsertTo = node.Index + 1;
                    TreeNodeCollection collection = node.Parent != null ? node.Parent.Nodes : node.TreeView.Nodes;
                    node.Remove();
                    collection.Insert(indexToInsertTo, node);
                    Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                    treeViewQuestionList.SelectedNode = node;
                    DesignerChangesMade = true;
                }
                catch { }
            }
            treeViewQuestionList.Focus();
        }
        #endregion

        #region Methods
        private void NotifyAssessmentClosed()
        {
            //Disable buttons
            panelButtons.Enabled = false;
            //Disable question editing area
            tableLayoutPanelDesignerContainer.Enabled = false;
            //Clear the question text
            richTextBoxQuestion.Text = "";
            //Clear the question name
            labelQuestion.Text = "";
            //Disable marks assign thingy
            numericUpDownMarksAssigner.Enabled = false;
            //Hide marks assignment information
            groupBoxMarks.Visible = false;
            //Need to also hide these two because they don't get set unless there is a parent question (In method UpdateMarkAllocations)
            labelMarksSelectedQuestionParentParent.Visible = false;
            labelMarksSelectedQuestionParentParentNum.Visible = false;
            //Disable treeview
            treeViewQuestionList.Enabled = false;
            //Disable menustrip buttons
            checkForQuestionsWithoutMarksToolStripMenuItem.Enabled = false;
            makePdfOfExamToolStripMenuItem.Enabled = false;
            exportToXMLToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            saveasToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;
            //Reset the fileinfo
            assessmentFile = null;
            //Reset the form text
            UpdateFormText();
            //Reset the publish screen
            ResetPublishTab();
        }

        private void NotifyAssessmentOpened()
        {
            //Enable buttons
            panelButtons.Enabled = true;
            //Enable question editing area
            tableLayoutPanelDesignerContainer.Enabled = true;
            //Enable marks assign thingy
            numericUpDownMarksAssigner.Enabled = true;
            //Show marks assignment information
            groupBoxMarks.Visible = true;
            //Enable treeview
            treeViewQuestionList.Enabled = true;
            //Enable menustrip buttons
            checkForQuestionsWithoutMarksToolStripMenuItem.Enabled = true;
            makePdfOfExamToolStripMenuItem.Enabled = true;
            exportToXMLToolStripMenuItem.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            saveasToolStripMenuItem.Enabled = true;
            closeToolStripMenuItem.Enabled = true;
            //Populate the treeview with the questions from the assessment
            Util.PopulateTreeView(treeViewQuestionList, Assessment);
            if (treeViewQuestionList.Nodes.Count > 0) treeViewQuestionList.SelectedNode = treeViewQuestionList.Nodes[0];
            //No changes will have been made yet
            DesignerChangesMade = false;
            //Change form text
            UpdateFormText();
            //Setup publish tab
            SetPublishTab();
        }

        private void InitialiseFontComboBoxes()
        {
            for (int i = 8; i <= 75; i++)
            {
                toolStripComboBoxSize.Items.Add(i.ToString());
            }

            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (var f in fonts.Families)
            {
                toolStripComboBoxFont.Items.Add(f.Name);
            }
        }

        /// <summary>
        /// Closes the open assessment.
        /// </summary>
        /// <returns>Returns DialogResult.Cancel if the user cancels saving and closing the document. Returns DialogResult.OK if it closes the document.</returns>
        private DialogResult CloseAssessment()
        {
            if (DesignerChangesMade)
            {
                DialogResult result = MessageBox.Show("Changes have been made to this Assessment. Closing it now will cause those changes to be lost. Would you like to save before closing?", "Unsaved changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    if (assessmentFile == null)
                    {
                        if (SaveToFile() == DialogResult.Cancel)
                            return DialogResult.Cancel;
                    }
                    else
                    {
                        SaveToFile(assessmentFile.FullName);
                    }
                }
                else if (result == DialogResult.Cancel)
                    return DialogResult.Cancel;
            }
            Assessment = null;
            treeViewQuestionList.Nodes.Clear();
            NotifyAssessmentClosed();
            return DialogResult.OK;
        }

        private bool DeleteNode(QuestionNode node)
        {
            if (node != null && MessageBox.Show("Are you sure you want to delete this question?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    node.Remove();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return false;
                }
                Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                return true;
            }
            return false;
        }

        private void UpdateMarkAllocations()
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                Question q = node.Question;
                try
                {
                    //Display the marks for the whole assessment
                    labelMarksWholeAssessmentNum.Text = Assessment.TotalMarks.ToString();
                    //Display the marks for the selected question
                    labelMarksSelectedQuestion.Text = q.Name + ":";
                    //labelMarksSelectedQuestionNum.Text = q.Marks.ToString();
                    labelMarksSelectedQuestionNum.Text = q.Marks.ToString() + $" ({q.TotalMarks.ToString()} total)";

                    //If the question has a parent, display total marks for that question
                    if (node.Parent != null)
                    {
                        Question parentQ = ((QuestionNode)node.Parent).Question;

                        labelMarksSelectedQuestionParent.Text = parentQ.Name + ":";
                        //labelMarksSelectedQuestionParentNum.Text = parentQ.TotalMarks.ToString();
                        labelMarksSelectedQuestionParentNum.Text = parentQ.Marks.ToString() + $" ({parentQ.TotalMarks.ToString()} total)";

                        labelMarksSelectedQuestionParent.Visible = true;
                        labelMarksSelectedQuestionParentNum.Visible = true;

                        //If there is another level of questions above the parent, display those
                        if (node.Parent.Parent != null)
                        {
                            Question parentParentQ = ((QuestionNode)node.Parent.Parent).Question;

                            labelMarksSelectedQuestionParentParent.Text = parentParentQ.Name + ":";
                            //labelMarksSelectedQuestionParentParentNum.Text = parentParentQ.TotalMarks.ToString();
                            labelMarksSelectedQuestionParentParentNum.Text = parentParentQ.Marks.ToString() + $" ({parentParentQ.TotalMarks.ToString()} total)";

                            labelMarksSelectedQuestionParentParent.Visible = true;
                            labelMarksSelectedQuestionParentParentNum.Visible = true;
                        }
                        else
                        {
                            labelMarksSelectedQuestionParentParent.Visible = false;
                            labelMarksSelectedQuestionParentParentNum.Visible = false;
                        }
                    }
                    else
                    {
                        //Disable parent marks text boxes if there is no parent
                        labelMarksSelectedQuestionParent.Visible = false;
                        labelMarksSelectedQuestionParentNum.Visible = false;

                        //Also disable the parent parent text
                        labelMarksSelectedQuestionParentParent.Visible = false;
                        labelMarksSelectedQuestionParentParentNum.Visible = false;
                    }
                }
                catch
                {
                }
            }
        }

        private void UpdateFormText()
        {
            Text = assessmentFile == null ? "Assessment Designer" : "Assessment Designer - " + assessmentFile.Name;
        }

        private void UpdateRecentFiles()
        {
            recentToolStripMenuItem.DropDownItems.Clear();
            foreach (var path in Settings.Instance.RecentFiles)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = path;
                item.Tag = path;
                item.Size = new Size(100, 22);
                item.Click += (sender, e) =>
                {
                    if (DesignerChangesMade)
                    {
                        DialogResult result = MessageBox.Show("There are unsaved changes. Do you wish to save before opening a new file?", "Unsaved changes", MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes)
                        {
                            if (assessmentFile == null)
                            {
                                if (SaveToFile() == DialogResult.Cancel)
                                    return;
                            }
                            else
                                SaveToFile(assessmentFile.FullName);
                        }
                        else if (result == DialogResult.Cancel)
                            return;
                    }
                    OpenFromFile(item.Tag.ToString());
                };
                recentToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// Save the currently open Assessment to file. Does not display SaveFileDialog, but instead is given the path to save to.
        /// </summary>
        /// <param name="path">The specified path to save the Assessment to.</param>
        private void SaveToFile(string path)
        {
            //Save the file here
            if (HasAssessmentOpen)
            {
                try
                {
                    using (FileStream s = File.Open(path, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(s, Assessment);
                    }
                    assessmentFile = new FileInfo(path);
                    DesignerChangesMade = false;
                    UpdateFormText();
                    Settings.Instance.AddRecentFile(path);
                    UpdateRecentFiles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save: \n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Save the currently open Assessment to file. Displays the SaveFileDialog
        /// </summary>
        private DialogResult SaveToFile()
        {
            if (HasAssessmentOpen)
            {
                DialogResult result = mainSaveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SaveToFile(mainSaveFileDialog.FileName);
                }
                return result;
            }
            return DialogResult.Cancel;
        }

        /// <summary>
        /// Open an Assessment from file. Does not show an OpenFileDialog.
        /// </summary>
        /// <param name="path">The specified path for the file</param>
        public void OpenFromFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream s = File.Open(path, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        Assessment = (Assessment)formatter.Deserialize(s);
                    }
                    assessmentFile = new FileInfo(path);
                    NotifyAssessmentOpened();
                    Settings.Instance.AddRecentFile(path);
                    UpdateRecentFiles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to load file: \n" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show($"Unable to find the file at: {path}\n\n Failed to open.");
            }
        }

        /// <summary>
        /// Open an Assessment from file. Displays OpenFileDialog.
        /// </summary>
        public void OpenFromFile()
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFromFile(openFileDialog.FileName);
            }
        }

        public void MakePdf(bool withAnswers)
        {
            if (Assessment == null)
            {
                MessageBox.Show("Unable to make pdf: Assessment is null", "Error");
                return;
            }
            if (Assessment.Questions.Count == 0)
            {
                MessageBox.Show("Unable to make pdf: Assessment has no questions", "Error");
                return;
            }
            if (pdfSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                AssessmentInformationForm aif = new AssessmentInformationForm();
                if (aif.ShowDialog() == DialogResult.OK)
                {
                    AssessmentWriter w = new AssessmentWriter(Assessment, aif.AssessmentInformation, pdfSaveFileDialog.FileName);
                    if (w.MakePdf(withAnswers))
                    {
                        if (MessageBox.Show("PDF successfully created. Would you like to view it now?", "PDF created", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Process.Start(pdfSaveFileDialog.FileName);
                        }
                    }
                }
            }
        }
        #endregion

        #region TreeView Events
        private void treeViewQuestionList_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //This opens the context menu for the node the user right-clicked.
                //Get the point where the user clicked
                Point p = new Point(e.X, e.Y);

                //Get the node that the user clicked
                QuestionNode node = (QuestionNode)treeViewQuestionList.GetNodeAt(p);
                if (node != null)
                {
                    //Select the node
                    treeViewQuestionList.SelectedNode = node;

                    //Configure the context menu for the given node

                    //Disable the paste option if there is nothing to paste
                    IDataObject data = Clipboard.GetDataObject();
                    if (data.GetDataPresent(QUESTION_FORMAT_STRING))
                    {
                        contextMenuNodePaste.Visible = true;
                        contextMenuNodePaste.Enabled = true;
                    }
                    else
                    {
                        contextMenuNodePaste.Visible = false;
                        contextMenuNodePaste.Enabled = false;
                    }


                    //Disable the move up if it is at the top
                    bool flag1 = false, flag2 = false;
                    if (!node.CanMoveUp)
                    {
                        contextMenuMoveUp.Visible = false;
                        flag1 = true;
                    }
                    else
                    {
                        contextMenuMoveUp.Visible = true;
                    }
                    //Disable move down if it is at the bottom
                    if (!node.CanMoveDown)
                    {
                        contextMenuMoveDown.Visible = false;
                        flag2 = true;
                    }
                    else
                    {
                        contextMenuMoveDown.Visible = true;
                    }
                    //Disable the separator if both move up and move down are disabled
                    if (flag1 && flag2)
                    {
                        contextMenuSeparatorMove.Visible = false;
                    }
                    else
                        contextMenuSeparatorMove.Visible = true;


                    //Disable change level up if already top level
                    bool levelFlag1 = false, levelFlag2 = false;
                    if (node.Level == 0)
                    {
                        contextMenuChangeLevelUp.Visible = false;
                        levelFlag1 = true;
                    }
                    else
                        contextMenuChangeLevelUp.Visible = true;

                    //Disable change level down if there is a limit on how many levels there are
                    if (node.Level == MaxNumSubQuestionLevels - 1)
                    {
                        contextMenuChangeLevelDown.Visible = false;
                        levelFlag2 = true;
                    }
                    else
                        contextMenuChangeLevelDown.Visible = true;

                    //Disable the separator if both are hidden
                    if (levelFlag1 && levelFlag2)
                        contextMenuSeparatorChangeLevel.Visible = false;
                    else
                        contextMenuSeparatorChangeLevel.Visible = true;


                    //Disable add sub question if question is at max level
                    bool subQuestionFlag = false;
                    if (node.Level >= MaxNumSubQuestionLevels - 1)
                    {
                        contextMenuAddSubQuestion.Visible = false;
                        subQuestionFlag = true;
                    }
                    else
                        contextMenuAddSubQuestion.Visible = true;
                    //Disable the separator
                    if (subQuestionFlag)
                        contextMenuSeparatorSubQuestion.Visible = false;
                    else
                        contextMenuSeparatorSubQuestion.Visible = true;

                    //Show the contextmenu
                    contextMenuStripQuestionNode.Show(treeViewQuestionList, p);
                }
            }
        }

        private void treeViewQuestionList_KeyDown(object sender, KeyEventArgs e)
        {
            if (treeViewQuestionList.ContainsFocus && e.KeyData == Keys.Delete)
            {
                QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
                if (node != null)
                {
                    DeleteNode(node);
                }
            }
        }

        private void treeViewQuestionList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            QuestionNode node = (QuestionNode)e.Node;
            if (node != null)
            {
                bool flag = designerChangesMade;
                //Display the question's name
                labelQuestion.Text = node.Question.Name;
                //Display the question's text
                richTextBoxQuestion.Rtf = node.Question.QuestionText;
                //Hide the marks assigner if the quesiton doesn't have an answer
                if (node.Question.AnswerType == AnswerType.None)
                {
                    labelMarksForQuestion.Visible = false;
                    numericUpDownMarksAssigner.Visible = false;
                }
                else
                {
                    labelMarksForQuestion.Visible = true;
                    numericUpDownMarksAssigner.Visible = true;
                    //Display the marks in the numeric up/down
                    numericUpDownMarksAssigner.Value = node.Question.Marks;
                }
                //Update the mark allocations
                UpdateMarkAllocations();

                //Update the font combo boxes
                toolStripComboBoxFont.SelectedItem = richTextBoxQuestion.SelectionFont.Name;
                toolStripComboBoxSize.SelectedItem = ((int)richTextBoxQuestion.SelectionFont.Size).ToString();
                //Update the colour button
                toolStripButtonColour.BackColor = richTextBoxQuestion.SelectionColor;

                //Display the question's answer type
                switch (node.Question.AnswerType)
                {
                    case AnswerType.Multi:
                        {
                            comboBoxAnswerType.SelectedItem = "Multi-choice";
                            //Display the answers in the boxes
                            textBoxMultiChoiceA.Text = node.Question.OptionA;
                            textBoxMultiChoiceB.Text = node.Question.OptionB;
                            textBoxMultiChoiceC.Text = node.Question.OptionC;
                            textBoxMultiChoiceD.Text = node.Question.OptionD;
                            break;
                        }
                    case AnswerType.Single:
                        {
                            comboBoxAnswerType.SelectedItem = "Single";
                            //Display the answers
                            richTextBoxAnswerSingleAcceptable.Text = node.Question.ModelAnswer;
                            break;
                        }
                    case AnswerType.Open:
                        {
                            comboBoxAnswerType.SelectedItem = "Open";
                            //Display the answer
                            richTextBoxAnswerOpen.Text = node.Question.ModelAnswer;
                            break;
                        }
                    case AnswerType.None:
                        {
                            comboBoxAnswerType.SelectedItem = "None";
                            break;
                        }
                }
                //Display the correct multi choice option
                comboBoxAnswerMultiCorrect.SelectedItem = node.Question.CorrectOption.ToString();
                //Disable the subquestion button if the question cannot have any more subquestions
                if (node.Level >= MaxNumSubQuestionLevels - 1)
                    buttonAddSubQuestion.Enabled = false;
                else
                    buttonAddSubQuestion.Enabled = true;
                DesignerChangesMade = flag;
            }
        }
        #endregion

        #region ContextMenu Events
        private void contextMenuDelete_Click(object sender, EventArgs e)
        {
            DeleteNode((QuestionNode)treeViewQuestionList.SelectedNode);
        }

        private void contextMenuInsertAbove_Click(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                int indexToInsertTo = node.Index - 1;
                if (indexToInsertTo < 0) indexToInsertTo = 0;
                if (node.Parent != null)
                {
                    node.Parent.Nodes.Insert(indexToInsertTo, new QuestionNode(new Question("unnamed")));
                    Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                }
                else
                {
                    treeViewQuestionList.Nodes.Insert(indexToInsertTo, new QuestionNode(new Question("unnamed")));
                    Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                }
                treeViewQuestionList.SelectedNode = null;
                treeViewQuestionList.SelectedNode = node;
            }
            treeViewQuestionList.Focus();
        }

        private void contextMenuInsertBelow_Click(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                int indexToInsertTo = node.Index + 1;
                if (indexToInsertTo < 0) indexToInsertTo = 0;
                if (node.Parent == null)
                {
                    if (!node.CanMoveDown) buttonAddMajorQuestion_Click(sender, e);
                    else
                    {
                        treeViewQuestionList.Nodes.Insert(indexToInsertTo, new QuestionNode(new Question("unnamed")));
                        Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                    }
                }
                else
                {
                    if (!node.CanMoveDown)
                    {
                        node.Parent.Nodes.Add(new QuestionNode(new Question("unnamed")));
                        Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                    }
                    else
                    {
                        node.Parent.Nodes.Insert(indexToInsertTo, new QuestionNode(new Question("unnamed")));
                        Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                    }
                }
                treeViewQuestionList.SelectedNode = null;
                treeViewQuestionList.SelectedNode = node;
            }
            treeViewQuestionList.Focus();
        }

        private void contextMenuMoveUp_Click(object sender, EventArgs e)
        {
            buttonMoveUp_Click(sender, e);
        }

        private void contextMenuMoveDown_Click(object sender, EventArgs e)
        {
            buttonMoveDown_Click(sender, e);
        }

        private void contextMenuAddSubQuestion_Click(object sender, EventArgs e)
        {
            buttonAddSubQuestion_Click(sender, e);
        }

        private void contextMenuChangeLevelUp_Click(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null && node.Level != 0)
            {
                //Two possibilities here:
                //A node will be second level, meaning its parent does not have a parent
                //Or a node's parent will also have a parent. So do two different things based on this fact.

                //Node's parent is top level:
                if (node.Parent.Level == 0)
                {
                    int index = node.Parent.Index + 1;
                    node.Remove();
                    treeViewQuestionList.Nodes.Insert(index, node);
                }
                else
                {
                    //Node's parent has a parent here
                    QuestionNode nodeToAddTo = (QuestionNode)node.Parent.Parent;
                    try
                    {
                        int index = node.Parent.Index + 1;
                        node.Remove();
                        nodeToAddTo.Nodes.Insert(index, node);
                    }
                    catch { }
                }
                treeViewQuestionList.SelectedNode = node;
                Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
            }
        }

        private void contextMenuChangeLevelDown_Click(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                if (node.Parent != null)
                {
                    QuestionNode parent = (QuestionNode)node.Parent;
                    int index = node.Index;
                    node.Remove();
                    QuestionNode newRootNode = new QuestionNode(new Question("unnamed"));
                    parent.Nodes.Insert(index, newRootNode);
                    newRootNode.Nodes.Add(node);
                    newRootNode.Expand();
                    Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                }
                else
                {
                    int index = node.Index;
                    QuestionNode newRootNode = new QuestionNode(new Question("unnamed"));
                    node.Remove();
                    treeViewQuestionList.Nodes.Insert(index, newRootNode);
                    newRootNode.Nodes.Add(node);
                    newRootNode.Expand();
                    Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                }
                treeViewQuestionList.SelectedNode = node;
            }
        }

        private void contextMenuCopyQuestion_Click(object sender, EventArgs e)
        {
            if (treeViewQuestionList.SelectedNode != null)
            {
                QuestionNode node = treeViewQuestionList.SelectedNode as QuestionNode;
                if (node != null)
                {
                    IDataObject dataObj = new DataObject();
                    dataObj.SetData(QUESTION_FORMAT_STRING, false, node.Question);

                    Clipboard.SetDataObject(dataObj, false);
                }
            }
        }

        private void contextMenuStripQuestionList_Opening(object sender, CancelEventArgs e)
        {
            //If there is paste data present, show the paste option
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(QUESTION_FORMAT_STRING))
            {
                contextMenuQuestionListPaste.Visible = true;
                contextMenuQuestionListPaste.Enabled = true;
                contextMenuQuestionListPasteSeparator.Visible = true;
            }
            else
            {
                contextMenuQuestionListPaste.Enabled = false;
                contextMenuQuestionListPaste.Visible = false;
                contextMenuQuestionListPasteSeparator.Visible = false;
            }
        }

        private void contextMenuQuestionListPaste_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(QUESTION_FORMAT_STRING))
            {
                Question copiedQuestion = data.GetData(QUESTION_FORMAT_STRING) as Question;
                if (copiedQuestion != null)
                {
                    QuestionNode newNode = new QuestionNode(copiedQuestion.Clone());
                    treeViewQuestionList.Nodes.Add(newNode);
                    Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                    DesignerChangesMade = true;
                    treeViewQuestionList.SelectedNode = newNode;
                    treeViewQuestionList.Focus();
                }
            }
        }

        private void contextMenuNodePaste_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(QUESTION_FORMAT_STRING))
            {
                Question copiedQuestion = data.GetData(QUESTION_FORMAT_STRING) as Question;
                if (copiedQuestion != null)
                {
                    QuestionNode node = treeViewQuestionList.SelectedNode as QuestionNode;
                    if (node != null)
                    {
                        node.Question = copiedQuestion.Clone();
                        Util.RebuildAssessmentQuestionList(Assessment, treeViewQuestionList);
                        DesignerChangesMade = true;
                        treeViewQuestionList.SelectedNode = null;
                        treeViewQuestionList.SelectedNode = node;
                        treeViewQuestionList.Focus();
                    }
                }
            }

        }
        #endregion

        #region QuestionEditingControls
        private void comboBoxAnswerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HasAssessmentOpen)
            {
                QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
                if (node != null)
                {
                    switch (comboBoxAnswerType.Text)
                    {
                        case "None":
                            {
                                node.Question.AnswerType = AnswerType.None;
                                //Hide the marks assigner
                                labelMarksForQuestion.Visible = false;
                                numericUpDownMarksAssigner.Visible = false;
                                UpdateMarkAllocations();
                                //Hide answer label
                                labelAnswerText.Visible = false;
                                break;
                            }
                        case "Multi-choice":
                            {
                                node.Question.AnswerType = AnswerType.Multi;
                                //Display the answers in the boxes
                                textBoxMultiChoiceA.Text = node.Question.OptionA;
                                textBoxMultiChoiceB.Text = node.Question.OptionB;
                                textBoxMultiChoiceC.Text = node.Question.OptionC;
                                textBoxMultiChoiceD.Text = node.Question.OptionD;
                                //Show the marks assigner
                                labelMarksForQuestion.Visible = true;
                                numericUpDownMarksAssigner.Visible = true;
                                UpdateMarkAllocations();
                                //Show answer label
                                labelAnswerText.Visible = true;
                                break;
                            }
                        case "Single":
                            {
                                node.Question.AnswerType = AnswerType.Single;
                                //Display the answers
                                richTextBoxAnswerSingleAcceptable.Text = node.Question.ModelAnswer;
                                //Show the marks assigner
                                labelMarksForQuestion.Visible = true;
                                numericUpDownMarksAssigner.Visible = true;
                                UpdateMarkAllocations();
                                //Show answer label
                                labelAnswerText.Visible = true;
                                break;
                            }
                        case "Open":
                            {
                                node.Question.AnswerType = AnswerType.Open;
                                //Display the answer
                                richTextBoxAnswerOpen.Text = node.Question.ModelAnswer;
                                //Show the marks assigner
                                labelMarksForQuestion.Visible = true;
                                numericUpDownMarksAssigner.Visible = true;
                                UpdateMarkAllocations();
                                //Show answer label
                                labelAnswerText.Visible = true;
                                break;
                            }
                    }
                    DesignerChangesMade = true;
                }
            }
            switch (comboBoxAnswerType.Text)
            {
                case "None":
                    {
                        panelAnswerMultiChoice.Visible = false;
                        panelAnswerOpen.Visible = false;
                        panelAnswerSingle.Visible = false;

                        break;
                    }
                case "Multi-choice":
                    {
                        panelAnswerMultiChoice.Visible = true;
                        panelAnswerOpen.Visible = false;
                        panelAnswerSingle.Visible = false;

                        break;
                    }
                case "Open":
                    {
                        panelAnswerMultiChoice.Visible = false;
                        panelAnswerOpen.Visible = true;
                        panelAnswerSingle.Visible = false;

                        break;
                    }
                case "Single":
                    {
                        panelAnswerMultiChoice.Visible = false;
                        panelAnswerOpen.Visible = false;
                        panelAnswerSingle.Visible = true;

                        break;
                    }
            }
        }

        private void richTextBoxQuestion_TextChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.QuestionText = richTextBoxQuestion.Rtf;
                node.Question.QuestionTextRaw = richTextBoxQuestion.Text;
                DesignerChangesMade = true;
            }
        }

        private void richTextBoxQuestion_SelectionChanged(object sender, EventArgs e)
        {
            //Set the values of the font combo boxes to the selected font
            toolStripComboBoxFont.SelectedItem = richTextBoxQuestion.SelectionFont.Name;
            toolStripComboBoxSize.SelectedItem = ((int)richTextBoxQuestion.SelectionFont.Size).ToString();
            toolStripButtonColour.BackColor = richTextBoxQuestion.SelectionColor;
        }

        private void richTextBoxAnswerOpen_TextChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.ModelAnswer = richTextBoxAnswerOpen.Text;
                DesignerChangesMade = true;
            }
        }

        private void richTextBoxAnswerSingleAcceptable_TextChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.ModelAnswer = richTextBoxAnswerSingleAcceptable.Text;
                DesignerChangesMade = true;
            }
        }

        private void textBoxMultiChoiceA_TextChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.OptionA = textBoxMultiChoiceA.Text;
                DesignerChangesMade = true;
            }
        }

        private void textBoxMultiChoiceB_TextChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.OptionB = textBoxMultiChoiceB.Text;
                DesignerChangesMade = true;
            }
        }

        private void textBoxMultiChoiceC_TextChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.OptionC = textBoxMultiChoiceC.Text;
                DesignerChangesMade = true;
            }
        }

        private void textBoxMultiChoiceD_TextChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.OptionD = textBoxMultiChoiceD.Text;
                DesignerChangesMade = true;
            }
        }

        private void comboBoxAnswerMultiCorrect_SelectedIndexChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                switch (comboBoxAnswerMultiCorrect.Text)
                {
                    case "A":
                        {
                            node.Question.CorrectOption = MultiChoiceOption.A;
                            break;
                        }
                    case "B":
                        {
                            node.Question.CorrectOption = MultiChoiceOption.B;
                            break;
                        }
                    case "C":
                        {
                            node.Question.CorrectOption = MultiChoiceOption.C;
                            break;
                        }
                    case "D":
                        {
                            node.Question.CorrectOption = MultiChoiceOption.D;
                            break;
                        }
                }
                DesignerChangesMade = true;
            }
        }

        private void numericUpDownMarksAssigner_ValueChanged(object sender, EventArgs e)
        {
            QuestionNode node = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (node != null)
            {
                node.Question.Marks = (int)numericUpDownMarksAssigner.Value;
                DesignerChangesMade = true;
                UpdateMarkAllocations();
            }
        }
        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Check for changes made to a course and prompt to save them. If cancled then dont close!
            if (DesignerChangesMade)
            {
                DialogResult result = MessageBox.Show("Changes have been made to this Assessment. Closing it now will cause those changes to be lost. Would you like to save before closing?", "Unsaved changes", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    if (assessmentFile == null)
                    {
                        if (SaveToFile() == DialogResult.Cancel)
                            e.Cancel = true;
                    }
                    else
                        SaveToFile(assessmentFile.FullName);
                }
                else if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
            if (CourseEdited && tvCourses.SelectedNode != null && tvCourses.SelectedNode is CourseNode)
            {
                string message = "There have been changes made to a course. Closing now will cause those changes to be discarded. Do you wish to commit your changes before closing?";
                DialogResult result = MessageBox.Show(message, "Unsaved changes to course", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    ApplyCourseChanges();
                }
                else if (result == DialogResult.No)
                {
                    //Revert the changes
                    CourseNode node = tvCourses.SelectedNode as CourseNode;
                    node.Course = courseRevertPoint;
                    CourseEdited = false;
                    node.Text = node.Course.CourseTitle;
                    node.Name = node.Text;
                }
                else
                {
                    //Cancel the change
                    e.Cancel = true;
                }
            }
            Settings.Instance.Save();
        }

        private void toolStripButtonAddImage_Click(object sender, EventArgs e)
        {
            QuestionNode qn = (QuestionNode)treeViewQuestionList.SelectedNode;
            if (qn != null)
            {
                Question q = qn.Question;
                ImageSelector i;

                if (q.Image != null)
                {
                    i = new ImageSelector(q.Name, q.Image);
                }
                else
                {
                    i = new ImageSelector(q.Name);
                }

                if (i.ShowDialog() == DialogResult.OK)
                {
                    q.Image = i.Image;
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //Hotkey for next/prev question in designer tab
            if (tabControlMain.SelectedTab.Name == "tabPageDesigner")
            {
                if (e.KeyCode == Keys.F4)
                {
                    QuestionNode node = treeViewQuestionList.SelectedNode as QuestionNode;
                    if (node != null)
                    {
                        if (node.PrevVisibleNode != null)
                            treeViewQuestionList.SelectedNode = node.PrevVisibleNode;
                    }
                    treeViewQuestionList.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F5)
                {
                    QuestionNode node = treeViewQuestionList.SelectedNode as QuestionNode;
                    if (node != null)
                    {
                        if (node.NextVisibleNode != null)
                            treeViewQuestionList.SelectedNode = node.NextVisibleNode;
                    }
                    treeViewQuestionList.Focus();
                    e.Handled = true;
                }
            }
            //Hotkey for next/prev in marking tab
            if (tabControlMain.SelectedTab == tabPageMark)
            {
                if(e.KeyCode==Keys.F4)
                {
                    TreeNode node = tvMarkQuestions.SelectedNode;
                    if(node!=null)
                    {
                        if(node.PrevVisibleNode!=null)
                        {
                            tvMarkQuestions.SelectedNode = node.PrevVisibleNode;
                        }
                    }
                    tvMarkQuestions.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.F5)
                {
                    TreeNode node = tvMarkQuestions.SelectedNode;
                    if (node != null)
                    {
                        if (node.NextVisibleNode != null)
                        {
                            tvMarkQuestions.SelectedNode = node.NextVisibleNode;
                        }
                    }
                    tvMarkQuestions.Focus();
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region CourseManagerTab

        #region Properties

        private bool CourseEdited
        {
            get
            {
                return courseEdited;
            }
            set
            {
                courseEdited = value;
                btnApplyCourseChanges.Enabled = value;
                btnDiscardCourseChanges.Enabled = value;
                if (value)
                {
                    reloadCourses = true;
                }
            }
        }

        #endregion

        #region Methods

        private void InitialiseCourseTab()
        {
            //course and assessment session panels initially disabled and cannot be viewed
            pnlAssessmentView.Visible = false;
            pnlAssessmentView.Enabled = false;
            pnlCourseView.Visible = false;
            pnlCourseView.Enabled = false;
            //Initialise the course manager
            CourseManager.Initialise(tvCourses, CourseManager);
        }

        public void ApplyCourseChanges()
        {
            if (tvCourses.SelectedNode is CourseNode)
            {
                CourseNode cn = tvCourses.SelectedNode as CourseNode;
                //Clear the students list then reload from the dgv
                Course c = cn.Course;
                c.Students.Clear();
                if (dgvCourseStudents.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvCourseStudents.Rows)
                    {
                        if (row.Cells[0].Value == null && row.Cells[1].Value == null && row.Cells[2].Value == null && row.Cells[3].Value == null)
                            continue;

                        string userName = row.Cells[0].Value?.ToString();
                        string lastName = row.Cells[1].Value?.ToString();
                        string firstName = row.Cells[2].Value?.ToString();
                        string studentID = row.Cells[3].Value?.ToString();
                        Student s = new Student(userName, lastName, firstName, studentID);
                        c.Students.Add(s);
                    }
                }

                CourseManager.SerialiseCourse(cn.Course);
                CourseEdited = false;
                //Update the revert point
                courseRevertPoint = c.Clone();
            }
        }

        public void RevertCourseChanges()
        {
            //Revert the changes
            prevNode.Course = courseRevertPoint;
            CourseEdited = false;
            prevNode.Text = prevNode.Course.CourseTitle;
            prevNode.Name = prevNode.Text;
        }

        public void DeleteCourseNode(CourseNode node)
        {
            //First make sure the user wants to do this.
            string message = "This will delete this course entry and all assessment sessions associated with it and will remove any files deployed to exam accounts. Are you sure you wish to do this? This cannot be undone.";
            if (MessageBox.Show(message, "Confirm delete course", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                //First delete the course contained in the node. The method returns DialogResult.No if user cancels it
                if (CourseManager.DeleteCourse(node.Course) == DialogResult.Yes)
                {
                    //Remove the node
                    tvCourses.Nodes.Remove(node);
                    //Remove any sessions attached to this course.
                    if (node.Nodes.Count > 0)
                    {
                        foreach (AssessmentSessionNode asn in node.Nodes.Cast<AssessmentSessionNode>())
                        {
                            try
                            {
                                DeleteSessionNode(asn, true);
                            }
                            catch { }
                        }
                    }
                    //Select the first node in the tree if there is one
                    if (tvCourses.Nodes.Count > 0)
                    {
                        tvCourses.SelectedNode = tvCourses.Nodes[0];
                    }
                    else
                    {
                        //There are no nodes left in the tree, so disable both panels
                        pnlCourseView.Visible = false;
                        pnlCourseView.Enabled = false;
                        pnlAssessmentView.Visible = false;
                        pnlAssessmentView.Enabled = false;
                    }
                }
            }
        }

        public void DeleteSessionNode(AssessmentSessionNode node, bool parentBeingRemoved)
        {
            //Ask if the user really wants to do this
            string message = "This will delete all records of this assessment session, including any files deployed to the assessment accounts. Are you sure you wish to do this? This cannot be undone.";
            if (parentBeingRemoved || MessageBox.Show(message, "Delete assessment session", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                CourseManager.DeleteSession(node.Session);
                TreeNode parent = node.Parent;
                node.Remove();
                UpdateLastDeploymentTime();
                if (!parentBeingRemoved)
                {
                    if (parent != null)
                        tvCourses.SelectedNode = node.Parent;
                    else
                    {
                        pnlCourseView.Visible = false;
                        pnlCourseView.Enabled = false;
                        pnlAssessmentView.Visible = false;
                        pnlAssessmentView.Enabled = false;
                    }
                }
            }
        }

        private void SetCoursesContextMenu(CourseContextMenuMode mode)
        {
            bool course, session;
            if (mode == CourseContextMenuMode.Course)
            {
                course = true;
                session = false;
            }
            else
            {
                course = false;
                session = true;
            }
            //Course related things
            tsmiDuplicateCourse.Enabled = course;
            tsmiDuplicateCourse.Visible = course;
            toolStripSeparatorCourses.Visible = course;
            tsmiDeleteCourse.Visible = course;
            tsmiDeleteCourse.Enabled = course;

            //Session related things
            tsmiDeleteAssessmentSession.Visible = session;
            tsmiDeleteAssessmentSession.Enabled = session;
        }

        private void GenerateHandout(AssessmentSession session)
        {
            //TODO:: THIS, get info from snjezana
        }

        #endregion

        #region Events

        private void btnNewCourse_Click(object sender, EventArgs e)
        {
            NewCourseForm ncf = new NewCourseForm();
            ncf.StartPosition = FormStartPosition.CenterParent;
            if (ncf.ShowDialog() == DialogResult.OK)
            {
                CourseManager.RegisterNewCourse(ncf.Course);
                reloadCourses = true;
            }
        }

        private void tbCourseSearch_TextChanged(object sender, EventArgs e)
        {
            if (!tbCourseSearch.Text.NullOrEmpty())
            {
                CourseManager.RebuildTreeView(tbCourseSearch.Text);
            }
            else
                CourseManager.RebuildTreeView();
        }

        private void tvCourses_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is CourseNode)
            {
                CourseNode node = e.Node as CourseNode;
                Course course = node.Course;
                CourseInformation info = course.CourseInfo;

                //Store the revert point
                courseRevertPoint = course.Clone();
                //Store the node for use later
                prevNode = node;

                //Show the course panel and hide the session panel
                pnlCourseView.Visible = true;
                pnlCourseView.Enabled = true;
                pnlAssessmentView.Visible = false;
                pnlAssessmentView.Enabled = false;

                //Show the course information
                tbCourseName.Text = info.CourseName;
                tbCourseCode1.Text = info.CourseCode1;
                tbCourseCode2.Text = info.CourseCode2;
                nudCourseYear.Value = int.Parse(info.Year);
                cbCourseSemester.SelectedItem = info.Semester;
                tbCourseID.Text = course.ID;

                //Show all the students
                dgvCourseStudents.Rows.Clear();
                foreach (Student s in course.Students)
                {
                    //DGVEDIT::
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvCourseStudents);
                    row.Cells[0].Value = s.UserName;
                    row.Cells[1].Value = s.LastName;
                    row.Cells[2].Value = s.FirstName;
                    row.Cells[3].Value = s.StudentID;
                    dgvCourseStudents.Rows.Add(row);
                }
            }
            else if (e.Node is AssessmentSessionNode)
            {
                AssessmentSessionNode node = e.Node as AssessmentSessionNode;
                AssessmentSession s = node.Session;
                //Show the session panel and hide course panel
                pnlCourseView.Visible = false;
                pnlCourseView.Enabled = false;
                pnlAssessmentView.Visible = true;
                pnlAssessmentView.Enabled = true;

                //Show the assessment details
                tbSessionName.Text = s.AssessmentInfo.AssessmentName;
                tbSessionFileName.Text = s.AssessmentFileName;
                tbSessionTarget.Text = s.DeploymentTarget;

                //Show the timing details
                tbSessionDate.Text = s.StartTime.Date.ToString("dd/MM/yyyy");
                tbSessionStartTime.Text = s.StartTime.ToString("hh:mm:ss tt");
                tbSessionFinishTime.Text = s.StartTime.AddMinutes(s.AssessmentLength + s.ReadingTime).ToString("hh:mm:ss tt");
                tbSessionLength.Text = s.AssessmentLength.ToString();
                tbSessionReadingTime.Text = s.ReadingTime.ToString();

                //Show course id and password
                tbSessionCourseID.Text = s.CourseID;
                tbSessionRestartPassword.Text = s.RestartPassword;

                //Show additional files
                lbSessionAdditionalFiles.Items.Clear();
                if (s.AdditionalFiles.Count > 0)
                {
                    foreach (var f in s.AdditionalFiles)
                    {
                        lbSessionAdditionalFiles.Items.Add(f);
                    }
                }

                //Show the students
                dgvPublishedAssessmentStudents.Rows.Clear();
                foreach (var sd in s.StudentData)
                {
                    //DGVEDIT:: 
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvPublishedAssessmentStudents);

                    row.Cells[0].Value = sd.UserName;
                    row.Cells[1].Value = sd.LastName;
                    row.Cells[2].Value = sd.FirstName;
                    row.Cells[3].Value = sd.StudentID;
                    row.Cells[4].Value = sd.StartTime;
                    row.Cells[5].Value = sd.AssessmentLength;
                    row.Cells[6].Value = sd.ReadingTime;
                    row.Cells[7].Value = sd.AccountName;
                    row.Cells[8].Value = sd.AccountPassword;

                    dgvPublishedAssessmentStudents.Rows.Add(row);
                }
            }
            else
            {
                pnlCourseView.Visible = false;
                pnlCourseView.Enabled = false;
                pnlAssessmentView.Visible = false;
                pnlAssessmentView.Enabled = false;
            }
            CourseEdited = false;
        }

        private void tvCourses_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            //Check for changes made to the current selected course. Propmpt the user to apply or discard changes before changing to new course
            if (CourseEdited && prevNode != null)
            {
                string message = "There have been changes made to this course. Changing to a different one now will cause those changes to be discarded. Do you wish to commit your changes before moving to a different course?";
                DialogResult result = MessageBox.Show(message, "Unsaved changes to course", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    ApplyCourseChanges();
                }
                else if (result == DialogResult.No)
                {
                    //Revert the changes
                    RevertCourseChanges();
                }
                else
                {
                    //Cancel the change
                    e.Cancel = true;
                }
            }
        }

        private void tbCourseName_TextChanged(object sender, EventArgs e)
        {
            if (tvCourses.SelectedNode is CourseNode)
            {
                //As this text box is only visible and editable when a coursenode is selected, this should never cause an exception. (hopefully)!!
                CourseNode node = tvCourses.SelectedNode as CourseNode;
                CourseEdited = true;
                node.Course.CourseInfo.CourseName = tbCourseName.Text;
                node.Text = node.Course.CourseTitle;
                node.Name = node.Text;
            }
        }

        #region CourseCode

        private void tbCourseCode1_TextChanged(object sender, EventArgs e)
        {
            if (tvCourses.SelectedNode is CourseNode)
            {
                CourseNode node = tvCourses.SelectedNode as CourseNode;
                CourseEdited = true;
                node.Course.CourseInfo.CourseCode1 = tbCourseCode1.Text;
                node.Text = node.Course.CourseTitle;
                node.Name = node.Text;
            }
        }

        private void tbCourseCode2_TextChanged(object sender, EventArgs e)
        {
            if (tvCourses.SelectedNode is CourseNode)
            {
                CourseNode node = tvCourses.SelectedNode as CourseNode;
                CourseEdited = true;
                node.Course.CourseInfo.CourseCode2 = tbCourseCode2.Text;
                node.Text = node.Course.CourseTitle;
                node.Name = node.Text;
            }
        }

        private void tbCourseCode1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (tbCourseCode1.Text.Length >= 3 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                tbCourseCode2.Focus();
            }
            else if (tbCourseCode1.Text.Length >= 2 && !char.IsControl(e.KeyChar))
            {
                tbCourseCode2.Focus();
            }
        }

        private void tbCourseCode2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (tbCourseCode2.Text.Length >= 3 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        private void dgvCourseStudents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CourseEdited = true;
        }

        private void dgvCourseStudents_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CourseEdited = true;
        }

        private void btnImportStudents_Click(object sender, EventArgs e)
        {
            //Import student data from another course
            ImportStudentsForm ipf = new ImportStudentsForm();
            ipf.StartPosition = FormStartPosition.CenterParent;

            if (ipf.ShowDialog() == DialogResult.OK)
            {
                //Show a confirmation message box, warning that previous students list will be removed
                if (MessageBox.Show("Importing this student list will overrite the current student list. Are you sure you wish to continue?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dgvCourseStudents.Rows.Clear();
                    foreach (Student s in ipf.Students)
                    {
                        //DGVEDIT::
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dgvCourseStudents);
                        row.Cells[0].Value = s.UserName;
                        row.Cells[1].Value = s.LastName;
                        row.Cells[2].Value = s.FirstName;
                        row.Cells[3].Value = s.StudentID;
                        dgvCourseStudents.Rows.Add(row);
                    }
                    CourseEdited = true;
                }
            }
        }

        private void nudCourseYear_ValueChanged(object sender, EventArgs e)
        {
            if (tvCourses.SelectedNode is CourseNode)
            {
                CourseNode node = tvCourses.SelectedNode as CourseNode;
                node.Course.CourseInfo.Year = nudCourseYear.Value.ToString();
                CourseEdited = true;
            }
        }

        private void cbCourseSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tvCourses.SelectedNode is CourseNode)
            {
                CourseNode node = tvCourses.SelectedNode as CourseNode;
                node.Course.CourseInfo.Semester = cbCourseSemester.SelectedItem.ToString();
                CourseEdited = true;
            }
        }

        private void btnApplyCourseChanges_Click(object sender, EventArgs e)
        {
            ApplyCourseChanges();
        }

        private void btnDiscardCourseChanges_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to discard these changes? This cannot be undone.", "Discard confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RevertCourseChanges();
                tvCourses_AfterSelect(sender, new TreeViewEventArgs(tvCourses.SelectedNode));
            }
        }

        private void tvCourses_KeyDown(object sender, KeyEventArgs e)
        {
            if (tvCourses.ContainsFocus && e.KeyCode == Keys.Delete)
            {
                if (tvCourses.SelectedNode is CourseNode)
                {
                    CourseNode node = tvCourses.SelectedNode as CourseNode;
                    DeleteCourseNode(node);
                    e.Handled = true;
                }
                else if (tvCourses.SelectedNode is AssessmentSessionNode)
                {
                    AssessmentSessionNode node = tvCourses.SelectedNode as AssessmentSessionNode;
                    DeleteSessionNode(node, false);
                    e.Handled = true;
                }
            }
        }

        private void tvCourses_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                TreeNode node = tvCourses.GetNodeAt(p);
                if (node != null)
                {
                    tvCourses.SelectedNode = node;
                    if (node is CourseNode)
                    {
                        CourseNode courseNode = node as CourseNode;
                        SetCoursesContextMenu(CourseContextMenuMode.Course);

                        cmsCoursesTree.Show(tvCourses, p);
                    }
                    else if (node is AssessmentSessionNode)
                    {
                        AssessmentSessionNode sessionNode = node as AssessmentSessionNode;
                        SetCoursesContextMenu(CourseContextMenuMode.AssessmentSession);

                        cmsCoursesTree.Show(tvCourses, p);
                    }
                }
            }
        }

        private void tsmiDeleteCourse_Click(object sender, EventArgs e)
        {
            TreeNode node = tvCourses.SelectedNode;
            if (node != null && node is CourseNode)
            {
                CourseNode courseNode = node as CourseNode;
                DeleteCourseNode(courseNode);
            }
        }

        private void tsmiDeleteAssessmentSession_Click(object sender, EventArgs e)
        {
            AssessmentSessionNode node = tvCourses.SelectedNode as AssessmentSessionNode;
            DeleteSessionNode(node, false);
        }

        private void tsmiDuplicateCourse_Click(object sender, EventArgs e)
        {
            TreeNode node = tvCourses.SelectedNode;
            if (node != null && node is CourseNode)
            {
                //Don't copy assessment sessions here
                CourseNode cNode = node as CourseNode;
                Course newCourse = cNode.Course.Clone(false);
                CourseManager.RegisterNewCourse(newCourse);
            }
        }

        private void btnSessionOpenLocation_Click(object sender, EventArgs e)
        {
            if (tvCourses.SelectedNode != null && tvCourses.SelectedNode is AssessmentSessionNode)
            {
                AssessmentSessionNode node = tvCourses.SelectedNode as AssessmentSessionNode;
                if (Directory.Exists(node.Session.FolderPath))
                {
                    Process.Start("explorer.exe", node.Session.FolderPath);
                }
            }
        }

        private void btnCourseOpenFolder_Click(object sender, EventArgs e)
        {
            if (tvCourses.SelectedNode != null && tvCourses.SelectedNode is CourseNode)
            {
                CourseNode node = tvCourses.SelectedNode as CourseNode;
                if (Directory.Exists(node.Course.GetCoursePath()))
                {
                    Process.Start("explorer.exe", node.Course.GetCoursePath());
                }
            }
        }

        private void btnCourseExpand_Click(object sender, EventArgs e)
        {
            tvCourses.ExpandAll();
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            tvCourses.CollapseAll();
        }

        private void btnSessionGenHandout_Click(object sender, EventArgs e)
        {
            AssessmentSessionNode node = tvCourses.SelectedNode as AssessmentSessionNode;
            if (node != null)
                GenerateHandout(node.Session);
        }

        private void btnCourseClearStudents_Click(object sender, EventArgs e)
        {
            string m = "Are you sure you wish to clear all students?";
            if (MessageBox.Show(m, "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dgvCourseStudents.Rows.Clear();
                CourseEdited = true;
            }
        }

        private void btnAssessmentMark_Click(object sender, EventArgs e)
        {
            //Load the assessment for marking
            AssessmentSessionNode node = null;
            if (tvCourses.SelectedNode is AssessmentSessionNode)
                node = tvCourses.SelectedNode as AssessmentSessionNode;
            if (node != null)
            {
                AssessmentSession session = node.Session;
                if (session == null)
                {
                    MessageBox.Show("Unable to load session", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                MarkSession = null;
                MarkSession = session;
            }
            //Select the marking tab
            tabControlMain.SelectedTab = tabPageMark;
        }

        #endregion

        #endregion

        #region Publisher Tab

        #region Properties

        private bool HasCourseSelected
        {
            get
            {
                return cbPublishCourseSelector.SelectedItem != null;
            }
        }

        private bool PublishPrepared
        {
            get
            {
                return publishPrepared;
            }
            set
            {
                publishPrepared = value;
            }
        }

        private Course SelectedCourse
        {
            get
            {
                return cbPublishCourseSelector.SelectedItem as Course;
            }
        }

        #endregion

        #region Methods

        private void InitialisePublishTab()
        {
            //Set the date time picker to current date
            dtpPublishDate.Value = DateTime.Now;

            //Populate the course selector.
            PopulateCoursePicker();

            //Set up the add files dialog
            addFilesDialog.InitialDirectory = DESKTOP_PATH;
            addFilesDialog.Multiselect = true;

            //Set up folder browser dialog
            folderBrowser.ShowNewFolderButton = false;
            folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void ResetPublishTab()
        {
            //Method called when an assessment is closed

            //Reset the values in the publish tab
            DateTime d = DateTime.Now;
            dtpPublishDate.Value = d;
            dtpPublishTime.Value = new DateTime(2016, 1, 1, 12, 0, 0, 0);
            lbPublishAdditionalFiles.Items.Clear();
            nudPublishAssessmentLength.Value = 60;
            nudPublishReadingTime.Value = 0;
            lblPublishFileName.Text = "";
            lblPublishLastDeployed.Text = "";
            tbPublishResetPassword.Text = "";
            btnPublishDeploy.Enabled = false;
            cbPublishCourseSelector.SelectedItem = null;
            PublishPrepared = false;
            btnPublishDeploy.Enabled = false;
            lblDeploymentTarget.Text = "";

            //Disable the publish screen
            tlpPublishContainer.Enabled = false;
            //Disable student editor
            dgvPublishStudents.Enabled = false;
            //Clear the students
            dgvCourseStudents.Rows.Clear();

        }

        private void SetPublishTab()
        {
            //Method called when an assessment is opened.

            //Set any values relevant to the assessment, ie file name, last time deployed
            lblPublishFileName.Text = assessmentFile.FullName;

            //Generate new password
            tbPublishResetPassword.Text = Util.RandomString(6);

            //Enable the publish screen
            tlpPublishContainer.Enabled = true;

            //Disable student editor
            dgvPublishStudents.Enabled = false;

            //Show when assessment was last published.
            UpdateLastDeploymentTime();
        }

        private void PopulateCoursePicker()
        {
            //Record the chosen item
            Course chosenCourse = null;
            if (cbPublishCourseSelector.SelectedItem != null)
                chosenCourse = (Course)cbPublishCourseSelector.SelectedItem;

            cbPublishCourseSelector.Items.Clear();
            cbPublishCourseSelector.Items.AddRange(CourseManager.Courses.ToArray());

            if (chosenCourse != null && cbPublishCourseSelector.Items.Contains(chosenCourse))
                cbPublishCourseSelector.SelectedItem = chosenCourse;
        }

        private void UpdateLastDeploymentTime()
        {
            if (HasAssessmentOpen)
            {
                DateTime date = INVALID_DATE;
                foreach (var course in CourseManager.Courses)
                {
                    if (course.Assessments.Count > 0)
                    {
                        foreach (var session in course.Assessments)
                        {
                            if (Path.GetFileName(session.AssessmentFileName) == assessmentFile.Name)
                            {
                                if (date == INVALID_DATE || session.DeploymentTime > date)
                                    date = session.DeploymentTime;
                            }
                        }
                    }
                }
                if (date != INVALID_DATE)
                    lblPublishLastDeployed.Text = date.ToShortDateString() + " " + date.ToShortTimeString();
                else
                    lblPublishLastDeployed.Text = "Never";
            }
        }

        private bool TryDeployAssessment(out AssessmentSession assessmentSession)
        {
            assessmentSession = null;
            const string title = "Unable to deploy - ";
            List<StudentData> students = new List<StudentData>();
            //Check deployment target exists
            if (lblDeploymentTarget.Text.NullOrEmpty())
            {
                MessageBox.Show("Please select a deployment target", title + "No deployment target specified");
                return false;
            }
            else if (!Directory.Exists(@lblDeploymentTarget.Text))
            {
                MessageBox.Show("The specified deployment path is unreachable or does not exist", title + "Cannot reach deployment target");
                return false;
            }
            //Check that a name has been entered
            if (tbPublishAssessmentName.Text.NullOrEmpty())
            {
                MessageBox.Show("Please enter a valid assessment name", title + "Invalid assessment name");
                return false;
            }
            AssessmentInformation info = new AssessmentInformation()
            {
                AssessmentName = tbPublishAssessmentName.Text,
                Author = tbPublishAuthor.Text,
                AssessmentWeighting = (int)nudPublishWeigthing.Value
            };

            #region Student Check
            //Check the data for each student is good:
            if (!(dgvCourseStudents.Rows.Count > 0))
            {
                MessageBox.Show("There are no students for the assessment!", title + "No students");
                return false;
            }
            bool flag = false;
            Dictionary<string, List<Student.ErrorType>> errors = new Dictionary<string, List<Student.ErrorType>>();
            try
            {
                for (int i = 0; i < dgvPublishStudents.Rows.Count; i++)
                {
                    DataGridViewRow row = dgvPublishStudents.Rows[i];
                    //DGVEDIT::
                    string userName = row.Cells[0].Value == null ? "" : row.Cells[0].Value.ToString();
                    string lastName = row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString();
                    string firstName = row.Cells[2].Value == null ? "" : row.Cells[2].Value.ToString();
                    string studentID = row.Cells[3].Value == null ? "" : row.Cells[3].Value.ToString();
                    DateTime startTime;
                    if (row.Cells[4].Value == null)
                    {
                        startTime = INVALID_DATE;
                    }
                    else
                    {
                        startTime = (DateTime)row.Cells[4].Value;
                    }

                    int assessmentLength;
                    if (row.Cells[5].Value == null)
                    {
                        assessmentLength = -1;
                    }
                    else
                    {
                        decimal al = (decimal)row.Cells[5].Value;
                        assessmentLength = (int)al;
                    }

                    int readingTime;
                    if (row.Cells[6].Value == null)
                    {
                        readingTime = -1;
                    }
                    else
                    {
                        decimal rt = (decimal)row.Cells[6].Value;
                        readingTime = (int)rt;
                    }
                    string accountName = row.Cells[7].Value == null ? "" : row.Cells[7].Value.ToString();
                    string accountPassword = row.Cells[8].Value == null ? "" : row.Cells[8].Value.ToString();

                    StudentData sd = new StudentData(userName, lastName, firstName, studentID, startTime, assessmentLength, readingTime, accountName, accountPassword, tbPublishResetPassword.Text);
                    if (!sd.ResolveErrors())
                    {
                        flag = true;
                        string id = sd.AnyIdentifiableTag();
                        if (errors.Keys.Contains(id))
                        {
                            int num = 1;
                            do
                            {
                                id = id + " " + num.ToString();
                            } while (errors.Keys.Contains(id));
                        }
                        errors.Add(id, sd.GetErrors());
                    }
                    else
                        students.Add(sd);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            if (flag)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("The following student(s) have errors:");
                foreach (var kvp in errors)
                {
                    sb.AppendLine();
                    sb.AppendLine("Student " + kvp.Key + ":");
                    foreach (var e in kvp.Value)
                    {
                        sb.AppendLine("     " + e.ToString());
                    }
                }
                MessageBox.Show(sb.ToString(), title + "Students errored");
                return false;
            }
            #endregion

            #region Additional Files
            List<string> missingFiles = new List<string>();
            List<string> additionalFiles = new List<string>();
            List<string> additionalFilesNames = new List<string>();
            if (lbPublishAdditionalFiles.Items.Count > 0)
            {
                foreach (var o in lbPublishAdditionalFiles.Items)
                {
                    FileListItem fi = (FileListItem)o;
                    if (!File.Exists(@fi.Path))
                        missingFiles.Add(@fi.Path);
                    additionalFiles.Add(@fi.Path);
                    additionalFilesNames.Add(fi.ToString());
                }
            }
            if (missingFiles.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("The following file(s) are missing: ");
                foreach (var p in missingFiles)
                {
                    sb.AppendLine("     " + p);
                    sb.AppendLine();
                }
                MessageBox.Show(sb.ToString(), title + "Missing files");
                return false;
            }
            #endregion

            #region Build AssessmentSession

            DateTime date = dtpPublishDate.Value;
            DateTime time = dtpPublishTime.Value;
            DateTime startTime2 = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            AssessmentSession session = new AssessmentSession(SelectedCourse.ID, lblDeploymentTarget.Text, info, assessmentFile.Name, startTime2, (int)nudPublishAssessmentLength.Value, (int)nudPublishReadingTime.Value, tbPublishResetPassword.Text, students, additionalFilesNames, DateTime.Now, Assessment);
            assessmentSession = session;
            #endregion

            #region Deploy all files

            string coursePath;
            try
            {
                coursePath = CourseManager.PathForCourse(session.CourseID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            //Backup the files
            string sessionPath = CourseManager.CreateAssessmentDir(session, coursePath);
            string sessionFilePath = Path.Combine(@sessionPath, session.AssessmentInfo.AssessmentName + ASSESSMENT_SESSION_EXT);
            session.FolderPath = sessionPath;
            CourseManager.SerialiseSession(session, @sessionFilePath);
            string assessmentPath = Path.Combine(@sessionPath, assessmentFile.Name);
            SaveToFile(@assessmentPath);
            if (additionalFiles.Count > 0)
            {
                try
                {
                    foreach (var p in additionalFiles)
                    {
                        string dest = Path.Combine(@sessionPath, Path.GetFileName(@p));
                        File.Copy(@p, @dest, true);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error copying files: \n\n" + e.Message);
                    return false;
                }
            }

            //Deploy to the student accounts
            foreach (var sd in session.StudentData)
            {
                //TODO:: Make sure that the target exists (When loading account names from spreadsheet)
                string destPath = Path.Combine(@lblDeploymentTarget.Text, sd.AccountName);
                try
                {
                    AssessmentScript script = AssessmentScript.BuildForPublishing(Assessment, sd, info);
                    script.CourseInformation = CourseManager.FindCourseByID(session.CourseID)?.CourseInfo.Clone();
                    string scriptPath = Path.Combine(@destPath, session.AssessmentInfo.AssessmentName + ASSESSMENT_SCRIPT_EXT);
                    using (FileStream s = File.Open(@scriptPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(s, script);
                    }
                    if (additionalFiles.Count > 0)
                    {
                        foreach (var p in additionalFiles)
                        {
                            string d = Path.Combine(@destPath, Path.GetFileName(p));
                            File.Copy(@p, @d, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to publish files for student: {sd.AnyIdentifiableTag()} \n\n" + ex.Message);
                    return false;
                }
            }

            #endregion

            //Rebuild course manager tree
            CourseManager.AddAssessmentSession(session);
            CourseManager.RebuildTreeView(tbCourseSearch.Text);
            //Set the last deployed time
            DateTime lastDeployed = DateTime.Now;
            lblPublishLastDeployed.Text = lastDeployed.ToShortDateString() + " " + lastDeployed.ToShortTimeString();
            //Success!
            return true;
        }

        private void AbortDeployment(AssessmentSession session)
        {
            if (session != null)
            {
                //Delete the backup folder
                if (!session.FolderPath.NullOrEmpty() && Directory.Exists(session.FolderPath))
                {
                    try
                    {
                        Util.DeleteDirectory(session.FolderPath);
                    }
                    catch { }
                }
                //Try delete any files that were deployed
                if (!session.DeploymentTarget.NullOrEmpty() && Directory.Exists(session.DeploymentTarget))
                {
                    string[] accountDirs = Directory.GetDirectories(session.DeploymentTarget);
                    if (accountDirs.Count() > 0)
                    {
                        foreach (var account in accountDirs)
                        {
                            string[] files = Directory.GetFiles(account);
                            if (files.Count() > 0)
                            {
                                foreach (var filePath in files)
                                {
                                    try
                                    {
                                        //Delete the script file
                                        string fileName = Path.GetFileName(filePath);
                                        string scriptName = session.AssessmentInfo.AssessmentName + ASSESSMENT_SCRIPT_EXT;
                                        if (fileName == scriptName)
                                        {
                                            Util.DeleteFile(filePath);
                                            continue;
                                        }

                                        //Delete the additional files
                                        if (session.AdditionalFiles.Count > 0)
                                        {
                                            if (session.AdditionalFiles.Contains(fileName))
                                                Util.DeleteFile(filePath);
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Events

        private void btnPublishDeploy_Click(object sender, EventArgs e)
        {
            //Tell user that this is final, cannot be changed and ask for them to check that everything is correct
            string m = "Are you sure that all information is correct? Once the assessment is deployed, it cannot be changed. Clicking 'Yes' will commence the deployment process.";
            AssessmentSession session;
            if (MessageBox.Show(m, "Deploy assessment?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (TryDeployAssessment(out session))
                {
                    //If publish is successful, offer to create a pdf containing all the information handout forms for the students. Let user know that this can be done by selecting assessment
                    // in course manager tab.
                    string message = "Assessment successfully published! Would you like to generate handout forms for each student in this assessment? This can be done later in the CourseManager tab by selecting the assessment.";
                    if (MessageBox.Show(message, "Create handout pdf?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        GenerateHandout(session);
                    }
                }
                else
                {
                    AbortDeployment(session);
                }
            }
        }

        private void btnPublishPrepare_Click(object sender, EventArgs e)
        {
            //Prepare the students. If has already been pressed, confirm to make changes.
            if (HasCourseSelected)
            {
                if (PublishPrepared)
                {
                    string message = "This action will overrite the current list of students. Are you sure you wish to continue?";
                    if (MessageBox.Show(message, "Confirm changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        dgvPublishStudents.Rows.Clear();
                    }
                    else
                    {
                        return;
                    }
                }
                if (lblDeploymentTarget.Text.NullOrEmpty() || !Directory.Exists(@lblDeploymentTarget.Text))
                {
                    string message = "Please select a valid deployment target.";
                    MessageBox.Show(message, "Invalid deployment target");
                    return;
                }

                List<string> paths = Directory.GetDirectories(@lblDeploymentTarget.Text).ToList();
                if (paths.Count < SelectedCourse.Students.Count)
                {
                    string m = "There are too many students in this course and not enough exam account folders. Please make sure that there are enough for the number of students.";
                    MessageBox.Show(m, "Too many students");
                    return;
                }

                PublishPrepared = true;
                dgvPublishStudents.Enabled = true;
                btnPublishDeploy.Enabled = true;

                //DGVEDIT:: Fill out the students grid.
                dgvPublishStudents.Rows.Clear();
                DateTime date = dtpPublishDate.Value;
                DateTime time = dtpPublishTime.Value;
                DateTime d = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
                int pathNum = 0;
                foreach (var student in SelectedCourse.Students)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvPublishStudents);

                    row.Cells[0].Value = student.UserName;
                    row.Cells[1].Value = student.LastName;
                    row.Cells[2].Value = student.FirstName;
                    row.Cells[3].Value = student.StudentID;
                    row.Cells[4].Value = d;
                    row.Cells[5].Value = nudPublishAssessmentLength.Value;
                    row.Cells[6].Value = nudPublishReadingTime.Value;
                    //TODO:: Assign account username and password to each student properly
                    //TODO:: Will read values from a given spreadsheet. Must check to make sure that each directory exists
                    try
                    {
                        row.Cells[7].Value = new DirectoryInfo(paths[pathNum]).Name;
                        row.Cells[8].Value = "password";
                    }
                    catch { }
                    pathNum++;
                    dgvPublishStudents.Rows.Add(row);
                }
            }
            else
            {
                //If no course selected, tell must select one.
                MessageBox.Show("Please select a course!");
            }
        }

        private void tabControlMain_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == "tabPagePublish" && reloadCourses)
            {
                reloadCourses = false;
                PopulateCoursePicker();
            }
        }

        private void dgvPublishStudents_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                //DateTime
                if (dgvPublishStudents.Focused && dgvPublishStudents.CurrentCell.ColumnIndex == 4)
                {
                    dtpPublishTimeStudent.Location = dgvPublishStudents.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                    dtpPublishTimeStudent.Visible = true;
                    dtpPublishTimeStudent.Width = dgvPublishStudents.CurrentCell.OwningColumn.Width;
                    dtpPublishTimeStudent.Height = dgvPublishStudents.CurrentCell.OwningRow.Height;
                    if (dgvPublishStudents.CurrentCell.Value != null)
                    {
                        dtpPublishTimeStudent.Value = (DateTime)dgvPublishStudents.CurrentCell.Value;
                    }
                    else
                    {
                        DateTime date = dtpPublishDate.Value;
                        DateTime time = dtpPublishTime.Value;
                        dtpPublishTimeStudent.Value = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                    }
                }
                else
                    dtpPublishTimeStudent.Visible = false;

                //Column index 5 is assessment length, 6 is reading time
                if (dgvPublishStudents.Focused && (dgvPublishStudents.CurrentCell.ColumnIndex == 5 || dgvPublishStudents.CurrentCell.ColumnIndex == 6))
                {
                    nudAssessmentTimeStudent.Location = dgvPublishStudents.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                    nudAssessmentTimeStudent.Visible = true;
                    nudAssessmentTimeStudent.Width = dgvPublishStudents.CurrentCell.OwningColumn.Width;
                    nudAssessmentTimeStudent.Height = dgvPublishStudents.CurrentCell.OwningRow.Height;
                    if (dgvPublishStudents.CurrentCell.Value != null)
                    {
                        nudAssessmentTimeStudent.Value = (decimal)dgvPublishStudents.CurrentCell.Value;
                    }
                    else
                    {
                        if (dgvPublishStudents.CurrentCell.ColumnIndex == 5)
                            nudAssessmentTimeStudent.Value = nudPublishAssessmentLength.Value;
                        else
                            nudAssessmentTimeStudent.Value = nudPublishReadingTime.Value;
                    }
                }
                else
                    nudAssessmentTimeStudent.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvPublishStudents_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //DateTime
                if (dgvPublishStudents.Focused && dgvPublishStudents.CurrentCell.ColumnIndex == 4)
                {
                    dgvPublishStudents.CurrentCell.Value = dtpPublishTimeStudent.Value;
                }
                dtpPublishTimeStudent.Visible = false;

                //length and reading time
                if (dgvPublishStudents.Focused && dgvPublishStudents.CurrentCell.ColumnIndex == 5)
                {
                    dgvPublishStudents.CurrentCell.Value = nudAssessmentTimeStudent.Value;
                }
                else if (dgvPublishStudents.Focused && dgvPublishStudents.CurrentCell.ColumnIndex == 6)
                {
                    dgvPublishStudents.CurrentCell.Value = nudAssessmentTimeStudent.Value;
                }
                nudAssessmentTimeStudent.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dtpPublishTimeStudent_ValueChanged(object sender, EventArgs e)
        {
            dgvPublishStudents.CurrentCell.Value = dtpPublishTimeStudent.Value;
        }

        private void nudAssessmentTimeStudent_ValueChanged(object sender, EventArgs e)
        {
            dgvPublishStudents.CurrentCell.Value = nudAssessmentTimeStudent.Value;
        }

        private void btnPublishAdditonalFilesAdd_Click(object sender, EventArgs e)
        {
            if (addFilesDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fp in addFilesDialog.FileNames)
                {
                    lbPublishAdditionalFiles.Items.Add(new FileListItem(fp));
                }
            }
        }

        private void btnPublishAdditionalFilesDelSel_Click(object sender, EventArgs e)
        {
            if (lbPublishAdditionalFiles.SelectedItems.Count > 0)
            {
                string message = "Are you sure you wish to remove the selected item(s) from the list?";
                if (MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    List<object> objs = new List<object>();
                    foreach (var o in lbPublishAdditionalFiles.SelectedItems)
                    {
                        objs.Add(o);
                    }
                    foreach (var o in objs)
                    {
                        lbPublishAdditionalFiles.Items.Remove(o);
                    }
                }
            }
        }

        private void btnPublishAdditionalFilesDelAll_Click(object sender, EventArgs e)
        {
            string m = "Are you sure you wish to clear all items from the list?";
            if (MessageBox.Show(m, "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lbPublishAdditionalFiles.Items.Clear();
            }
        }

        private void btnDeploymentTarget_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                lblDeploymentTarget.Text = folderBrowser.SelectedPath;
            }
        }

        #endregion

        #endregion

        #region Marking Tab

        #region Properties

        private AssessmentSession MarkSession
        {
            get
            {
                return markSession;
            }
            set
            {
                markSession = value;
                InitialiseMarkTab(markSession);
            }
        }

        private AssessmentScriptListItem CurMarkScript
        {
            get
            {
                return cbMarkAssessmentVersion.SelectedItem as AssessmentScriptListItem;
            }
        }

        #endregion

        #region Methods

        private void InitialiseMarkTab(AssessmentSession session)
        {
            if (session == null)
            {
                //Disable all items in this tab
                lbMarkStudents.Enabled = false;
                btnMarkLoadSel.Enabled = false;
                btnMarkLoadAll.Enabled = false;

                EnableMarkEditorGUI(false);
            }
            else
            {
                //Load all information for the session
                lbMarkStudents.Enabled = true;
                btnMarkLoadSel.Enabled = true;
                btnMarkLoadAll.Enabled = true;

                if (!session.MarkingStarted)
                {
                    foreach (var s in session.StudentData)
                    {
                        StudentMarkingData smd = new StudentMarkingData(s, session.Assessment);
                        session.StudentMarkingData.Add(smd);
                    }
                    session.MarkingStarted = true;
                }
                lbMarkStudents.Items.Clear();
                lbMarkStudents.Items.AddRange(session.StudentMarkingData.ToArray());
            }
        }

        private void LoadStudent(StudentMarkingData smd)
        {
            if (smd != null && MarkSession != null)
            {
                smd.Loaded = true;
                smd.DateLastLoaded = DateTime.Now;
                string deployedPath = Path.Combine(MarkSession.DeploymentTarget, smd.StudentData.AccountName);
                string studentBackupPath = Path.Combine(MarkSession.FolderPath, smd.StudentData.UserName);
                if (!Directory.Exists(studentBackupPath))
                    Directory.CreateDirectory(studentBackupPath);
                //Load the main file
                smd.Scripts.Clear();
                string mainScriptPath = Path.Combine(deployedPath, MarkSession.AssessmentScriptFileName);
                try
                {
                    using (Stream s = File.OpenRead(mainScriptPath))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        AssessmentScript mainScript = (AssessmentScript)bf.Deserialize(s);
                        if (mainScript == null)
                            throw new Exception("Cannot read file at: " + mainScriptPath);
                        else
                        {
                            smd.Scripts.Add(new AssessmentScriptListItem(mainScript, "Main file"));
                        }
                    }

                    //Copy over backup file
                    string backupPath = Path.Combine(studentBackupPath, MarkSession.AssessmentScriptFileName);
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    File.Copy(mainScriptPath, backupPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error when loading assessment script: \n\n" + ex.Message);
                }

                //Load the autosaves
                string autosavesPath = Path.Combine(deployedPath, AUTOSAVE_FOLDER_NAME(MarkSession.AssessmentInfo.AssessmentName));
                if (Directory.Exists(autosavesPath))
                {
                    try
                    {
                        string[] files = Directory.GetFiles(autosavesPath);
                        foreach (var filePath in files)
                        {
                            using (FileStream s = File.OpenRead(filePath))
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                AssessmentScript autosave = (AssessmentScript)bf.Deserialize(s);
                                if (autosave != null)
                                {
                                    string str = Path.GetFileNameWithoutExtension(filePath);
                                    string number = str.Substring(str.Length - 3);
                                    smd.Scripts.Add(new AssessmentScriptListItem(autosave, "autosave" + number));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading autosave file: \n\n" + ex.Message);
                    }
                }

                int index = lbMarkStudents.Items.IndexOf(smd);
                lbMarkStudents.Items.Remove(smd);
                lbMarkStudents.Items.Insert(index, smd);
                lbMarkStudents.SelectedItem = smd;
            }
        }

        private void EnableMarkEditorGUI(bool enable)
        {
            if (enable)
            {
                //Enable all the things
                tvMarkQuestions.Enabled = true;
                tlpMarkContainer.Enabled = true;
                cbMarkAssessmentVersion.Enabled = true;
                btnMarkEmailStudent.Enabled = true;
                btnMarkStudentPDF.Enabled = true;
                btnMarkEmailAll.Enabled = true;
                btnMarkAllPDF.Enabled = true;
                btnMarkQuestionsCollapse.Enabled = true;
                btnMarkQuestionsExpand.Enabled = true;
                lblMarkLastLoadedStudentDate.Visible = true;
                lblMarkLastLoadedStudentDateText.Visible = true;
            }
            else
            {
                //Disable all the things
                tvMarkQuestions.Enabled = false;
                tlpMarkContainer.Enabled = false;
                cbMarkAssessmentVersion.Enabled = false;
                btnMarkEmailStudent.Enabled = false;
                btnMarkStudentPDF.Enabled = false;
                btnMarkEmailAll.Enabled = false;
                btnMarkAllPDF.Enabled = false;
                btnMarkQuestionsCollapse.Enabled = false;
                btnMarkQuestionsExpand.Enabled = false;
                lblMarkLastLoadedStudentDate.Visible = false;
                lblMarkLastLoadedStudentDateText.Visible = false;

                tvMarkQuestions.Nodes.Clear();
                cbMarkAssessmentVersion.Items.Clear();

                rtbMarkerResponse.Text = "";
                rtbMarkModelAnswer.Text = "";
                rtbMarkQuestionText.Text = "";
                rtbMarkStudentAnswer.Text = "";
            }
        }

        private void SaveMarkingSession()
        {
            if (suppressMarkingSave)
                return;
            try
            {
                string path = Path.Combine(MarkSession.FolderPath, MarkSession.AssessmentInfo.AssessmentName + ASSESSMENT_SESSION_EXT);
                if (File.Exists(path))
                    File.Delete(path);
                using (FileStream s = File.Open(path, FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(s, MarkSession);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving marking data: \n\n" + ex.Message);
            }
            finally
            {
                markingChangesMade = false;
            }
        }

        #endregion

        #region Events

        private void btnMarkLoadSel_Click(object sender, EventArgs e)
        {
            if (lbMarkStudents.SelectedItem is StudentMarkingData)
            {
                StudentMarkingData smd = lbMarkStudents.SelectedItem as StudentMarkingData;
                LoadStudent(smd);
            }
        }

        private void btnMarkLoadAll_Click(object sender, EventArgs e)
        {
            List<StudentMarkingData> list = lbMarkStudents.Items.Cast<StudentMarkingData>().ToList();
            suppressMarkingSave = true;
            foreach (var smd in list)
            {
                LoadStudent(smd);
            }
            suppressMarkingSave = false;
            SaveMarkingSession();
        }

        private void lbMarkStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbMarkStudents.SelectedItem != null && lbMarkStudents.SelectedItem is StudentMarkingData)
            {
                StudentMarkingData smd = lbMarkStudents.SelectedItem as StudentMarkingData;
                if (smd != null)
                {
                    if (smd.Loaded)
                    {
                        //Enable the editor gui
                        EnableMarkEditorGUI(true);

                        //Load the script versions into the combo box
                        cbMarkAssessmentVersion.Items.Clear();
                        cbMarkAssessmentVersion.Items.AddRange(smd.Scripts.ToArray());
                        suppressCBEvent = true;
                        cbMarkAssessmentVersion.SelectedIndex = 0;
                        suppressCBEvent = false;

                        //Load the questions into the tree view
                        smd.FillTreeView(tvMarkQuestions);
                        tvMarkQuestions.ExpandAll();

                        //Select the first question
                        if (tvMarkQuestions.Nodes.Count > 0)
                            tvMarkQuestions.SelectedNode = tvMarkQuestions.Nodes[0];

                        //Show the time the student was last loaded
                        string str = smd.DateLastLoaded == INVALID_DATE ? "Never" : smd.DateLastLoaded.ToShortDateString() + " " + smd.DateLastLoaded.ToShortTimeString();
                        lblMarkLastLoadedStudentDate.Text = str;
                    }
                    else
                    {
                        EnableMarkEditorGUI(false);
                    }
                }
                else
                    EnableMarkEditorGUI(false);
            }
        }

        private void btnMarkQuestionsCollapse_Click(object sender, EventArgs e)
        {
            tvMarkQuestions.CollapseAll();
        }

        private void btnMarkQuestionsExpand_Click(object sender, EventArgs e)
        {
            tvMarkQuestions.ExpandAll();
        }

        private void tvMarkQuestions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (CurMarkScript == null)
            {
                MessageBox.Show("Please select an assessment script from the combo box");
                return;
            }
            MarkingQuestionNode node = tvMarkQuestions.SelectedNode as MarkingQuestionNode;
            if (node != null)
            {
                AssessmentScript script = CurMarkScript.Script;
                //First, find the question that the node is related to, in the selected script
                Question q = script.FindQuestion(node.MarkingQuestion.QuestionName);
                if (q == null)
                {
                    MessageBox.Show("Error: Unable to find question", "Error");
                    return;
                }
                Answer a = null;
                if (q.AnswerType != AnswerType.None)
                {
                    //Find the student answer in the script
                    a = script.Answers[q.Name];
                }

                //Display the question stuff
                rtbMarkQuestionText.Rtf = q.QuestionText;

                switch (q.AnswerType)
                {
                    case AnswerType.None:
                        {
                            pnlMarkStudentAnswerContainer.Enabled = false;
                            pnlMarkStudentAnswerContainer.Visible = false;

                            pnlMarkerResponseContainer.Enabled = false;
                            pnlMarkerResponseContainer.Visible = false;

                            pnlMarkModelAnswer.Visible = false;
                            pnlMarkModelAnswer.Enabled = false;
                            break;
                        }
                    case AnswerType.Multi:
                        {
                            pnlMarkStudentAnswerContainer.Enabled = true;
                            pnlMarkStudentAnswerContainer.Visible = true;

                            pnlMarkerResponseContainer.Enabled = true;
                            pnlMarkerResponseContainer.Visible = true;

                            pnlMarkModelAnswer.Visible = true;
                            pnlMarkModelAnswer.Enabled = true;

                            //Build the student answer string
                            string str = "";
                            if (a.SelectedOption != MultiChoiceOption.None)
                                str = $"The student selected answer: ({a.SelectedOption.ToString()}) {q.GetOptionText(a.SelectedOption)}";
                            else
                                str = "The student did not select an answer";
                            rtbMarkStudentAnswer.Text = str;

                            //Build the model answer string
                            string mStr = string.Concat(new object[]
                            {
                                "The correct option was: \n",
                                $"({q.CorrectOption.ToString()}) {q.GetOptionText(q.CorrectOption)}\n\n",
                                "The options were: \n\n",
                                $"(A) {q.OptionA}\n",
                                $"(B) {q.OptionB}\n",
                                $"(C) {q.OptionC}\n",
                                $"(D) {q.OptionD}"
                            });
                            rtbMarkModelAnswer.Text = mStr;

                            //Show the marker response
                            rtbMarkerResponse.Text = node.MarkingQuestion.MarkerResponse;

                            //Show the marks
                            lblMarksMaximum.Text = q.Marks.ToString("00");
                            nudMarkAssign.Maximum = q.Marks;
                            nudMarkAssign.Value = node.MarkingQuestion.AssignedMarks;

                            break;
                        }
                    default:
                        {
                            pnlMarkStudentAnswerContainer.Enabled = true;
                            pnlMarkStudentAnswerContainer.Visible = true;

                            pnlMarkerResponseContainer.Enabled = true;
                            pnlMarkerResponseContainer.Visible = true;

                            pnlMarkModelAnswer.Visible = true;
                            pnlMarkModelAnswer.Enabled = true;

                            //Show the student answer
                            string str = q.AnswerType == AnswerType.Open ? a.LongAnswer : a.ShortAnswer;
                            rtbMarkStudentAnswer.Text = str;

                            //Show the model answer
                            rtbMarkModelAnswer.Text = q.ModelAnswer;

                            //Show the marker response
                            rtbMarkerResponse.Text = node.MarkingQuestion.MarkerResponse;

                            //Show the marks
                            lblMarksMaximum.Text = q.Marks.ToString("00");
                            nudMarkAssign.Maximum = q.Marks;
                            nudMarkAssign.Value = node.MarkingQuestion.AssignedMarks;

                            break;
                        }
                }
            }
            SaveMarkingSession();
        }

        private void nudMarkAssign_ValueChanged(object sender, EventArgs e)
        {
            MarkingQuestionNode node = tvMarkQuestions.SelectedNode as MarkingQuestionNode;
            if (node != null)
            {
                node.MarkingQuestion.AssignedMarks = (int)nudMarkAssign.Value;
            }
        }

        private void rtbMarkerResponse_TextChanged(object sender, EventArgs e)
        {
            MarkingQuestionNode node = tvMarkQuestions.SelectedNode as MarkingQuestionNode;
            if (node != null)
            {
                node.MarkingQuestion.MarkerResponse = rtbMarkerResponse.Text;
                markingChangesMade = true;
            }
        }

        #endregion

        #endregion

        private void cbMarkAssessmentVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressCBEvent)
                return;

            tvMarkQuestions_AfterSelect(sender, new TreeViewEventArgs(tvMarkQuestions.SelectedNode));
        }
    }
}
