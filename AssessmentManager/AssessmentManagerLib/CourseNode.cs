using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssessmentManager
{
    public class CourseNode : TreeNode
    {
        private Course course;

        public CourseNode(Course course)
        {
            this.course = course;
            Text = course.CourseTitle;
        }

        public Course Course
        {
            get
            {
                return course;
            }
            set
            {
                course = value;
            }
        }
    }
}
