using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchResultProcessor.Models
{
    public static class Calculator
    {
        public static string Concatenate<T>(this IEnumerable<T> source, string delimiter)
        {
            var sb = new StringBuilder();
            bool first = true;
            foreach (T t in source)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(delimiter);
                }
                sb.Append(t);
            }
            return sb.ToString();
        }
    }
}
