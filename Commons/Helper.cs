using System;
using System.Drawing;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Commons
{
    public static class Helper
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


        public static T AsObject<T>(this IFormCollection pairs) where T : class
        {
            string jsonString = $"{{{string.Join(",", pairs.Select(x => $"\"{x.Key}\" : \"{x.Value}\""))}}}";

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

public static string[] GetKeys(Type type, bool hasIDnKey = false)
		{
			if (type == null) return null;
			var lstInfos = type.GetProperties();
			var arrKeys = new List<string>();
			var arrIDs = new List<string>();
			foreach (var info in lstInfos)
			{
				if (info.GetCustomAttribute<KeyAttribute>() != null)
				{
					arrKeys.Add(info.Name);
				}
			};

			return arrIDs.Count > 0 ? arrIDs.ToArray() : arrKeys.ToArray()
		}
    }
}
