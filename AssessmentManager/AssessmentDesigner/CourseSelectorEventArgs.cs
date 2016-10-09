using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    public class CourseSelectorEventArgs : EventArgs
    {
        public bool ListUpdate = false;
        public CourseSelectorEventArgs(bool listUpdate = false)
        {
            ListUpdate = listUpdate;
        }
    }
}
