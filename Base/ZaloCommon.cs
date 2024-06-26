﻿using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using ZaloDotNetSDK;
namespace KGQT.Base
{
    public static class ZaloCommon
    {
        static Timer refreshToken;
        public static void CallRefresh()
        {
            refreshToken = new Timer(RefreshToken, null, TimeSpan.FromHours(1), TimeSpan.FromHours(2));
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

            if (token == null)
            {
                Log.Error("Không có token");
                return null;
            }

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

        public static async Task GetListFollowerV2(int page = 1, int pageSize = 50)
        {
            var token = BusinessBase.GetFirst<tbl_Zalo>();
            if (token != null)
            {
                if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
                    token = await RefreshToken();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("access_token", token.access_token);

                    string url = "https://openapi.zalo.me/v3.0/oa/user/getlist";
                    var data = new
                    {
                        offset = (page - 1) * pageSize,
                        count = pageSize,
                        is_follower = true
                    };

                    var json = JsonConvert.SerializeObject(data);
                    var sData = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, sData);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var lstData = JsonConvert.DeserializeObject<Followers>(content);
                        if (lstData.message.ToLower() == "success")
                        {
                            int totalPage = lstData.data.total / pageSize;

                            if (totalPage * pageSize < lstData.data.total)
                                totalPage = totalPage + 1;

                            SaveListFollower(lstData.data.users);

                            if (page < totalPage)
                            {
                                page++;
                                await GetListFollowerV2(page, pageSize);
                                await Task.Delay(5000);
                            }
                        }
                    }
                }
            }
        }

        private static void SaveListFollower(List<Follower> data, ZaloClient client = null)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(
            async delegate (object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                // do some simple processing for 10 seconds
                using (var db = new nhanshiphangContext())
                {
                    if (client == null)
                    {
                        var token = db.tbl_Zalos.FirstOrDefault();
                        client = new ZaloClient(token.access_token);
                    }
                    data = data.Distinct().ToList();
                    foreach (var item in data)
                    {
                        var oa = db.tbl_ZaloFollewers.FirstOrDefault(x => x.user_id == item.user_id);
                        if (oa == null)
                        {
                            var f = new tbl_ZaloFollewer();
                            f.RecID = Guid.NewGuid();
                            f.user_id = item.user_id;
                            f.Status = 0;
                            f.SendRequest = false;
                            f.SendRequestTimes = 0;
                            var info = await GetInfoFlowerAsync(item.user_id);
                            if (info != null)
                                f.display_name = info.display_name;
                            db.Add(f);
                        }
                        else if (oa != null && string.IsNullOrEmpty(oa.display_name))
                        {
                            var info = await GetInfoFlowerAsync(item.user_id);
                            if (info != null)
                                oa.display_name = info.display_name;
                            db.Update(oa);
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

        public static async Task<bool> SendMessage(string uid, string message)
        {
            var token = BusinessBase.GetFirst<tbl_Zalo>();
            if (token != null)
            {
                if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
                    token = await RefreshToken();
                ZaloClient client = new ZaloClient(token.access_token);
                JObject result = client.sendTextMessageToUserIdV3(uid, message);
                var oData = JsonConvert.DeserializeObject<SendMessageResponse>(result.ToString());
                if (oData.error != 0)
                {
                    var log = new tbl_ZaloLog();
                    log.RecID = Guid.NewGuid();
                    log.action = 1;
                    log.error = oData.error;
                    log.message = oData.message;
                    log.user_id = uid;
                    log.context = message;
                    log.CreatedDate = DateTime.Now;
                    var info = BusinessBase.GetOne<tbl_ZaloFollewer>(x => x.user_id == uid);
                    if (info != null)
                        log.user_name = info.display_name;
                    return BusinessBase.Add(log);
                }
            }
            return false;
        }

        public static async Task SendMessageToAllV2(string message)
        {
            var token = BusinessBase.GetFirst<tbl_Zalo>();
            if (token != null)
            {
                if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
                    token = await RefreshToken();
                ZaloClient client = new ZaloClient(token.access_token);
                using (var db = new nhanshiphangContext())
                {
                    var lstData = db.tbl_ZaloFollewers.ToList();
                    if (lstData != null && lstData.Count > 0)
                    {
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.WorkerSupportsCancellation = true;
                        bw.DoWork += new DoWorkEventHandler(
                        delegate (object o, DoWorkEventArgs args)
                        {
                            BackgroundWorker b = o as BackgroundWorker;

                            foreach (var item in lstData)
                            {
                                Log.Information("send message to uid:" + item.user_id);
                                JObject result = client.sendTextMessageToUserIdV3(item.user_id, message);
                                var oData = JsonConvert.DeserializeObject<SendMessageResponse>(result.ToString());
                                if (oData.error != 0)
                                {
                                    var log = new tbl_ZaloLog();
                                    log.RecID = Guid.NewGuid();
                                    log.action = 1;
                                    log.error = oData.error;
                                    log.message = oData.message;
                                    log.user_id = item.user_id;
                                    log.user_name = item.display_name;
                                    log.context = message;
                                    log.CreatedDate = DateTime.Now;
                                    BusinessBase.Add(log);
                                }
                            }

                        });
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                           delegate (object o, RunWorkerCompletedEventArgs args)
                           {
                               bw.CancelAsync();
                           });
                        bw.RunWorkerAsync();
                    }
                }
            }
        }

        public static async Task<SendMessageResponse> RequestMoreInfoAsync(string uid)
        {
            var token = BusinessBase.GetFirst<tbl_Zalo>();
            if (token != null)
            {
                if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
                    token = await RefreshToken();
                ZaloClient client = new ZaloClient(token.access_token);
                JObject result = client.sendRequestUserProfileToUserIdV3(uid,
                    "Yêu cầu cung cấp thông tin",
                    "Vui lòng bấm vào hình để cung cấp cho chúng tôi thông tin của bạn!",
                    "https://tracking.nhanshiphang.vn/uploads/images/zalo-oa-request-moreinfo-14x9.png");
                Log.Information("Send request more info: " + result.ToString());
                var oData = JsonConvert.DeserializeObject<SendMessageResponse>(result.ToString());

                return oData;
            }
            return null;
        }

        public static async Task<UserInfo> GetInfoFlowerAsync(string uid)
        {
            using (var db = new nhanshiphangContext())
            {
                var token = db.tbl_Zalos.FirstOrDefault();
                if (token != null)
                {
                    if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
                        token = await RefreshToken();

                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            client.DefaultRequestHeaders.Add("access_token", token.access_token);
                            var response = await client.GetAsync("https://openapi.zalo.me/v3.0/oa/user/detail?data={\"user_id\":" + uid + "}");
                            if (response.IsSuccessStatusCode)
                            {
                                string content = await response.Content.ReadAsStringAsync();
                                var user = JsonConvert.DeserializeObject<RootUserInfo>(content);
                                if (user.message.ToLower() == "success")
                                    return user.data;
                                return null;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"GetInfoFlower {JsonConvert.SerializeObject(e)}");
                        }
                    }
                }

            };
            return null;
        }


        public static async Task<bool> UpdateInfoFollower(string uid, Info info, string alias)
        {
            using (var db = new nhanshiphangContext())
            {
                var token = db.tbl_Zalos.FirstOrDefault();
                if (token != null)
                {
                    if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
                        token = await RefreshToken();

                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            client.DefaultRequestHeaders.Add("access_token", token.access_token);

                            var data = new
                            {
                                user_id = uid,
                                shared_info = new
                                {
                                    name = info.name,
                                    phone = info.phone,
                                    address = info.address,
                                    city_id = info.district,
                                    district_id = info.ward,
                                },
                                user_alias = alias
                            };

                            var json = JsonConvert.SerializeObject(data);
                            var sData = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("https://openapi.zalo.me/v3.0/oa/user/update", sData);
                            if (response.IsSuccessStatusCode)
                            {
                                string content = await response.Content.ReadAsStringAsync();
                                var user = JsonConvert.DeserializeObject<SendMessageResponse>(content);
                                if (user.message.ToLower() == "success")
                                    return true;
                                return false;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"GetInfoFlower {JsonConvert.SerializeObject(e)}");
                        }
                    }
                }

            };
            return false;
        }
        #region Common
        public static void SendRequestAuto()
        {
            using (var db = new nhanshiphangContext())
            {
                var followers = db.tbl_ZaloFollewers.Where(x => string.IsNullOrEmpty(x.phone));
                foreach (var item in followers)
                {
                    RequestMoreInfoAsync(item.user_id);
                    item.SendRequest = true;
                    item.SendRequestTimes++;
                    item.SendRequestDate = DateTime.Now;
                    db.Update(item);
                }
                db.SaveChanges();
            }
        }
        #endregion

        #region Cũ
        //public static async Task SendMessageToAll(string message, int page = 1, int pageSize = 50)
        //{
        //    var token = BusinessBase.GetFirst<tbl_Zalo>();
        //    if (token != null)
        //    {
        //        if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
        //            token = await RefreshToken();
        //        ZaloClient client = new ZaloClient(token.access_token);
        //        JObject result = client.getListFollower((page - 1) * pageSize, pageSize);
        //        var oData = JsonConvert.DeserializeObject<Followers>(result.ToString());
        //        if (oData != null && oData?.message.ToLower() == "success")
        //        {
        //            int totalPage = oData.data.total / pageSize;

        //            if (totalPage * pageSize < oData.data.total)
        //                totalPage = totalPage + 1;

        //            SendMessageToUID(message, oData.data.users, client);

        //            if (page < totalPage)
        //            {
        //                page++;
        //                await GetListFollower(page, pageSize);
        //            }
        //        }
        //    }
        //}

        //public static void SendMessageToUID(string message, List<Follower> followers, ZaloClient client)
        //{
        //    BackgroundWorker bw = new BackgroundWorker();
        //    bw.WorkerSupportsCancellation = true;
        //    bw.DoWork += new DoWorkEventHandler(
        //    delegate (object o, DoWorkEventArgs args)
        //    {
        //        BackgroundWorker b = o as BackgroundWorker;

        //        foreach (var item in followers)
        //        {
        //            JObject result = client.sendTextMessageToUserIdV3(item.user_id, message);
        //            var oData = JsonConvert.DeserializeObject<SendMessageResponse>(result.ToString());
        //            if (oData.error != 0)
        //            {
        //                var log = new tbl_ZaloLog();
        //                log.RecID = Guid.NewGuid();
        //                log.action = 1;
        //                log.error = oData.error;
        //                log.message = oData.message;
        //                log.user_id = item.user_id;
        //                log.context = message;
        //                log.CreatedDate = DateTime.Now;
        //                var info = BusinessBase.GetOne<tbl_ZaloFollewer>(x => x.user_id == item.user_id);
        //                if (info != null)
        //                    log.user_name = info.display_name;
        //                BusinessBase.Add(log);
        //            }
        //        }

        //    });
        //    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
        //       delegate (object o, RunWorkerCompletedEventArgs args)
        //       {
        //           bw.CancelAsync();
        //       });
        //    bw.RunWorkerAsync();
        //}

        //public static async void GetListFollower(object obj)
        //{
        //    await GetListFollower();
        //}

        //public static async Task GetListFollower(int page = 1, int pageSize = 20)
        //{
        //    var token = BusinessBase.GetFirst<tbl_Zalo>();
        //    if (token != null)
        //    {
        //        if (token.accesstoken_expire < DateTime.Now.AddHours(-2))
        //            token = await RefreshToken();
        //        ZaloClient client = new ZaloClient(token.access_token);
        //        JObject result = client.getListFollower((page - 1) * pageSize, pageSize);
        //        var oData = JsonConvert.DeserializeObject<Followers>(result.ToString());
        //        if (oData.message.ToLower() == "success")
        //        {
        //            int totalPage = oData.data.total / pageSize;

        //            if (totalPage * pageSize < oData.data.total)
        //                totalPage = totalPage + 1;

        //            SaveListFollower(oData.data.users, client);

        //            if (page < totalPage)
        //            {
        //                page++;
        //                await GetListFollower(page, pageSize);
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
