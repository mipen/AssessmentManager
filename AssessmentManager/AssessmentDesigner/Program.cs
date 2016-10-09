using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Application.Run(form);

        }
    }
}
