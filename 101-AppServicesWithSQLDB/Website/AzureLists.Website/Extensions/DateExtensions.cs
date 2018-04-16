using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureLists.Website.Extensions
{
    public static class DateExtensions
    {
        public static string TimeSinceOrUntil(this DateTime d)
        {
            if (DateTime.Now > d)
                return TimeSince(d);
            else
                return TimeUntil(d);
        }

        public static string TimeSince(DateTime d)
        {
            var now = DateTime.Now;
            if (now.Day == d.Day && now.Month == d.Month && now.Year == d.Year)
                return "Today";
            if (now.AddDays(-1).Day == d.Day && now.AddDays(-1).Month == d.Month && now.AddDays(-1).Year == d.Year)
                return "Yesterday";

            var ts = DateTime.Now.ToStartOfDay().Subtract(d.ToStartOfDay());
            int dayDiff = (int)ts.TotalDays;
            if (dayDiff < 7)
                return (DatesAreInTheSameWeek(d, DateTime.Now)) ? "This Week" : $"{ts.Days} days ago";
            if (dayDiff < 91)
                return string.Format("{0} weeks ago", Math.Floor((double)dayDiff / 7));
            else
                return string.Format("{0} months ago", Math.Ceiling((double)dayDiff / 30));
        }

        public static string TimeUntil(DateTime d)
        {
            var now = DateTime.Now;
            if (now.Day == d.Day && now.Month == d.Month && now.Year == d.Year)
                return "Today";
            if (now.AddDays(+1).Day == d.Day && now.AddDays(+1).Month == d.Month && now.AddDays(+1).Year == d.Year)
                return "Tommorrow";

            var ts = d.ToStartOfDay().Subtract(DateTime.Now.ToStartOfDay());
            int dayDiff = (int)ts.TotalDays;
            if (dayDiff < 7)
                return (DatesAreInTheSameWeek(d, DateTime.Now)) ? "This Week" : $"{ts.Days} days time";
            if (dayDiff < 91)
                return string.Format("{0} weeks time", Math.Floor((double)dayDiff / 7));
            else
                return string.Format("{0} months time", Math.Ceiling((double)dayDiff / 30));
        }

        private static bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

            return d1 == d2;
        }

        public static DateTime ToStartOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

    }
}