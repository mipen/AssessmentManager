using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssessmentManager
{
    public static class CONSTANTS
    {
        public static readonly string ASSESSMENT_EXT = ".exm";
        public static readonly string ASSESSMENT_SCRIPT_EXT = ".exms";
        public static readonly string ASSESSMENT_SESSION_EXT = ".as";
        public static readonly string XML_EXT = ".xml";
        public static readonly string COURSE_EXT = ".crse";
        public static readonly string PDF_EXT = ".pdf";

        public static readonly string ASSESSMENT_FILTER = $"Assessment Files (*{ASSESSMENT_EXT}) | *{ASSESSMENT_EXT}";
        public static readonly string ASSESSMENT_SCRIPT_FILTER = $"Assessment Script Files (*{ASSESSMENT_SCRIPT_EXT}) | *{ASSESSMENT_SCRIPT_EXT}";
        public static readonly string COMBINED_FILTER = $"Assessment Files (*{ASSESSMENT_EXT}; *{ASSESSMENT_SCRIPT_EXT}) | *{ASSESSMENT_EXT}; *{ASSESSMENT_SCRIPT_EXT}";
        public static readonly string XML_FILTER = $"XML Files (*{XML_EXT}) | *{XML_EXT}";
        public static readonly string PDF_FILTER = $"PDF Files (*{PDF_EXT}) | *{PDF_EXT}";
        public static readonly string ALL_FILTER = "All Files (*.*) | *.*";

        public static string DESKTOP_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static string DOCUMENTS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string TEMP_PDF_PATH = Path.Combine(Application.StartupPath, "temp");

        public static readonly string EXAMDRIVE_ROOT = @"X:\";
        public static readonly string C_ROOT = @"C:\";

        public static readonly string STUDENT_STAGE = "STUDENTS_CHECK";

        public static readonly string COURSES_FOLDER_NAME = "Courses";

        public static readonly DateTime INVALID_DATE = new DateTime(1975, 1, 1, 1, 1, 1, 1);

        public static readonly string QUESTION_FORMAT_STRING = DataFormats.GetFormat(typeof(Question).FullName).Name;

        public static string AUTOSAVE_FOLDER_NAME(string assessmentName)
        {
            return $"{assessmentName.Replace(" ","")}_autosaves";
        }
    }
}
