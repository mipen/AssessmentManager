using System;
using System.Collections.Generic;

namespace AssessmentManager
{
    [Serializable]
    public class Student
    {
        private string userName;
        private string firstName;
        private string lastName;
        private string studentID;

        public Student()
        {

        }

        public Student(string userName, string lastName, string firstName, string studentID)
        {
            this.userName = userName;
            this.lastName = lastName;
            this.firstName = firstName;
            this.studentID = studentID;
        }

        #region Properties

        public string StudentID
        {
            get
            {
                return studentID;
            }

            set
            {
                studentID = value;
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        [Flags]
        public enum ErrorType
        {
            UserName,
            FirstName,
            LastName,
            StudentID,
            StartTime,
            AssessmentLength,
            ReadingTime,
            AccountName,
            AccountPassword
        }

        #endregion

        /// <summary>
        /// Checks that all data is valid for the student
        /// </summary>
        /// <returns>Returns true if everything is okay, false if there is an error.</returns>
        public virtual bool ResolveErrors()
        {
            return !FirstName.NullOrEmpty() && !LastName.NullOrEmpty() && !StudentID.NullOrEmpty() && !UserName.NullOrEmpty();
        }

        public virtual List<ErrorType> GetErrors()
        {
            List<ErrorType> list = new List<ErrorType>();

            if (FirstName.NullOrEmpty())
                list.Add(ErrorType.FirstName);
            if (LastName.NullOrEmpty())
                list.Add(ErrorType.LastName);
            if (UserName.NullOrEmpty())
                list.Add(ErrorType.UserName);
            if (StudentID.NullOrEmpty())
                list.Add(ErrorType.StudentID);

            return list;
        }

        public virtual string AnyIdentifiableTag(int row = -1)
        {
            if (!UserName.NullOrEmpty())
                return UserName;
            if(!FirstName.NullOrEmpty())
            {
                if (!LastName.NullOrEmpty())
                    return FirstName + " " + LastName;
                else
                    return FirstName;
            }
            if (!LastName.NullOrEmpty())
                return LastName;
            if (!StudentID.NullOrEmpty())
                return StudentID;
            if (row >= 0)
                return $"on row {row}";

            return "Unidentifiable student";
        }

        public Student Clone()
        {
            Student s = new Student(userName, lastName, firstName, studentID);
            return s;
        }
    }
}
