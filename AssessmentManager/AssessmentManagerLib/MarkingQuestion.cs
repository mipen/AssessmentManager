using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    public class MarkingQuestion
    {
        private string questionName = "";

        public MarkingQuestion(string questionName)
        {
            this.questionName = questionName;
        }

        #region Properties

        public string QuestionName
        {
            get
            {
                return questionName;
            }
        }

        #endregion

        public override string ToString()
        {
            return questionName;
        }
    }
}
