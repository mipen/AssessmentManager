using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    [Serializable]
    public class AssessmentInformation
    {
        public string AssessmentName    = "";
        public string Author            = "";
        public int AssessmentWeighting  = 0;

        public AssessmentInformation()
        {

        }

        public AssessmentInformation(string assessmentName, string author, int weighting)
        {
            AssessmentName = assessmentName;
            Author = author;
            AssessmentWeighting = weighting;
        }
    }
}
