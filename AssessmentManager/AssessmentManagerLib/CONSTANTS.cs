using System;
using System.IO;
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
        public static readonly string SPREADSHEET_EXT = ".xlsx";
        public static readonly string TEXT_EXT = ".txt";

        public static readonly string ASSESSMENT_FILTER = $"Assessment Files (*{ASSESSMENT_EXT}) | *{ASSESSMENT_EXT}";
        public static readonly string ASSESSMENT_SCRIPT_FILTER = $"Assessment Script Files (*{ASSESSMENT_SCRIPT_EXT}) | *{ASSESSMENT_SCRIPT_EXT}";
        public static readonly string COMBINED_FILTER = $"Assessment Files (*{ASSESSMENT_EXT}; *{ASSESSMENT_SCRIPT_EXT}) | *{ASSESSMENT_EXT}; *{ASSESSMENT_SCRIPT_EXT}";
        public static readonly string XML_FILTER = $"XML Files (*{XML_EXT}) | *{XML_EXT}";
        public static readonly string PDF_FILTER = $"PDF Files (*{PDF_EXT}) | *{PDF_EXT}";
        public static readonly string SPREADSHEET_FILTER = $"Spreadsheet Files (*{SPREADSHEET_EXT}) | *{SPREADSHEET_EXT}";
        public static readonly string TEXT_FILTER = $"Text Files (*{TEXT_EXT}) | *{TEXT_EXT}";
        public static readonly string ALL_FILTER = "All Files (*.*) | *.*";

        public static readonly string DESKTOP_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        public static readonly string DOCUMENTS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static readonly string DATA_FOLDER_NAME = "AssessmentManager";
        public static readonly string DATA_FOLDER_PATH = Path.Combine(DOCUMENTS_PATH, DATA_FOLDER_NAME);
        public static readonly string COURSES_FOLDER_NAME = "Courses";
        public static readonly string COURSES_FOLDER_PATH = Path.Combine(DATA_FOLDER_PATH, COURSES_FOLDER_NAME);
        public static readonly string SETTINGS_FILE_NAME = "settings.settings";
        public static readonly string SETTINGS_FILE_PATH = Path.Combine(DATA_FOLDER_PATH, SETTINGS_FILE_NAME);
        public static readonly string TEMP_PDF_PATH = Path.Combine(DATA_FOLDER_PATH, "temp");
        public static readonly string RULES_FILE_NAME = "rules.txt";
        public static readonly string RULES_FILE_PATH = Path.Combine(Application.StartupPath, RULES_FILE_NAME);

        public static readonly string C_ROOT = @"C:\";

        public static readonly string EXAMINEE_EXE = "Examinee.exe";
        public static readonly string SHARED_DLL = "AssessmentManagerLib.dll";
        public static readonly string ASSESSMENT_DESIGNER_EXE = "Assessment Designer.exe";
        public static readonly string ITEXTSHARP_DLL = "itextsharp.dll";

        public const string INVALID = "not found";


        public static readonly DateTime INVALID_DATE = new DateTime(1990, 1, 1, 1, 1, 1, 1);

        public static readonly string QUESTION_FORMAT_STRING = DataFormats.GetFormat(typeof(Question).FullName).Name;

        public static string AUTOSAVE_FOLDER_NAME(string assessmentName)
        {
            string str = @assessmentName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace(".", "");
            return str + "_autosaves";
        }

        public static string USERNAME_FILE_PATH(string assessmentPath, string deployTarget)
        {
            return Path.Combine(assessmentPath, new DirectoryInfo(deployTarget).Name + SPREADSHEET_EXT);
        }
    }
}
