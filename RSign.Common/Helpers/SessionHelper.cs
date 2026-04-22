using Microsoft.AspNetCore.Http;
using RSign.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSign.Common.Helpers
{
    public static class SessionHelper
    {
        public static object Get(string key)
        {
            //if (HttpContext.Current.Session != null)
            //    if (HttpContext.Current.Session[key] != null)
            //        return HttpContext.Current.Session[key];

            return null;
        }
        public static object Get(SessionKey key)
        {
            return Get(key.ToString());
        }

        public static void Set(string key, object data)
        {
           // HttpContext.Current.Session[key] = data;
        }

        public static void Set(SessionKey key, object data)
        {
            Set(key.ToString(), data);
        }

        public static void Remove(string key)
        {
           // HttpContext.Current.Session.Remove(key);
        }

        public static void Remove(SessionKey key)
        {
            Remove(key.ToString());
        }

        public static void RemoveAll()
        {
           // HttpContext.Current.Session.RemoveAll();
           // HttpContext.Current.Session.Abandon();
        }
    }
}
