using KGQT.Base;
using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Text.RegularExpressions;

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
                    case "user_send_text":
                        AutoCreatePackage(data);
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
                {
                    phone = phone.Substring(2);
                    phone = "0" + phone;
                }
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
                    if (!string.IsNullOrEmpty(data.info.name))
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
                    await ZaloCommon.UpdateInfoFollower(data.sender.id, data.info, user.Username);
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

        public async void AutoCreatePackage(WebHookReceive data)
        {
            string message = data.message.text;
            if (string.IsNullOrEmpty(message) || !message.ToLower().Contains("taokien"))
                return;

            var sender = data.sender;
            using (var db = new nhanshiphangContext())
            {
                var f = db.tbl_ZaloFollewers.FirstOrDefault(x => x.user_id == sender.id);
                if (f == null | string.IsNullOrEmpty(f?.Username))
                {
                    string sendText = $"Quý khách cần phải quan tâm OA và cung cấp thông tin cho tracking.nhanshiphang.vn mới có thể sử dụng tính năng này";
                    await ZaloCommon.SendMessage(sender.id, sendText);
                }
                var user = db.tbl_Accounts.FirstOrDefault(x => x.Username == f.Username);
                if (user == null)
                {
                    string sendText = $"Quý khách chưa có tài khoản tại hệ thống tracking.nhanshiphang.vn";
                    await ZaloCommon.SendMessage(sender.id, sendText);
                }

                string[] arrString = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (arrString.Length > 1)
                {

                    var p = new tbl_Package();
                    p.PackageCode = arrString[1];
                    if (arrString.Length > 2)
                        p.Note = arrString[2];
                    p.UID = user.ID;
                    p.Username = user.Username;
                    p.FullName = user.FullName;
                    p.Address = user.Address;
                    p.Phone = user.Phone;
                    p.Email = user.Email;
                    p.IsBrand = false;
                    p.IsAirPackage = false;
                    p.IsWoodPackage = false;
                    p.IsInsurance = false;
                    p.MovingMethod = 0;
                    p.Status = 1;
                    p.Weight = "0";
                    p.WeightExchange = "0";
                    p.Height = "0";
                    p.Width = "0";
                    p.Length = "0";
                    p.WeightReal = "0";
                    p.SearchBaiduTimes = 0;
                    p.TrackingShipping = 0;
                    p.OrderDate = DateTime.Now;
                    p.CreatedDate = DateTime.Now;
                    p.CreatedBy = user.Username;
                    db.Add(p);
                    if (db.SaveChanges() > 0)
                    {
                        string sendText = $"Quý khách đã tạo thành công kiện hàng {p.PackageCode}. \r\nTruy cập website:https://tracking.nhanshiphang.vn/package/details?id={p.ID} để xem chi tiết kiện hàng.";
                        await ZaloCommon.SendMessage(sender.id, sendText);
                    }
                }
            }
        }
    }
}
