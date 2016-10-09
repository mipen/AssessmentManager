using System.Windows.Forms;

namespace AssessmentManager
{
    public static class CoursesExt
    {
        public static string GetCoursePath(this Course course)
        {
            return Application.StartupPath + "\\"+ CONSTANTS.COURSES_FOLDER_NAME + "\\" + course.ID;
        }

        public static string GetFilePath(this Course course)
        {
            return course.GetCoursePath() + "\\" + course.ID + CONSTANTS.COURSE_EXT;
        }
    }
}
