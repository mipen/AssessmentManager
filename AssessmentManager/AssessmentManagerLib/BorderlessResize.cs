using System;
using System.Windows.Forms;
using System.Drawing;

namespace AssessmentManager
{
    /*
    Code from http://stackoverflow.com/questions/2575216/how-to-move-and-resize-a-form-without-a-border
    Answer by user1306322

    Allows for resizing of a borderless form. Override form's WndProc method and call the BorderlessResize method from 
    in there. To show the bounds for the resize handles, call DebugPaint from within the form's OnPaint event handler.
    */
    public class BorderlessResize
    {
        private Form _parent;

        private const int
                    HTLEFT = 10,
                    HTRIGHT = 11,
                    HTTOP = 12,
                    HTTOPLEFT = 13,
                    HTTOPRIGHT = 14,
                    HTBOTTOM = 15,
                    HTBOTTOMLEFT = 16,
                    HTBOTTOMRIGHT = 17;

        const int padding = 10; // you can rename this variable if you like

        Rectangle Top { get { return new Rectangle(0, 0, _parent.ClientSize.Width, padding); } }
        Rectangle Left { get { return new Rectangle(0, 0, padding, _parent.ClientSize.Height); } }
        Rectangle Bottom { get { return new Rectangle(0, _parent.ClientSize.Height - padding, _parent.ClientSize.Width, padding); } }
        Rectangle Right { get { return new Rectangle(_parent.ClientSize.Width - padding, 0, padding, _parent.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, padding, padding); } }
        Rectangle TopRight { get { return new Rectangle(_parent.ClientSize.Width - padding, 0, padding, padding); } }
        Rectangle BottomLeft { get { return new Rectangle(0, _parent.ClientSize.Height - padding, padding, padding); } }
        Rectangle BottomRight { get { return new Rectangle(_parent.ClientSize.Width - padding, _parent.ClientSize.Height - padding, padding, padding); } }

        public BorderlessResize(Form form)
        {
            _parent = form;
        }

        public void DebugPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Green, Top);
            e.Graphics.FillRectangle(Brushes.Green, Left);
            e.Graphics.FillRectangle(Brushes.Green, Right);
            e.Graphics.FillRectangle(Brushes.Green, Bottom);
        }

        public void WndProc(ref Message message)
        {
            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = _parent.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }
    }
}
