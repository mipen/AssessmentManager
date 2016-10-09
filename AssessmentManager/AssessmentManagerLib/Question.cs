using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    [Serializable]
    public class Question
    {
        private string name = "Question";
        private int marks = 0;
        private string questionText = "";
        private string questionTextRaw = "";
        private string modelAnswer;
        private string optionA, optionB, optionC, optionD;
        private MultiChoiceOption correctOption = MultiChoiceOption.A;
        private AnswerType answerType = AnswerType.Open;
        private List<Question> subQuestions = new List<Question>();
        private Bitmap image;

        public Question()
        {
        }

        public Question(string name)
        {
            Name = name;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string QuestionText
        {
            get
            {
                return questionText;
            }
            set
            {
                questionText = value;
            }
        }

        public string QuestionTextRaw
        {
            get
            {
                return questionTextRaw;
            }
            set
            {
                questionTextRaw = value;
            }
        }

        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }

        #region SubQuestions
        public List<Question> SubQuestions => subQuestions;
        public bool HasSubQuestions => subQuestions.Count != 0;
        #endregion

        #region Answer
        public AnswerType AnswerType
        {
            get
            {
                return answerType;
            }
            set
            {
                answerType = value;
            }
        }

        public string ModelAnswer
        {
            get
            {
                return modelAnswer;
            }
            set
            {
                modelAnswer = value;
            }
        }

        public string OptionA
        {
            get
            {
                return optionA;
            }
            set
            {
                optionA = value;
            }
        }

        public string OptionB
        {
            get
            {
                return optionB;
            }
            set
            {
                optionB = value;
            }
        }

        public string OptionC
        {
            get
            {
                return optionC;
            }
            set
            {
                optionC = value;
            }
        }

        public string OptionD
        {
            get
            {
                return optionD;
            }
            set
            {
                optionD = value;
            }
        }

        public MultiChoiceOption CorrectOption
        {
            get
            {
                return correctOption;
            }
            set
            {
                correctOption = value;
            }
        }
        #endregion

        #region Marks
        /// <summary>
        /// The sum of all sub question marks, including the marks for this question.
        /// </summary>
        public int TotalMarks
        {
            get
            {
                if (!HasSubQuestions)
                    return Marks;
                else
                {
                    int num = Marks;
                    foreach (var q in SubQuestions)
                    {
                        if (q.AnswerType == AnswerType.None)
                            continue;
                        num += q.TotalMarks;
                    }
                    return num;
                }
            }
        }

        /// <summary>
        /// The number of marks for the question.
        /// </summary>
        public int Marks
        {
            get
            {
                if (AnswerType == AnswerType.None)
                    return 0;
                return marks;
            }
            set
            {
                marks = value;
            }
        }

        public void CheckMissingMarks(List<Question> questions)
        {
            if (AnswerType != AnswerType.None && Marks == 0)
                questions.Add(this);
            if (HasSubQuestions)
            {
                foreach (var q in SubQuestions)
                {
                    q.CheckMissingMarks(questions);
                }
            }

        }

        public int AttemptedMarks(Dictionary<string, Answer> dict)
        {
            int num = 0;
            if (AnswerType != AnswerType.None)
            {
                Answer answer = dict[Name];
                if (answer != null && answer.Attempted)
                {
                    num += Marks;
                }
            }

            if(HasSubQuestions)
            {
                foreach(var q in SubQuestions)
                {
                    num += q.AttemptedMarks(dict);
                }
            }
            return num;
        }

        #endregion

        public void AddToAnswerDict(Dictionary<string, Answer> dict)
        {
            if (AnswerType != AnswerType.None)
            {
                if (!dict.ContainsKey(Name))
                    dict.Add(Name, new Answer());
            }

            if (HasSubQuestions)
            {
                foreach(var q in SubQuestions)
                {
                    q.AddToAnswerDict(dict);
                }
            }
        }

        public Question Clone(bool includeSubQuestions = false)
        {
            Question q = new Question("unnamed");

            q.Marks = marks;
            q.QuestionText = QuestionText;
            q.QuestionTextRaw = QuestionTextRaw;
            q.ModelAnswer = ModelAnswer;
            q.OptionA = OptionA;
            q.OptionB = OptionB;
            q.OptionC = OptionC;
            q.OptionD = OptionD;
            q.CorrectOption = CorrectOption;
            q.AnswerType = AnswerType;
            q.Image = Image;
            if(includeSubQuestions && HasSubQuestions)
            {
                foreach(var sq in SubQuestions)
                {
                    q.SubQuestions.Add(sq.Clone(includeSubQuestions));
                }
            }

            return q;
        }
    }
}