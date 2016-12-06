using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AssessmentManager
{
    [Serializable]
    public class StudentMarkingData
    {
        private StudentData studentData = null;
        public bool Loaded = false;
        private List<AssessmentScriptListItem> scripts = new List<AssessmentScriptListItem>();
        private List<MarkingQuestion> markingQuestions = new List<MarkingQuestion>();
        public DateTime DateLastLoaded = CONSTANTS.INVALID_DATE;

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

        public List<MarkingQuestion> MarkingQuestions
        {
            get
            {
                return markingQuestions;
            }
        }

        public decimal FinalMark
        {
            get
            {
                decimal num = 0;
                foreach (var q in MarkingQuestions)
                    num += q.TotalAssignedMarks;
                return num;
            }
        }

        public int TotalAvailableMarks
        {
            get
            {
                return (int)Scripts.First()?.Script.TotalMarks;
            }
        }

        #endregion

        #region Methods

        private void BuildFromAssessment(Assessment assessment)
        {
            markingQuestions.Clear();
            foreach(Question q in assessment.Questions)
            {
                q.BuildMarkingQuestion(markingQuestions);
            }
        }

        public void FillTreeView(TreeView tv)
        {
            tv.Nodes.Clear();
            foreach(var mq in markingQuestions)
            {
                tv.Nodes.Add(mq.BuildNode());
            }
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
