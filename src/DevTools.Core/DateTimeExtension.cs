using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Core
{
    public static class DateTimeExtension
    {
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        public static bool IsBetween(this DateTime dateTime, DateTime lowerBound, DateTime upperBound)
        {
            if (lowerBound > upperBound)
            {
                // for those who switch those two
                return dateTime.IsBetween(upperBound, lowerBound);
            }

            return lowerBound <= dateTime && dateTime <= upperBound;
        }

        public static bool IsWorkday(this DateTime dateTime)
        {
            return !dateTime.IsWeekend();
        }

        public static bool IsWeekend(this DateTime dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Sunday || dateTime.DayOfWeek == DayOfWeek.Saturday;
        }

        public static bool IsInFuture(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime() > DateTime.UtcNow;
        }

        public static bool IsPast(this DateTime dateTime)
        {
            return dateTime < DateTime.Now;
        }
    }
}
