using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Commons
{
    public class Helper
    {

        public static void SetSession(string key,string  value)
        {
            //HttpContext.Session.SetString(key, value);
        }

        public static void GetSession(string key)
        {

            //HttpContext.Session.Set("userLogin",user);
        }
    }
}
