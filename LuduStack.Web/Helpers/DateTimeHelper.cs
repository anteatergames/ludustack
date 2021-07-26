using Microsoft.Extensions.Localization;
using System;
using System.Text;

namespace LuduStack.Web.Helpers
{
    public static class DateTimeHelper
    {
        public static string DateTimeToCreatedAgoMessage(DateTime createDate, IStringLocalizer localizer)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - createDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? localizer["one second ago"] : string.Format("{0} {1}", ts.Seconds, localizer["seconds ago"]);
            }

            if (delta < 2 * MINUTE)
            {
                return localizer["a minute ago"];
            }

            if (delta < 45 * MINUTE)
            {
                return string.Format("{0} {1}", ts.Minutes, localizer["minutes ago"]);
            }

            if (delta < 90 * MINUTE)
            {
                return localizer["an hour ago"];
            }

            if (delta < 24 * HOUR)
            {
                return string.Format("{0} {1}", ts.Hours, localizer["hours ago"]);
            }

            if (delta < 48 * HOUR)
            {
                return localizer["yesterday"];
            }

            if (delta < 30 * DAY)
            {
                return string.Format("{0} {1}", ts.Days, localizer["days ago"]);
            }

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? localizer["one month ago"] : string.Format("{0} {1}", months, localizer["months ago"]);
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? localizer["one year ago"] : string.Format("{0} {1}", years, localizer["years ago"]);
            }
        }

        public static string DurationBetweenDates(DateTime dateOne, DateTime dateTwo, IStringLocalizer localizer)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            TimeSpan ts = new TimeSpan(dateTwo.Ticks - dateOne.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
            {
                return ts.Seconds == 1 ? localizer["one second"] : string.Format("{0} {1}", ts.Seconds, localizer["seconds"]);
            }

            if (delta < 2 * MINUTE)
            {
                return localizer["a minute"];
            }

            if (delta < 45 * MINUTE)
            {
                return string.Format("{0} {1}", ts.Minutes, localizer["minutes"]);
            }

            if (delta < 90 * MINUTE)
            {
                return localizer["an hour"];
            }

            if (delta < 24 * HOUR)
            {
                return string.Format("{0} {1}", ts.Hours, localizer["hours"]);
            }

            if (delta < 30 * DAY)
            {
                return string.Format("{0} {1}", ts.Days, localizer["days"]);
            }

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? localizer["one month"] : string.Format("{0} {1}", months, localizer["months"]);
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? localizer["one year"] : string.Format("{0} {1}", years, localizer["years"]);
            }
        }

        public static string DurationBetweenDatesForGameJam(DateTime dateOne, DateTime dateTwo, IStringLocalizer localizer)
        {
            TimeSpan ts = new TimeSpan(dateTwo.Ticks - dateOne.Ticks);

            StringBuilder sb = new StringBuilder();

            decimal totalHours = Math.Ceiling((decimal)ts.TotalHours);

            if (totalHours < 73)
            {
                if (totalHours == 1)
                {
                    sb.Append(localizer["{0} hour", totalHours]);
                }
                else
                {
                    sb.Append(localizer["{0} hours", totalHours]);
                }
            }
            else
            {
                if (ts.Days > 0)
                {
                    if (ts.Days == 1)
                    {
                        sb.Append(localizer["{0} day", ts.Days]);
                    }
                    else
                    {
                        sb.Append(localizer["{0} days", ts.Days]);
                    }
                }

                if (ts.Hours > 0)
                {
                    decimal remainingHours = Math.Ceiling((decimal)ts.Hours);

                    sb.Append(" ");
                    if (remainingHours == 1)
                    {
                        sb.Append(localizer["and {0} hour", remainingHours]);
                    }
                    else
                    {
                        sb.Append(localizer["and {0} hours", remainingHours]);
                    }
                }
            }

            return sb.ToString();
        }
    }
}