using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace AssessmentManager
{
    public partial class ImportStudentsForm : Form
    {
        public ImportStudentsForm()
        {
            InitializeComponent();
            LoadCourses();
        }

        public List<Student> Students
        {
            get
            {
                List<Student> list = new List<Student>();

                if (dgv.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.Cells[0].Value == null && row.Cells[1].Value == null && row.Cells[2].Value == null && row.Cells[3].Value == null)
                            continue;
                        //DGVEDIT::
                        string studentID = row.Cells[0].Value?.ToString();
                        string lastName = row.Cells[1].Value?.ToString();
                        string firstName = row.Cells[2].Value?.ToString();
                        string userName = row.Cells[3].Value?.ToString();
                        Student s = new Student(userName, lastName, firstName, studentID);
                        list.Add(s);
                    }
                }

                return list;
            }
        }

        private void LoadCourses()
        {
            cbChooseCourse.Items.Clear();
            foreach (var c in CourseManager.Instance.Courses)
            {
                cbChooseCourse.Items.Add(c);
            }
        }

        private void PopulateGridView(Course course)
        {
            dgv.Rows.Clear();
            if (course.Students.Count > 0)
            {
                foreach (var s in course.Students)
                {
                    //DGVEDIT::
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgv);
                    row.Cells[0].Value = s.StudentID;
                    row.Cells[1].Value = s.LastName;
                    row.Cells[2].Value = s.FirstName;
                    row.Cells[3].Value = s.UserName;
                    dgv.Rows.Add(row);
                }
            }
        }

        private void cbChooseCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbChooseCourse.SelectedItem != null && cbChooseCourse.SelectedItem is Course)
            {
                Course c = cbChooseCourse.SelectedItem as Course;
                PopulateGridView(c);
            }
        }

        private void btnImportSpreadsheet_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = CONSTANTS.SPREADSHEET_FILTER;
            ofd.DefaultExt = CONSTANTS.SPREADSHEET_FILTER.Remove(0, 1);
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Excel.Application xl = null;
                Excel.Workbook workBook = null;
                Excel.Worksheet sheet = null;
                Excel.Range range = null;

                try
                {
                    xl = new Excel.Application();
                    workBook = xl.Workbooks.Open(ofd.FileName);
                    sheet = workBook.Worksheets[1];
                    range = sheet.UsedRange;

                    dgv.Rows.Clear();
                    cbChooseCourse.SelectedItem = null;

                    for (int i = 1; i <= range.Rows.Count; i++)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dgv);

                        if (i == 1)
                        {
                            if ((range.Cells[i, 1] != null && (range.Cells[i, 1].Value2 == null || range.Cells[i, 1].Value2.ToString().ToLower().Contains("id"))) ||
                                (range.Cells[i, 2] != null && (range.Cells[i, 2].Value2 == null || range.Cells[i, 2].Value2.ToString().ToLower().Contains("name"))) ||
                                (range.Cells[i, 3] != null && (range.Cells[i, 3].Value2 == null || range.Cells[i, 3].Value2.ToString().ToLower().Contains("name"))) ||
                                (range.Cells[i, 4] != null && (range.Cells[i, 4].Value2 == null || range.Cells[i, 4].Value2.ToString().ToLower().Contains("name"))))
                            {
                                continue;
                            }
                        }

                        for (int j = 1; j <= range.Columns.Count; j++)
                        {
                            if (range.Cells[i, j] != null && range.Cells[i, j].Value2 != null)
                            {
                                if (j > row.Cells.Count)
                                    break;
                                row.Cells[j - 1].Value = range.Cells[i, j].Value2;
                            }
                        }

                        dgv.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error importing from file: \n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    Marshal.ReleaseComObject(sheet);
                    workBook.Close();
                    Marshal.ReleaseComObject(workBook);
                    xl.Quit();
                    Marshal.ReleaseComObject(xl);
                }
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string msg = "This option lets you import a list of students from an Excel spreadsheet. The file format it uses is " + CONSTANTS.SPREADSHEET_EXT + "\n" + "The order of the columns expected is: \n\n"
            + "[Student ID] [Surname] [First Name] [MIT Username]";
            MessageBox.Show(msg, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
