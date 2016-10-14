using System;

namespace AssessmentManager
{
    [Serializable]
    public class CourseInformation
    {
        public string CourseName = "";
        public string CourseCode1 = "";
        public string CourseCode2 = "";
        public string Year = "2010";
        public string Semester = "1";

        public string CourseCodeFull
        {
            get
            {
                return CourseCode1 + "." + CourseCode2;
            }
        }

        public CourseInformation Clone()
        {
            CourseInformation c = new CourseInformation();
            c.CourseName = CourseName;
            c.CourseCode1 = CourseCode1;
            c.CourseCode2 = CourseCode2;
            c.Year = Year;
            c.Semester = Semester;
            return c;
        }
    }
}
