using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    public class FileCellItem
    {

        private string path = "";

        public FileCellItem(string path)
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
            return new DirectoryInfo(@path).Name;
        }
    }
}
