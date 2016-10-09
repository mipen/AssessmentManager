using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    [Serializable]
    public class AssessmentScript
    {
        public CourseInformation CourseInformation = null;
        public AssessmentInformation AssessmentInfo = null;
        private List<Question> questions = new List<Question>();
        private Dictionary<string, Answer> answers = new Dictionary<string, Answer>();
        private TimeData timeData = null;
        private StudentData studentData = null;
        public bool Started = false;
        private bool published = false;

        public AssessmentScript()
        {
        }

        #region Properties

        public List<Question> Questions => questions;
        public Dictionary<string, Answer> Answers => answers;
        public TimeData TimeData => timeData;
        public StudentData StudentData => studentData;

        public int TotalMarks
        {
            get
            {
                int num = 0;
                foreach (var q in Questions)
                {
                    num += q.TotalMarks;
                }
                return num;
            }
        }

        public bool Published
        {
            get
            {
                return published;
            }
        }

        #endregion

        #region Methods

        public static AssessmentScript BuildFromAssessment(Assessment assessment)
        {
            AssessmentScript script = new AssessmentScript();
            script.questions = assessment.Questions;
            script.published = false;
            //Populate answers dictionary with answer objects for each question
            foreach (var q in script.Questions)
            {
                q.AddToAnswerDict(script.Answers);
            }
            script.timeData = new TimeData()
            {
                Minutes = 60
            };
            script.AssessmentInfo = new AssessmentInformation()
            {
                AssessmentName = "Assessment",
                Author = "",
                AssessmentWeighting = 0
            };

            return script;
        }

        public static AssessmentScript BuildForPublishing(Assessment assessment, StudentData data, AssessmentInformation info)
        {
            AssessmentScript script = BuildFromAssessment(assessment);
            //Set the data
            script.studentData = data;
            script.AssessmentInfo = info;
            script.timeData = data.GenerateTimeData();
            script.published = true;
            return script;
        }

        #endregion
    }
}
