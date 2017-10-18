using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WpfSampleProj2.Lib.Extensions
{
    public static class StringDateTimeExtensions
    {

        /// <summary>
        /// Convert global dates to corresponding DateTime string.
        /// 
        /// for example:  Tue, 02 Jul 2013 12:00:01 GMT  converts to  Locasl EDT 2013,07,2, 8, 00, 01
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTimeFromStr(this string obj)
        {
            var dtRtn = new DateTime();

            if (obj.IsEmpty()) return dtRtn;

            if (!DateTime.TryParse(obj, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtRtn))
                if (!DateTime.TryParseExact(obj, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dtRtn))
                    if (!DateTime.TryParseExact(ReplaceRfcDate(obj), dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite, out dtRtn))
                        DateTime.TryParse(obj, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dtRtn);

            // Guess best effot date
            if (dtRtn == DateTime.MinValue)
            {
                var dtLower = obj.ToLower();

                dtRtn = new DateTime(DateTime.Now.Year, GuessMonth(dtLower), GuessDay(dtLower), DateTime.Now.Hour, DateTime.Now.Minute, 0).ToUniversalTime();

                if (dtRtn > DateTime.Now.ToUniversalTime())
                    dtRtn = dtRtn.AddYears(-1);

                //DefaultLogger.IfIsDebug("Invalid Date: {0} - Guess New Date: {1}", obj, dtRtn);
            }


            // Guess best effot date
            if (dtRtn == DateTime.MinValue)
            {
                dtRtn = DateTime.Now.AddDays(-1).ToUniversalTime();

                //DefaultLogger.IfIsDebug("Invalid Date: {0} - Today Date: {1}", obj, dtRtn);
            }

            return dtRtn;
        }

        //"Tue, 10 Jun 2014 15:48:01 EST"
        private static readonly string[] dateFormats = new[]
        {
            "ddd, d MMM yyyy HH:mm:ss zzz",
            "ddd, dd MMM yyyy HH:mm:ss zzz",
            "dd MMM yyyy HH:mm:ss zzz",
            "d MMM yyyy HH:mm:ss zzz",
            "ddd, dd MMM yyyy hh:mm:ss zzz",
        };

        static int GuessMonth(string dt)
        {
            if (-1 != dt.IndexOf("jan")) return 1;
            if (-1 != dt.IndexOf("feb")) return 2;
            if (-1 != dt.IndexOf("mar")) return 3;
            if (-1 != dt.IndexOf("apr")) return 4;
            if (-1 != dt.IndexOf("may")) return 5;
            if (-1 != dt.IndexOf("jun")) return 6;
            if (-1 != dt.IndexOf("jul")) return 7;
            if (-1 != dt.IndexOf("aug")) return 8;
            if (-1 != dt.IndexOf("sep")) return 9;
            if (-1 != dt.IndexOf("oct")) return 10;
            if (-1 != dt.IndexOf("nov")) return 11;
            if (-1 != dt.IndexOf("dec")) return 12;

            return DateTime.Now.Month;
        }

        static int GuessDay(string dt)
        {
            var parts = dt.Split(' ');
            var day = parts
                .Select(x =>
                {
                    int number;
                    int.TryParse(x, out number);
                    return number;
                })
                .FirstOrDefault(x => x > 0 && x <= 27)
                ;

            return day == 0 ? DateTime.Now.Day : day;
        }


        static string ReplaceRfcDate(string dateStr)
        {
            // http://stackoverflow.com/questions/284775/how-do-i-parse-and-convert-datetimes-to-the-rfc-822-date-time-format

            if (dateStr.IndexOf(" EDT") != -1) return dateStr.Replace(" EDT", " -04:00");
            if (dateStr.IndexOf(" EST") != -1) return dateStr.Replace(" EST", " -05:00");
            if (dateStr.IndexOf(" GMT") != -1) return dateStr.Replace(" GMT", " -00:00");
            if (dateStr.IndexOf(" CST") != -1) return dateStr.Replace(" CST", " -06:00");
            if (dateStr.IndexOf(" CDT") != -1) return dateStr.Replace(" CDT", " -05:00");
            if (dateStr.IndexOf(" MST") != -1) return dateStr.Replace(" MST", " -07:00");
            if (dateStr.IndexOf(" MDT") != -1) return dateStr.Replace(" MDT", " -06:00");
            if (dateStr.IndexOf(" PST") != -1) return dateStr.Replace(" PST", " -08:00");
            if (dateStr.IndexOf(" PDT") != -1) return dateStr.Replace(" PDT", " -07:00");

            return dateStr;
        }

        public static string Chop(this string param, int len = 1024)
        {
            if (param.IsEmpty()) return string.Empty;

            return param.Length > len ? param.Substring(0, len) + "..." : param;
        }

        public static string Chop(this object param, int len = 1024)
        {
            if (param.IsNull()) return string.Empty;

            return param.ToString().Chop();
        }



        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset)
        {
            var currentDate = DateTime.UtcNow;
            int age = currentDate.Year - dateTimeOffset.Year;

            if (currentDate < dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }

    }
}
