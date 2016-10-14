using System;
using System.Collections.Generic;

namespace AssessmentManager
{
    [Serializable]
    public class Assessment
    {
        private List<Question> questions = new List<Question>();

        public Assessment()
        {

        }

        public int TotalMarks
        {
            get
            {
                int num=0;
                foreach (var q in Questions)
                {
                    num += q.TotalMarks;
                }
                return num;
            }
        }

        public List<Question> Questions => questions;

        #region Add Question Methods
        /// <summary>
        /// Add a new top level question to the assessment at the end of the list
        /// </summary>
        public void AddQuestion()
        {
            AddQuestion("unnamed");
        }

        public void AddQuestion(string name)
        {
            questions.Add(new Question(name));
        }
        #endregion

        public List<Question> CheckMissingMarks()
        {
            List<Question> list = new List<Question>();
            foreach(var q in Questions)
            {
                q.CheckMissingMarks(list);
            }
            return list;
        }

    }
}
