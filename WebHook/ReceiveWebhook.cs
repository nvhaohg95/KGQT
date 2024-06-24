using KGQT.Base;
using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace KGQT.WebHook
{
    public class ReceiveWebhook : IReceiveWebhook
    {
        public Task<HttpStatusCode> ReceiveData(string json)
        {
            var data = JsonConvert.DeserializeObject<WebHookReceive>(json);
            Log.Information($"ReceiveWebhook  {json}");
            Task task = new Task(() =>
            {
                switch (data.event_name)
                {
                    case "user_submit_info":
                        UpdateFollowerInfoAsync(data);
                        break;
                    case "unfollow":
                        RemoveFollower(data);
                        break;
                    case "follow":
                        AddFollower(data);
                        break;
                }

            });
            task.Start();
            return Task.FromResult(HttpStatusCode.OK);
        }

        public async Task UpdateFollowerInfoAsync(WebHookReceive data)
        {
            using (var db = new nhanshiphangContext())
            {
                var token = db.tbl_Zalos.FirstOrDefault();
                if (token?.accesstoken_expire < DateTime.Now.AddHours(-2))
                    token = await ZaloCommon.RefreshToken();

                string phone = data.info.phone;

                if (phone.StartsWith("84"))
                    phone = phone.Replace("84", "0");
                var user = db.tbl_Accounts.FirstOrDefault(x => x.Phone == phone);
                var f = db.tbl_ZaloFollewers.FirstOrDefault(x => x.user_id == data.sender.id);
                if (f == null)
                {
                    f = new tbl_ZaloFollewer();
                    f.RecID = Guid.NewGuid();
                    f.user_id = data.sender.id;
                    f.phone = phone;
                    f.display_name = data.info.name;
                    f.address = data.info.address;
                    f.SendRequest = true;
                    f.SendRequestTimes = 1;
                    f.SendRequestDate = DateTime.Now;
                    f.CreatedDate = DateTime.Now;
                    f.Status = 1;
                    if (user != null)
                    {
                        f.Username = user.Username;
                        f.Status = 2;
                    }
                    else
                    {
                        f.Note = "Không tìm thấy số điện thoại nào khớp với sdt khách đã cung cấp";
                    }
                    db.Add(f);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.info.name))
                        data.info.name = f.display_name;
                    f.phone = phone;
                    f.address = data.info.address;
                    if (user != null)
                    {
                        f.Username = user.Username;
                        f.Status = 2;
                    }
                    else
                    {
                        f.Note = "Không tìm thấy số điện thoại nào khớp với sdt khách đã cung cấp";
                    }
                    db.Update(f);
                }
                if (db.SaveChanges() > 0)
                {
                    await ZaloCommon.UpdateInfoFollower(data.sender.id, data.info);
                    Log.Information($"Cập nhật thông tin người dùng zalo {f.user_id} - {f.display_name} thành công");

                    if (user != null)
                    {
                        string mess = $"Cảm ơn {(string.IsNullOrEmpty(f.display_name) ? user.Username : f.display_name)} đã cung cấp thông tin thành công trên hệ thống Zalo OA của Nhận Ship Hàng. Hệ thống gửi tặng Quý khách 10.000 VND sẽ được nạp vào số dư ví trên ứng dụng TRAKUAIDI của Quý khách hàng trong vòng 24h. \r\nMọi thắc mắc xin vui lòng liên hệ CSKH của chúng tôi để được tư vấn giải đáp!";

                        if (AccountBusiness.UpdateInfoZalo(f.Username))
                            await ZaloCommon.SendMessage(f.user_id, mess);
                    }
                }
                else
                {
                    Log.Error($"Cập nhật thông tin người dùng zalo {f.user_id} - {f.display_name} thất bại");
                }
            }
        }

        public async void AddFollower(WebHookReceive data)
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    var f = db.tbl_ZaloFollewers.FirstOrDefault(x => x.user_id == data.follower.id);
                    if (f == null)
                    {
                        f = new tbl_ZaloFollewer();
                        f.RecID = Guid.NewGuid();
                        f.user_id = data.follower.id;
                        f.SendRequest = false;
                        f.SendRequestTimes = 0;
                        f.Status = 0;
                        f.CreatedDate = DateTime.Now;
                        db.Add(f);

                        if (db.SaveChanges() > 0)
                        {
                            await ZaloCommon.RequestMoreInfoAsync(data.follower.id);
                        }
                        else
                        {
                            var w = new tbl_ZaloWebHook();
                            w.RecID = Guid.NewGuid();
                            w.app_id = data.app_id;
                            w.event_name = data.event_name;
                            w.timestamp = data.timestamp;
                            w.sender = data.follower?.id;
                            w.status = 0;
                            w.CreatedDate = DateTime.Now;
                            db.Add(w);
                            await db.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Lỗi addFollower {ex.Message}");
                }
            }
        }

        public void RemoveFollower(WebHookReceive data)
        {
            using (var db = new nhanshiphangContext())
            {
                var f = db.tbl_ZaloFollewers.FirstOrDefault(x => x.user_id == data.follower.id);
                if (f != null)
                {
                    db.Remove(f);
                    if (db.SaveChanges() == 0)
                    {
                        var w = new tbl_ZaloWebHook();
                        w.RecID = Guid.NewGuid();
                        w.app_id = data.app_id;
                        w.event_name = data.event_name;
                        w.timestamp = data.timestamp;
                        w.sender = data.sender?.id;
                        w.recipient = data.recipient?.id;
                        w.phone = data.info?.phone;
                        w.address = data.info?.address;
                        w.name = data.info?.name;
                        w.status = 0;
                        w.CreatedDate = DateTime.Now;
                        db.Add(w);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
