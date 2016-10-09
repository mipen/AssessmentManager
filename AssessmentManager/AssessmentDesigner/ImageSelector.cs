using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AssessmentManager
{
    public partial class ImageSelector : Form
    {
        public ImageSelector()
        {
            InitializeComponent();
            btnOK.Enabled = false;
            btnSelectImage.Focus();
        }

        public ImageSelector(string text) : this()
        {
            Text = text + " Image";
        }

        public ImageSelector (string text, Bitmap image) : this(text)
        {
            Image = image;
        }

        public string Path
        {
            get
            {
                return tbPath.Text;
            }
            set
            {
                tbPath.Text = value;
            }
        }

        public Bitmap Image
        {
            get
            {
                return pbImage.Image as Bitmap;
            }
            set
            {
                pbImage.Image = value;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            btnOK.Enabled = true;

            this.AutoSize = false;

            Path = "";
            Image = null;


            pbImage.Width = 260;
            pbImage.Height = 194;

            this.Width = 300;
            this.Height = 350;

            this.AutoSize = true;
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.InitialDirectory = !Path.NullOrEmpty() && Directory.Exists(Path) ? Path : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            o.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (o.ShowDialog() == DialogResult.OK)
            {
                Bitmap prevImage = Image;
                string prevPath = Path;
                btnRemove_Click(sender, e);

                try
                {
                    Image = (Bitmap)Bitmap.FromFile(o.FileName);
                    tbPath.Text = o.FileName;

                    btnOK.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not load image from file: \n" + o.FileName + "\n\n" + ex.Message);
                    Image = prevImage;
                    Path = prevPath;
                }
            }
        }
    }
}
