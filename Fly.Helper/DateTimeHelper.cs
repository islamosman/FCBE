using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Helper
{
    public static class DateTimeHelper
    {


        public static string LongYeayIsoDateFormat => "yyyyMMdd";
        public static string ShortYearDateFormat => "dd/MM/yy";
        public static string UserDateFormat1 => "d/M/yyyy";
        public static string UserDateFormat2 => "dd/M/yyyy";
        public static string UserDateFormat3 => "d/MM/yyyy";
        public static string DateFormat => "dd/MM/yyyy";
        public static string DateTimeFormat => "dd/MM/yyyy hh:mm:ss tt";
        public static string MonthYearFormat => "MMMM yyyy";
        public static string IsoYearFormat => "yyyy";
        private static string[] GetFormats()
        {
            return new[]
            {
                 DateTimeFormat, DateFormat,UserDateFormat1,UserDateFormat2,UserDateFormat3, ShortYearDateFormat, LongYeayIsoDateFormat, MonthYearFormat,
                DateTimeFormat.Replace('/', '.'),
                DateTimeFormat.Replace('/', '-'),
                DateFormat.Replace('/', '.'),
                DateFormat.Replace('/', '-'),
                ShortYearDateFormat.Replace('/', '.'),
                ShortYearDateFormat.Replace('/', '-'),
                 UserDateFormat1.Replace('/', '-'),
                 UserDateFormat2.Replace('/', '-'),
                 UserDateFormat3.Replace('/', '-')
            };
        }

        public static string GetFormattedYear(this DateTime dateTime)
        {
            return dateTime.ToString(IsoYearFormat);
        }

        public static int CalculateAge(this DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;
            return age;
        }

        public static bool IsValidDate(this string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
                return false;
            if (dateTime.Length <= 7)
            {
                dateTime = "01/" + dateTime;
            }
            var date = dateTime.Length == 7 ? dateTime.Remove(0, 1) : dateTime;
            var formats = GetFormats();
            DateTime retDate;
            var parsed = DateTime.TryParseExact(date, formats, null, DateTimeStyles.None, out retDate);
            return parsed;
        }


        public static string GetFormattedDate(this DateTime dateTime)
        {
            return dateTime.ToString(DateFormat);
        }
        public static string GetFormattedDate(this string dateTime)
        {
            if (dateTime == null) return "-";
            var date = dateTime.Length == 7 ? dateTime.Remove(0, 1) : dateTime;
            var formats = GetFormats();
            DateTime retDate;
            var parsed = DateTime.TryParseExact(date, formats, null, DateTimeStyles.None, out retDate);
            return parsed ? retDate.ToString(DateFormat) : string.Empty;
        }
        public static string GetFormattedDate(this string dateTime, string format)
        {
            string date = string.Empty;
            if (!string.IsNullOrEmpty(dateTime) && !string.IsNullOrEmpty(format))
            {
                date = dateTime.Length == 7 ? dateTime.Remove(0, 1) : dateTime;
            }

            var formats = GetFormats();
            DateTime retDate;
            var parsed = DateTime.TryParseExact(date, formats, null, DateTimeStyles.None, out retDate);
            return parsed ? retDate.ToString(format) : string.Empty;
        }

        public static string GetPreviousMonthDate()
        {
            var month = DateTime.Now.Month - 1;
            var year = DateTime.Now.Year;
            if (month == 0)
            {
                month = 12;
                year -= 1;
            }
            return new DateTime(year, month, 1).ToString(MonthYearFormat);
        }
        public static string GetMonthLastDate(int month, int year)
        {
            return new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString(DateFormat);
        }

        //public static string GetIsoDate(this string dateTime)
        //{
        //    var date = dateTime.GetDateFromString();
        //    var isoDate = "1" + date.ToString(IsoDateFormat);
        //    return isoDate;
        //}
        public static string GetLongYearIsoDate(this string dateTime)
        {
            var date = dateTime.GetDateFromString();
            var isoDate = date.ToString(LongYeayIsoDateFormat);
            return isoDate;
        }
        public static string GetLongYearIsoDate(this DateTime dateTime)
        {
            var isoDate = dateTime.ToString(LongYeayIsoDateFormat);
            return isoDate;
        }
        public static string GetLongYearIsoDate(this DateTime? dateTime)
        {
            if (dateTime.HasValue == false) return "";
            var isoDate = dateTime.Value.ToString(LongYeayIsoDateFormat);
            return isoDate;
        }
        public static string GetTodayString()
        {
            return DateTime.Now.ToString(DateFormat);
        }
        public static string GetTodayLongYearIsoFormat()
        {
            return DateTime.Now.ToString(LongYeayIsoDateFormat);
        }

        public static string GetNowDatetimeString()
        {
            return GetNowDatetime().ToString(CultureInfo.InvariantCulture);
        }
        public static DateTime GetNowDatetime()
        {
            var zoneDifference = int.Parse(ConfigurationManager.AppSettings["zoneDifference"]);
            return DateTime.UtcNow.AddHours(zoneDifference);
        }

        public static bool IsDateGreaterThanOrEqual(DateTime firstDate, DateTime secondDate)
        {
            int daydiff = (int)((firstDate - secondDate).TotalDays);
            return daydiff <= 0;
        }

        public static string GetFormatedDate(this DateTime? date)
        {
            return date?.ToString(DateFormat) ?? "";
        }
        public static string GetFormatedDate(this DateTime date)
        {
            return date.ToString(DateFormat);
        }

        public static DateTime GetFormatDateFromString(this string dateString)
        {

            if (!string.IsNullOrEmpty(dateString))
            {
                if (dateString.Length <= 7)
                {
                    dateString = "01/" + dateString;
                }
            }
            return Convert.ToDateTime(dateString.Substring(0, 10), new CultureInfo("en-GB"));
        }
        public static string GetFormatDateStringFromString(this string dateString)
        {

            string[] array = new string[3];
            if (!string.IsNullOrEmpty(dateString))
            {
                if (dateString.Contains("/"))
                {
                    array = dateString.Split('/');
                }
                else if (dateString.Contains("-"))
                {
                    array = dateString.Split('-');
                }
                else
                {
                    array = dateString.Split('.');
                }
            }
            var date = array[1] + "/" + array[0] + "/" + array[2];
            return date;
        }
        public static DateTime GetFormatDateTimeFromString(this string dateString)
        {
            if (dateString != null)
            {
                if (dateString.Length <= 7)
                {
                    dateString = "01/" + dateString;
                }
            }
            var date = Convert.ToDateTime(dateString.Substring(0, 10), new CultureInfo("en-GB"));

            var time = DateTime.Now.TimeOfDay;
            date = date.AddHours(time.Hours).AddMinutes(time.Minutes).AddSeconds(time.Seconds);

            return date;
        }

        public static string AgeToReadableString(this TimeSpan span)
        {
            var formatted = $"{(span.Duration().Days > 0 ? $"{span.Days:0} D, " : string.Empty)}" +
                               $"{(span.Duration().Hours > 0 ? $"{span.Hours:0} H, " : string.Empty)}" +
                               $"{(span.Duration().Minutes > 0 ? $"{span.Minutes:0} M, " : string.Empty)}" +
                               $"{(span.Duration().Seconds > 0 ? $"{span.Seconds:0} S" : string.Empty)}";

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "Just Now";

            return formatted;
        }
        public static string AgeToReadableString(this TimeSpan? span)
        {
            if (span == null) return "-";



            var formatted =
                               $"{(span.Value.Duration().Days > 0 ? $"{span.Value.Days:0} D, " : string.Empty)}" +
                               $"{(span.Value.Duration().Hours > 0 ? $"{span.Value.Hours:0} H, " : string.Empty)}" +
                               $"{(span.Value.Duration().Minutes > 0 ? $"{span.Value.Minutes:0} M, " : string.Empty)}" +
                               $"{(span.Value.Duration().Seconds > 0 ? $"{span.Value.Seconds:0} S" : string.Empty)}";

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "Just Now";

            return formatted;
        }

        public static string TwoDatesToReadableAge(DateTime? newerDate, DateTime? olderDate)
        {
            if (newerDate == null || olderDate == null)
                return "-";




            Age age = new Helper.Age();
            var data = age.CountNullable(olderDate, newerDate);


            var span = newerDate.Value - olderDate.Value;
            var formatted =
                               $"{(span.Duration().Days > 365 ? $"{data.Years:0} Y, " : string.Empty)}" +
                               $"{(span.Duration().Days > 30 ? $"{data.Months:0} M, " : string.Empty)}" +
                               $"{(span.Duration().Days > 0 ? $"{data.Days:0} D, " : string.Empty)}" +
                               $"{(span.Duration().Hours > 0 ? $"{span.Hours:0} H, " : string.Empty)}" +
                               $"{(span.Duration().Minutes > 0 ? $"{span.Minutes:0} M, " : string.Empty)}" +
                               $"{(span.Duration().Seconds > 0 ? $"{span.Seconds:0} S" : string.Empty)}";

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "Just Now";

            return formatted;
        }

        public static decimal Age(this DateTime date)
        {
            var nowDate = Convert.ToInt32(GetTodayString().GetFormattedDate(LongYeayIsoDateFormat));
            var birthDate = Convert.ToInt32(date.GetFormatedDate().GetFormattedDate(LongYeayIsoDateFormat));
            var age = (nowDate - birthDate).ToString();
            return age.Length > 4 ? Convert.ToDecimal(age.Remove(age.Length - 4)) : 0;
        }
        public static string DurationToReadableString(this TimeSpan span)
        {
            var formatted = $"{(span.Duration().Days > 0 ? $"{span.Days:0} D, " : string.Empty)}" +
                            $"{(span.Duration().Hours > 0 ? $"{span.Hours:0} H, " : string.Empty)}" +
                            $"{(span.Duration().Minutes > 0 ? $"{span.Minutes:0} M, " : string.Empty)}" +
                            $"{(span.Duration().Seconds > 0 ? $"{span.Seconds:0} S" : string.Empty)}";

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "Just Few Seconds";

            return formatted;
        }

        public static string GetFormatedMonthYear(this string value)
        {
            DateTime date;
            if (!DateTime.TryParseExact(value, "MMMM yyyy", null, DateTimeStyles.None, out date))
                return string.Empty;
            return date.ToString("MM/yyyy");
        }
        public static DateTime GetDateFromUnixTimeStamp(this double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static string GetFormatedTime(this DateTime? date)
        {
            if (date != null)
                return date.Value.ToString("hh:mm tt");
            else
                return "";
        }
        public static string GetFormatedDateTime(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy hh:mm tt");
        }
        public static string GetFormatedDateTime(this DateTime? date)
        {
            if (date != null)
                return date.Value.ToString("dd/MM/yyyy hh:mm tt");
            else
                return "";
        }
        public static DateTime GetDateFromString(this string dateTime)
        {
            if (dateTime == null)
            {
                return new DateTime(); ;
            }
            if (dateTime.Length <= 7)
            {
                dateTime = "01/" + dateTime;
            }
            var date = dateTime.Length == 7 ? dateTime.Remove(0, 1) : dateTime;
            var formats = GetFormats();
            DateTime retDate;
            var parsed = DateTime.TryParseExact(date, formats, null, DateTimeStyles.None, out retDate);

            var time = DateTime.Now.TimeOfDay;
            retDate = retDate.AddHours(time.Hours).AddMinutes(time.Minutes).AddSeconds(time.Seconds);

            return parsed ? retDate : DateTime.Now;
        }
        public static string GetFormattedDate(this decimal isoDate)
        {
            if (isoDate == 0 || isoDate.ToString().Length < 8)
                return "";

            var date = isoDate.ToString(CultureInfo.InvariantCulture).Insert(4, "/").Insert(7, "/");
            return date;
        }
        public static DateTime ConvertEQDateToDateTime(this string eqDate)
        {

            if (eqDate == "0") return DateTime.Now;
            long eqDateLong = long.Parse(eqDate) + 19000000;
            string eqDateStr = eqDateLong + "";
            int year = int.Parse(eqDateStr.Substring(0, 4));
            int month = int.Parse(eqDateStr.Substring(4, 2));
            int day = int.Parse(eqDateStr.Substring(6, 2));

            return new DateTime(year, month, day);


        }

        public static Nullable<DateTime> ConvertAnyEqDateToDateTime(this string EqDate)
        {
            if (EqDate == null)
                return null;
            else if (EqDate == "")
                return null;
            else if (EqDate == "0")
                return null;
            else if (EqDate == "19000000")
                return null;
            if (EqDate.Length == 7)
                EqDate = (long.Parse(EqDate) + 19000000).ToString();
            else if (EqDate.Length > 20)
                return DateTime.ParseExact(EqDate, "yyyy-MM-dd-HH.mm.ss.ffffff", CultureInfo.InvariantCulture);

            //"2018-06-05-12.33.24.442000"
            return DateTime.ParseExact(EqDate, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static string GetMonthName(this DateTime date)
        {
            return date.ToString("MMMM");
        }
    }

}
