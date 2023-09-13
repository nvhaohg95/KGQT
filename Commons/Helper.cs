using System;
using System.Drawing;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Commons
{
    public class Helper
    {
        #region Encode string to base64
        public static string Base64Encode(string text)
        {
            var planTextBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(planTextBytes);
        }
        #endregion

        #region Decode string from base64
        public static string Base64Decode(string base64)
        {
            var planTextBytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(planTextBytes);
        }
        #endregion

    }
}
