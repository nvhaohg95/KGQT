using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;
using System.Text;
using Fasterflect;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Vml;
using KGQT.Business.Base;
using KGQT.Models;

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

			return arrIDs.Count > 0 ? arrIDs.ToArray() : arrKeys.ToArray();
		}

		public static object GetValue(string name, object obj)
		{
			var type = obj.GetType();
			object result = null;
			PropertyInfo pInfo = type.GetProperty(name);
			if (pInfo != null)
				result = pInfo.GetValue(obj, null);
			return result;
		}
        public static async Task<object> SendFCMAsync(string body, string token, Dictionary<string, string> data,int userID)
        {
            try
            {
                var total = 0;
                List <tbl_Notification> query = BusinessBase.GetList<tbl_Notification>(x => x.ReceivedID == userID && x.Status == 0).ToList();
                if (query.Count > 0)
                {
                    total = query.Count();
                }
                if (FirebaseApp.DefaultInstance == null)
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "Firebase\\" + "trakuaidi-app-firebase-key.json";
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(path)
                    });
                }

                // This registration token comes from the client FCM SDKs.
                //var registrationToken = "dLz14KR5QdmuioC_LZ0y8j:APA91bHduPYeNfEOAFHpOEhUmFAQrhZVvrhpok95mJulgQzF6OFYY99HukLTCAxNQ9lGpxhcdD0BE8WMqDKuVP2ahi4Z8wZT81qk8-3juViDjAa35oVWu650R90nMgPyOKAXL3D6GN-0";
                // See documentation on defining a message payload.
                var message = new Message()
                {

                    Data = data,
                    Token = token,
                    Android = new AndroidConfig()
                    {
                        Notification = new AndroidNotification()
                        {
                            ChannelId = "Trakuaidi",
                            Priority = NotificationPriority.HIGH,
                            Title = "Trakuaidi xin thông báo!",
                            Body = body,
                            DefaultSound = true,
                            DefaultLightSettings = true,
                            NotificationCount = total,
                        },
						Priority = Priority.High
                    },
					Notification = new Notification()
					{
                        Title = "Trakuaidi xin thông báo!",
						Body = body
                    },
                    Apns = new ApnsConfig()
                    {
                        Aps = new Aps()
                        {
                            Badge = total,
							ContentAvailable = true,
                        }
                    }
                    /*Apns = new ApnsConfig()
					{
						Aps = {
							Badge = 10,
						}
					}*/
                };

                // Send a message to the device corresponding to the provided
                // registration token.
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                // Response is a message ID string.
                return response;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
