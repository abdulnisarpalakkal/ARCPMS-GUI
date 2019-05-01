using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCPSGUI
{
    public static class CacheUI
    {
       static Dictionary<string, object> dicCached = new Dictionary<string, object>();

        public static void Add(string uiName, object ui)
        {
            if (dicCached.ContainsKey(uiName) == false)
            {
                dicCached.Add(uiName, ui);
            }
        }

        public static object Get(string uiName)
        {
            object ui = null;
            if (dicCached.ContainsKey(uiName) == true)
            {
               ui =  dicCached[uiName]; 
            }

            return ui;
        }

        public static bool IsValueCached(string uiName)
        {

            return dicCached.ContainsKey(uiName);
        }

    }
}
