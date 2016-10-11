using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    [Serializable]
    public class MarkingQuestion
    {
        private string questionName = "";
        private List<MarkingQuestion> subMarkingQuestions = new List<MarkingQuestion>();
        private string markerResponse = "";
        private int assignedMarks = 0;

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

        public string MarkerResponse
        {
            get
            {
                return markerResponse;
            }
            set
            {
                markerResponse = value;
            }
        }

        public int AssignedMarks
        {
            get
            {
                return assignedMarks;
            }
            set
            {
                assignedMarks = value;
            }
        }

        public List<MarkingQuestion> SubMarkingQuestions
        {
            get
            {
                return subMarkingQuestions;
            }
        }

        public bool HasSubQuestions
        {
            get
            {
                return SubMarkingQuestions != null && SubMarkingQuestions.Count > 0;
            }
        }

        #endregion

        #region Methods

        public MarkingQuestionNode BuildNode()
        {
            MarkingQuestionNode node = new MarkingQuestionNode(this);
            if(HasSubQuestions)
            {
                foreach(var sq in SubMarkingQuestions)
                {
                    node.Nodes.Add(sq.BuildNode());
                }
            }
            return node;
        }

        public override string ToString()
        {
            return questionName;
        }

        #endregion
    }
}
