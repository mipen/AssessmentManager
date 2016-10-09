using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AssessmentManager
{
    /*
    Code from this article: http://www.codeproject.com/Articles/11114/Move-window-form-without-Titlebar-in-C

    Call the MouseDown method inside the form's MouseDown event handler. Can also be used inside other controls,
    just pass the Handle for the form.
    */
    public class BorderlessMove
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private Form _parent;

        public BorderlessMove(Form form)
        {
            _parent = form;
        }

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                 int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public void MouseDown(MouseEventArgs e, IntPtr handle)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
