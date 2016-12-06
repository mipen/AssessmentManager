using System;
using System.IO;
using System.Windows.Forms;

namespace AssessmentManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Settings.Init();
            MainForm form = new MainForm();
            if (args.Length >= 1)
            {
                form.OpenFromFile(args[0]);
            }
            //Look for the iTextSharp dll, warn that some parts of the program won't work if it isn't found
            string itsPath = Path.Combine(Application.StartupPath, CONSTANTS.ITEXTSHARP_DLL);
            if (!File.Exists(itsPath))
            {
                if (MessageBox.Show($"{CONSTANTS.ITEXTSHARP_DLL} not found in {Application.StartupPath}. Parts of the program dealing with PDF files will not work correctly without it. Continue?", $"{CONSTANTS.ITEXTSHARP_DLL} not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    Application.Run(form);
            }
            else
                Application.Run(form);

        }
    }
}
