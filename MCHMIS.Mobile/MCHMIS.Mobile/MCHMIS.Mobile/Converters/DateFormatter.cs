using System;

namespace MCHMIS.Mobile.Converters
{
    public static class DateFormatter
    {
        /// Converts Date time object to string formated as YYYY-MM-DD T HH:MM:SS
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToSQLiteDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString("s");
        }

        /// <summary>
        /// Converts DateTime string to string formated as yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dateTime">Date time string to convert</param>
        /// <returns></returns>
        public static string ToEnDateTimeString(string dateTime)
        {
            string format = "yyyy-MM-dd HH:mm:ss";
            return Convert.ToDateTime(dateTime).ToString(format);
        }
    }
}