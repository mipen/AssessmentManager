using System.IO;
using System.Windows.Forms;

namespace AssessmentManager
{
    public static class CoursesExt
    {
        public static string GetCoursePath(this Course course)
        {
            return Path.Combine(CONSTANTS.COURSES_FOLDER_PATH, course.ID);
        }

        public static string GetFilePath(this Course course)
        {
            return Path.Combine(course.GetCoursePath(), course.ID + CONSTANTS.COURSE_EXT);
        }
    }
}
