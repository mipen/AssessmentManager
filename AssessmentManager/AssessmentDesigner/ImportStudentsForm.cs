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
                        string userName = row.Cells[0].Value?.ToString();
                        string lastName = row.Cells[1].Value?.ToString();
                        string firstName = row.Cells[2].Value?.ToString();
                        string studentID = row.Cells[3].Value?.ToString();
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
                    row.Cells[0].Value = s.UserName;
                    row.Cells[1].Value = s.LastName;
                    row.Cells[2].Value = s.FirstName;
                    row.Cells[3].Value = s.StudentID;
                    dgv.Rows.Add(row);
                }
            }
        }

        private void cbChooseCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbChooseCourse.SelectedItem is Course)
            {
                Course c = cbChooseCourse.SelectedItem as Course;
                PopulateGridView(c);
            }
        }
    }
}
