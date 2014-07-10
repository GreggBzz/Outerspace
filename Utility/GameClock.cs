using System;

namespace OuterSpace
{
    public class CGameClock
    {
        #region Member Variables

        private DateTime clock;
        private DateTime saveClock;
        private Int32 interval;
        
        #endregion

        #region Properties

        //  in seconds
        public DateTime Clock {
            get { return clock; }
            set { clock = value; }
        }
        
        public Int32 Interval {
            get { return interval; }
            set 
            {
                interval = value;
                saveClock = DateTime.Now;
            }
        }

        #endregion

        #region Constructors

        public CGameClock(Int32 startYear, Int32 startMonth, Int32 startDay, Int32 clockInterval) {
            Clock = new DateTime(startYear, startMonth, startDay);
            Interval = clockInterval;
            saveClock = DateTime.Now;
        }

        #endregion

        #region Methods

        public void CheckInterval() 
        {
            if (Interval > 0) 
            {
                TimeSpan timeDiff = DateTime.Now.Subtract(saveClock);

                if (timeDiff.Seconds >= Interval) 
                {
                    UpdateClock();
                }
            }
        }
        
        public void UpdateClock() 
        {
            Clock = Clock.AddDays(1);
            saveClock = DateTime.Now;
        }

        #endregion
    }
}
