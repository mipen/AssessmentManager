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
        private List<MarkingQuestion> markingQuestions = new List<MarkingQuestion>();

        public StudentMarkingData(StudentData d, Assessment assessment)
        {
            studentData = d;
            BuildFromAssessment(assessment);
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

        #region Methods

        private void BuildFromAssessment(Assessment assessment)
        {
            //TODO:: this
        }

        public override string ToString()
        {
            string str = StudentData.UserName;
            if (!Loaded)
                str += " - Unloaded";
            return str;
        }

        #endregion
    }
}
