using System;
using System.Collections.Generic;
using System.Text;

namespace WpfSampleProj2.Lib.Extensions
{
    public static class NumeircExtensions
    {
        /// <summary>
        /// Convert string to int
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>Nullable int, or NULL if not a valid string</returns>
        public static int ToInt(this string value)
        {
            var converted = 0d;

            if (!double.TryParse(value, out converted))
                return 0;

            return (int)Math.Round(converted);
        }

        /// <summary>
        /// Convert string to nullable int
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>Nullable int, or NULL if not a valid string</returns>
        public static int? ToIntNullable(this string value)
        {
            var converted = 0d;

            if (!double.TryParse(value, out converted))
                return null;

            return (int)Math.Round(converted);
        }

        /// <summary>
        /// Convert string to nullable double
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>Nullable double, or NULL if not a valid string</returns>
        public static double? ToDouble(this string value)
        {
            var converted = double.MinValue;

            if (!double.TryParse(value, out converted))
                return null;

            return converted;
        }
    }
}
