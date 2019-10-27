using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models.Constants
{
    public static class DateTimeFormats
    {
        public const string Unambiguous = "dd/MMM/yyyy";        //e.g. 17/Apr/2017
        public const string SortableShortSlash = "yyyy/MM/dd";  //e.g. 2017/11/29
        public const string SortableShortDash = "yyyy-MM-dd";   //e.g. 2017-11-29 (prefered)
        public const string Sortable = "s";                     //e.g. 2009-06-15T13:45:30
        public const string SortableUniversal = "u";            //e.g. 2009-06-15T13:45:30Z   
    }

    public static class ViewConstants
    {
        public const int DefaultResultNum = 10;
        public const int DefaultResultNumLong = 100;
    }
}
