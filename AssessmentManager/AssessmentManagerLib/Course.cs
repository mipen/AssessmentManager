using System;
using System.Collections.Generic;

namespace AssessmentManager
{
    [Serializable]
    public class Course
    {
        private CourseInformation courseInfo = new CourseInformation();
        private string id = "";
        private List<Student> students = new List<Student>();
        [NonSerialized]
        private List<AssessmentSession> assessments = new List<AssessmentSession>();
        public string ID => id;
        public CourseInformation CourseInfo => courseInfo;
        public List<Student> Students
        {
            get
            {
                return students;
            }
            set
            {
                students = value;
            }
        }
        public List<AssessmentSession> Assessments
        {
            get
            {
                return assessments;
            }
        }
        public string CourseTitle
        {
            get
            {
                return CourseInfo.CourseCodeFull + " " + CourseInfo.CourseName;
            }
        }

        //Should only be used by the course manager when registering a new course!
        public void SetIdDirect(string newID)
        {
            id = newID;
        }

        public Course Clone(bool retainID = true)
        {
            Course c = new Course();
            if (retainID)
                c.id = id;
            else
            {
                c.id = CourseManager.Instance.RandomCourseID();
            }
            c.courseInfo = courseInfo.Clone();
            c.students = CloneStudentList();
            return c;
        }

        private List<Student> CloneStudentList()
        {
            List<Student> list = new List<Student>();
            if (students.Count > 0)
            {
                foreach (var s in students)
                {
                    list.Add(s.Clone());
                }
            }
            return list;
        }

        public void ResetAssessments()
        {
            assessments = new List<AssessmentSession>();
        }

        public override string ToString()
        {
            return CourseTitle;
        }
    }
}
