using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PreviewDay.Extensions
{
    public static class MyExtensions
    {
        public static string FormatStringToShortDateTimeString(this string s)
        {
            if(DateTime.TryParse(s, out DateTime startDateTime))
            {
                return $"{startDateTime.ToShortDateString()} {startDateTime.ToShortTimeString()}";
            }
            else
            {
                return s;
            }

        }
    }
}