using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ARCPSGUI.StaticGlobalClass
{
    public static class CommonMethods
    {
        /// <summary>
        /// Gets the 12:00:00 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteStart(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteEnd(this DateTime dateTime)
        {
            return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
        }
        public static List<string> GetStringListFromCSV(string filePath )
        {
            List<string> ls = null;
            using (var reader = new StreamReader(@filePath))
            {
                ls = new List<string>();
               
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    ls.Add(line);
                    
                }
            }
            return ls;

        }
        public static string GetNumberPartFromString(string str)
        {
            return Regex.Replace(str, "[^0-9]+", string.Empty); 
        }
       
    }
}
