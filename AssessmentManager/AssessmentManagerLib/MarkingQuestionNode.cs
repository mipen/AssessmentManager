using System.Windows.Forms;

namespace AssessmentManager
{
    public class MarkingQuestionNode : TreeNode
    {
        private MarkingQuestion markingQuestion = null;

        public MarkingQuestionNode(MarkingQuestion mq)
        {
            markingQuestion = mq;
            Name = mq.QuestionName;
            Text = Name;
        }

        public MarkingQuestion MarkingQuestion
        {
            get
            {
                return markingQuestion;
            }
        }

        public override string ToString()
        {
            return MarkingQuestion.QuestionName;
        }
    }
}
