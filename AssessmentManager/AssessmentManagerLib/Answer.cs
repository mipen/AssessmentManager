using System;

namespace AssessmentManager
{
    [Serializable]
    public class Answer
    {
        private MultiChoiceOption selectedOption = MultiChoiceOption.None;
        private string shortAnswer = null;
        private string longAnswer = null;

        public string LongAnswer
        {
            get
            {
                return longAnswer;
            }

            set
            {
                longAnswer = value;
            }
        }

        public string ShortAnswer
        {
            get
            {
                return shortAnswer;
            }

            set
            {
                shortAnswer = value;
            }
        }

        public MultiChoiceOption SelectedOption
        {
            get
            {
                return selectedOption;
            }

            set
            {
                selectedOption = value;
            }
        }

        public bool Attempted
        {
            get
            {
                return SelectedOption != MultiChoiceOption.None || !LongAnswer.NullOrEmpty() || !ShortAnswer.NullOrEmpty();
            }
        }
    }
}
