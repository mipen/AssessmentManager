using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentManager
{
    [Serializable]
    public class AssessmentSession
    {
        private AssessmentInformation assessmentInfo = new AssessmentInformation();
        private string assessmentFileName = "";
        private string courseID = "";
        private string deploymentTarget = "";
        private DateTime startTime;
        private DateTime deploymentTime;
        private int assessmentLength = 0;
        private int readingTime = 0;
        private string restartPassword;
        private List<StudentData> studentData = new List<StudentData>();
        private List<string> additionalFiles = new List<string>();

        public string FolderPath = "";

        public AssessmentSession(string courseID, string deploymentTarget, AssessmentInformation assessmentInfo, string assessmentFileName, DateTime startTime, int assessmentLength, int readingTime, string restartPassword, List<StudentData> studentData, List<string> additionalFiles, DateTime deploymentTime)
        {
            this.assessmentInfo = assessmentInfo;
            this.assessmentFileName = assessmentFileName;
            this.courseID = courseID;
            this.deploymentTarget = deploymentTarget;
            this.startTime = startTime;
            this.assessmentLength = assessmentLength;
            this.readingTime = readingTime;
            this.restartPassword = restartPassword;
            this.studentData = studentData;
            this.additionalFiles = additionalFiles;
            this.deploymentTime = deploymentTime;
        }

        public string CourseID
        {
            get
            {
                return courseID;
            }
        }

        public AssessmentInformation AssessmentInfo
        {
            get
            {
                return assessmentInfo;
            }
        }

        public string AssessmentFileName
        {
            get
            {
                return assessmentFileName;
            }
        }

        public string DeploymentTarget
        {
            get
            {
                return deploymentTarget;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
        }

        public DateTime DeploymentTime
        {
            get
            {
                return deploymentTime;
            }
        }

        public int AssessmentLength
        {
            get
            {
                return assessmentLength;
            }
        }

        public int ReadingTime
        {
            get
            {
                return readingTime;
            }
        }

        public string RestartPassword
        {
            get
            {
                return restartPassword;
            }
        }

        public List<StudentData> StudentData
        {
            get
            {
                return studentData;
            }
        }

        /// <summary>
        /// Holds the names of the addtional files deployed with the assessment. Does not hold a file path, only name and extension.
        /// </summary>
        public List<string> AdditionalFiles
        {
            get
            {
                return additionalFiles;
            }
        }

    }
}
