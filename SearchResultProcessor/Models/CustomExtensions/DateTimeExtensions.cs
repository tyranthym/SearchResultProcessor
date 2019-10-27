using SearchResultProcessor.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models.CustomExtensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// a short-cut to use format yyyy-MM-dd, this can be used for sortable purpose. e.g. 2017-11-29
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringSortableShortDash(this DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormats.SortableShortDash);
        }
        /// <summary>
        /// a short-cut to use format yyyy-MM-dd, this can be used for sortable purpose. e.g. 2017-11-29
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToStringSortableShortDash(this DateTime? dateTime)
        {
            return dateTime?.ToString(DateTimeFormats.SortableShortDash);
        }
    }
}
