using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.ComponentModel;
using System.Net.Http.Headers;
using ZaloDotNetSDK;
namespace KGQT.Base
{
    public static class ZaloCommon
    {
        static Timer timer;

        public static void CallRefresh()
        {
            timer = new Timer(RefreshToken, null, TimeSpan.FromHours(1), TimeSpan.FromHours(2));
        }
        public static async Task<bool> GetTokenAsync(string code)
        {
            string appID = Config.Zalo.AppID;
            string secret = Config.Zalo.Secret;
            string url = Config.Zalo.GetTokenUrl;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpContent requestContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("app_id", appID),
                    new KeyValuePair<string, string>("code", code)
                    });

                    client.DefaultRequestHeaders.Add("secret_key", secret);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    HttpResponseMessage response = await client.PostAsync(url, requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseBody) && !responseBody.Contains("error"))
                        {
                            var data = JsonConvert.DeserializeObject<tbl_Zalo>(responseBody);
                            if (!string.IsNullOrEmpty(data.access_token))
                            {
                                using (var db = new nhanshiphangContext())
                                {
                                    db.RemoveRange(db.tbl_Zalos);
                                    db.SaveChanges();
                                }
                                data.RecID = Guid.NewGuid();
                                data.freshtoken_expire = DateTime.Now.Date.AddMonths(3);
                                data.accesstoken_expire = DateTime.Now.AddSeconds(Int32.Parse(data.expires_in));
                                data.CreatedDate = DateTime.Now;
                                return BusinessBase.Add(data);
                            }
                        }
                        else
                        {
                            Log.Error("Không get dc token: " + responseBody);
                        }
                    }
                    else
                    {

                        Log.Error($"Request failed with status code {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Log.Error($"Request exception: {e.Message}");
                }
            }
            return false;
        }

        public static async void RefreshToken(object obj)
        {
            string appID = Config.Zalo.AppID;
            string secret = Config.Zalo.Secret;
            string url = Config.Zalo.GetTokenUrl;
            var token = BusinessBase.GetFirst<tbl_Zalo>();

            if (token == null) return;

            if (token.freshtoken_expire <= DateTime.Now) return;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpContent requestContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("app_id", appID),
                    new KeyValuePair<string, string>("refresh_token", token.refresh_token)
                    });

                    client.DefaultRequestHeaders.Add("secret_key", secret);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    HttpResponseMessage response = await client.PostAsync(url, requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseBody) && !responseBody.Contains("error"))
                        {
                            var data = JsonConvert.DeserializeObject<tbl_Zalo>(responseBody);
                            if (!string.IsNullOrEmpty(data.access_token))
                            {
                                token.access_token = data.access_token;
                                token.refresh_token = data.refresh_token;
                                token.accesstoken_expire = DateTime.Now.AddSeconds(Int32.Parse(data.expires_in));
                                BusinessBase.Update(token);
                            }
                        }
                        else
                        {
                            Log.Error("Không get dc token: " + responseBody);
                        }
                    }
                    else
                    {

                        Log.Error($"Request failed with status code {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Log.Error($"Request exception: {e.Message}");
                }
            }
        }

        public static async Task<tbl_Zalo> RefreshToken()
        {
            string appID = Config.Zalo.AppID;
            string secret = Config.Zalo.Secret;
            string url = Config.Zalo.GetTokenUrl;
            var token = BusinessBase.GetFirst<tbl_Zalo>();

            if (token == null) return null;

            if (token.freshtoken_expire <= DateTime.Now) return null;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpContent requestContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("app_id", appID),
                    new KeyValuePair<string, string>("refresh_token", token.refresh_token)
                    });

                    client.DefaultRequestHeaders.Add("secret_key", secret);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    HttpResponseMessage response = await client.PostAsync(url, requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseBody) && !responseBody.Contains("error"))
                        {
                            var data = JsonConvert.DeserializeObject<tbl_Zalo>(responseBody);
                            if (!string.IsNullOrEmpty(data.access_token))
                            {
                                token.access_token = data.access_token;
                                token.refresh_token = data.refresh_token;
                                token.accesstoken_expire = DateTime.Now.AddSeconds(Int32.Parse(data.expires_in));
                                BusinessBase.Update(token);
                                return token;
                            }
                        }
                        else
                        {
                            Log.Error("Không get dc token: " + responseBody);
                        }
                    }
                    else
                    {

                        Log.Error($"Request failed with status code {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Log.Error($"Request exception: {e.Message}");
                }
            }
            return null;
        }

        public static async Task GetListFollower(int page = 1, int pageSize = 20)
        {
            var token = BusinessBase.GetFirst<tbl_Zalo>();
            if (token != null)
            {
                if (token.accesstoken_expire < DateTime.Now)
                    token = await RefreshToken();
                ZaloClient client = new ZaloClient(token.access_token);
                JObject result = client.getListFollower((page - 1) * pageSize, pageSize);
                var oData = JsonConvert.DeserializeObject<Followers>(result.ToString());
                if (oData.message.ToLower() == "success")
                {
                    int totalPage = oData.data.total / 20;

                    if (totalPage * pageSize < oData.data.total)
                        totalPage = totalPage + 1;

                    BackGroundWorker(oData.data.followers, client);

                    if (page < totalPage)
                    {
                        page++;
                        await GetListFollower(page, pageSize);
                    }
                }
            }
        }

        private static void BackGroundWorker(List<Follower> data, ZaloClient client)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(
            delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                // do some simple processing for 10 seconds
                using (var db = new nhanshiphangContext())
                {
                    data = data.Distinct().ToList();
                    foreach (var item in data)
                    {
                        if (!db.tbl_ZaloFollewers.Any(x => x.user_id == item.user_id))
                        {
                            var f = new tbl_ZaloFollewer();
                            f.RecID = Guid.NewGuid();
                            f.user_id = item.user_id;

                            JObject oDetail = client.getProfileOfFollower(item.user_id);
                            var info = JsonConvert.DeserializeObject<RootUserInfo>(oDetail.ToString());
                            if (info.error == 0 || info.message.ToLower() == "success")
                                f.display_name = info.data.display_name;
                            db.Add(f);
                        }
                    }

                    db.SaveChanges();
                }
            });
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
               delegate (object o, RunWorkerCompletedEventArgs args)
               {
                   bw.CancelAsync();
               });
            bw.RunWorkerAsync();
        }

        public static async Task<bool> SendMessage()
        {
            var token = BusinessBase.GetFirst<tbl_Zalo>();
            if (token != null)
            {
                if (token.accesstoken_expire < DateTime.Now)
                    token = await RefreshToken();
                ZaloClient client = new ZaloClient(token.access_token);
                JObject result = client.sendTextMessageToUserId("7553678334610336355", "day la tin nhan\r\ntest \r\nthoi");
            }
            return false;
        }
    }
}
