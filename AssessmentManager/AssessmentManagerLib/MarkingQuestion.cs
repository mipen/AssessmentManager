using System;
using System.Collections.Generic;

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

        public int TotalAssignedMarks
        {
            get
            {
                int num = AssignedMarks;
                if(HasSubQuestions)
                {
                    foreach (var q in SubMarkingQuestions)
                        num += q.TotalAssignedMarks;
                }
                return num;
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

        public void GetAutoMarkingQuestions(AssessmentScript script, Dictionary<MarkingQuestion, Question> dict)
        {
            Question q = script.FindQuestion(QuestionName);
            if(q!= null)
            {
                if(q.AnswerType==AnswerType.Multi || q.AnswerType==AnswerType.Single)
                {
                    dict.Add(this, q);
                }
            }
            if(HasSubQuestions)
            {
                foreach(var smq in SubMarkingQuestions)
                {
                    smq.GetAutoMarkingQuestions(script, dict);
                }
            }
        }

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
