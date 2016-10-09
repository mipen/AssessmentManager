using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    [Serializable]
    public class StudentMarkingData
    {
        private StudentData studentData = null;
        public bool Loaded = false;
        private List<AssessmentScriptListItem> scripts = new List<AssessmentScriptListItem>();

        public StudentMarkingData(StudentData d)
        {
            studentData = d;
        }

        #region Properties

        public StudentData StudentData
        {
            get
            {
                return studentData;
            }
        }

        public List<AssessmentScriptListItem> Scripts
        {
            get
            {
                return scripts;
            }
        }

        #endregion

        public override string ToString()
        {
            string str = StudentData.UserName;
            if (!Loaded)
                str += " - Unloaded";
            return str;
        }
    }
}
