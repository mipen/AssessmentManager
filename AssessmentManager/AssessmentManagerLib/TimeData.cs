using System;

namespace AssessmentManager
{
    [Serializable]
    public class TimeData
    {
        public DateTime PlannedStartTime = CONSTANTS.INVALID_DATE;
        public DateTime PlannedFinishTime;
        public DateTime TimeStarted;
        public DateTime TimeFinished;
        public int minutes = 0;
        public int readingMinutes = 0;
        private bool hasReadingTime = false;
        public DateTime ReadingFinishTime;

        public TimeData()
        {

        }

        #region Properties

        public int Minutes
        {
            get
            {
                return minutes;
            }
            set
            {
                minutes = value;
            }
        }

        public int ReadingMinutes
        {
            get
            {
                return readingMinutes;
            }
            set
            {
                readingMinutes = value;
                if (readingMinutes > 0)
                    hasReadingTime = true;
                else
                    hasReadingTime = false;
            }
        }

        public bool HasReadingTime
        {
            get
            {
                return hasReadingTime;
            }
        }

        public bool Finished
        {
            get
            {
                return DateTime.Now >= PlannedFinishTime;
            }
        }

        public TimeSpan TimeRemaining
        {
            get
            {
                return PlannedFinishTime - DateTime.Now;
            }
        }

        public TimeSpan TimeUntilBegin
        {
            get
            {
                if (PlannedStartTime == CONSTANTS.INVALID_DATE || DateTime.Now >= PlannedStartTime)
                    return new TimeSpan(0, 0, 0, 0, 0);

                return PlannedStartTime - DateTime.Now;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return PlannedStartTime == CONSTANTS.INVALID_DATE || DateTime.Now >= PlannedStartTime;
            }
        }


        #endregion

    }
}
