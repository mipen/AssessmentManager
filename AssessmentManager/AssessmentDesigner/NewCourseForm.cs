using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AssessmentManager
{
    public partial class NewCourseForm : Form
    {
        public NewCourseForm()
        {
            InitializeComponent();

            cbSemester.SelectedIndex = 0;
            //Set the year to the current year.
            nudYear.Value = DateTime.Now.Year;
        }

        #region Properties

        public string CourseName
        {
            get
            {
                return tbCourseName.Text;
            }
        }

        public string Year
        {
            get
            {
                return nudYear.Value.ToString();
            }
        }

        public string Semester
        {
            get
            {
                return cbSemester.SelectedItem.ToString();
            }
        }

        public List<Student> Students
        {
            get
            {
                List<Student> list = new List<Student>();
                if (dgvStudents.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvStudents.Rows)
                    {
                        //DGVEDIT::
                        if (row.Cells[0].Value == null && row.Cells[1].Value == null && row.Cells[2].Value == null && row.Cells[3].Value == null)
                            continue;

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

        public Course Course
        {
            get
            {
                Course c = new Course();
                c.CourseInfo.CourseName = CourseName;
                c.CourseInfo.CourseCode1 = tbCode1.Text;
                c.CourseInfo.CourseCode2 = tbCode2.Text;
                c.CourseInfo.Year = Year;
                c.CourseInfo.Semester = Semester;
                c.Students = Students;
                return c;
            }
        }

        #endregion

        #region Events

        private void lblInfo_Click(object sender, EventArgs e)
        {
            //Write infomration about student details things
            const string message = "This is where you fill out the details for each student in the class. These details will be used when publishing and marking any assessments deployed for this course. This list can be edited later on if need be. \n Use the 'Import Students' button to import students from another course.";
            MessageBox.Show(message, "Students list", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tbCode1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (tbCode1.Text.Length >= 3 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                tbCode2.Focus();
            }
            else if (tbCode1.Text.Length >= 2 && !char.IsControl(e.KeyChar))
            {
                tbCode2.Focus();
            }
        }

        private void tbCode2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (tbCode2.Text.Length >= 3 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (tbCourseName.Text.NullOrEmpty())
            {
                MessageBox.Show("A course name is required", "Course name required");
                return;
            }
            else if (tbCode1.Text.NullOrEmpty() || tbCode2.Text.NullOrEmpty())
            {
                MessageBox.Show("Please enter a valid course code", "Valid course code required");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            ImportStudentsForm ipf = new ImportStudentsForm();
            ipf.StartPosition = FormStartPosition.CenterParent;
            if (ipf.ShowDialog() == DialogResult.OK)
            {
                dgvStudents.Rows.Clear();
                if (ipf.Students.Count > 0)
                {
                    foreach (var s in ipf.Students)
                    {
                        //DGVEDIT::
                        DataGridViewRow row = new DataGridViewRow();
                        row.CreateCells(dgvStudents);
                        row.Cells[0].Value = s.StudentID;
                        row.Cells[1].Value = s.LastName;
                        row.Cells[2].Value = s.FirstName;
                        row.Cells[3].Value = s.UserName;
                        dgvStudents.Rows.Add(row);
                    }
                }
            }
        }

        #endregion

    }
}
