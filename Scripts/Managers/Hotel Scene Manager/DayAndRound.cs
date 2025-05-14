using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoManual.Managers
{
    public class DayAndRound
    {
        public const int MIN_DAY = 0;
        public const int MAX_DAY = 4;
        public const int MIN_ROUND = 1;
        public const int MAX_ROUND = 4;
        
        public int CurrentDay { get; private set; } = 0;
        public int CurrentRound  { get; private set; } = 1;
        
        
        /// <summary>
        /// 새로운 일차 시작
        /// </summary>
        public DayAndRound(int currentDay)
        {
            this.CurrentDay = currentDay;
            this.CurrentRound = 1;
        }

        public void IncreaseRound()
        {
            CurrentRound = Mathf.Clamp(CurrentRound + 1, MIN_ROUND, MAX_ROUND);
        }

        public void DecreaseRound()
        {
            CurrentRound = Mathf.Clamp(CurrentRound - 1, MIN_ROUND, MAX_ROUND);
        }

        public void IncreaseDay()
        {
            CurrentDay = Mathf.Clamp(CurrentDay + 1, MIN_DAY, MAX_DAY);
        }

        public void DecreaseDay()
        {
            CurrentDay = Mathf.Clamp(CurrentDay - 1, MIN_DAY, MAX_DAY);
        }

        public void DebugModeDay(int day)
        {
            CurrentDay = day;
        }

        public void DebugModeRound(int round)
        {
            CurrentRound = round;
        }

        public bool LastDayCheck()
        {
            return CurrentDay == MAX_DAY;
        }
        
        public bool LastRoundCheck()
        {
            return CurrentRound == MAX_ROUND;
        }
    }
}


