using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssessmentManager
{
    public partial class ImageDisplay : Form
    {
        public ImageDisplay(string questionName, Image image)
        {
            InitializeComponent();
            Text = questionName + " Image";
            pbImage.Image = image;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - Width, Screen.PrimaryScreen.WorkingArea.Bottom - Height);
            ImageTracker.RegisterImageDisplay(this);
        }

        private void ImageDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            ImageTracker.DeregisterImageDisplay(this);
        }
    }
}
