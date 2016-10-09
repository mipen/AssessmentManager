namespace AssessmentManager
{
    public class FileListItem
    {
        private string path = "";

        public FileListItem(string path)
        {
            this.path = path;
        }

        public string Path
        {
            get
            {
                return path;
            }
        }

        public override string ToString()
        {
            return System.IO.Path.GetFileName(path);
        }
    }
}
