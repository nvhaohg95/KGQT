using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business;
using KGQT.Commons;
using KGQT.Controllers;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using System.Text.RegularExpressions;

namespace KGQT.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class CassoController : ControllerBase
    {
        [HttpPost]
        [Route("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var secureToken = Request.Headers["secure-token"];
            try
            {
                if (secureToken == "y19og1Q8q3nW7CzyuwOi")
                {
                    if (Request.Body.CanSeek)
                    {
                        // Reset the position to zero to read from the beginning.
                        Request.Body.Position = 0;
                    }

                    var input = await new StreamReader(Request.Body).ReadToEndAsync();
                    Log.Information($"Data nạp {input}");

                    CassoModel cassoData = JsonConvert.DeserializeObject<CassoModel>(input);
                    if (cassoData.error == 0 && cassoData.data.Count > 0)
                    {
                        using (var db = new nhanshiphangContext())
                        {
                            foreach (var item in cassoData.data)
                            {
                                var oCasso = new tbl_Casso();
                                oCasso.RecID = Guid.NewGuid();
                                oCasso.tid = item.tid;
                                oCasso.Time = item.when;
                                oCasso.amount = item.amount;
                                oCasso.description = item.description;
                                oCasso.subAccId = item.subAccId;
                                oCasso.OrderID = item.orderOfTheDay;
                                oCasso.cusum_balance = item.cusum_balance;
                                oCasso.CreatedDate = DateTime.Now;
                                db.tbl_Cassos.Add(oCasso);

                                string pattern = @"NAP *(\w*)";
                                RegexOptions options = RegexOptions.IgnoreCase;
                                Regex rg = new Regex(pattern);
                                string desCasso = "";
                                Match m = Regex.Match(item.description, pattern, options);
                                desCasso = m.Value;
                                Log.Information($"Cú pháp nạp {desCasso}");
                                if (!string.IsNullOrEmpty(desCasso))
                                {
                                    var checkw = db.tbl_Withdraws.FirstOrDefault(x => x.tid == item.id.ToString());
                                    if (checkw == null)
                                    {
                                        var sDec = desCasso.Split(' ');
                                        string key = sDec[0];
                                        if (key.ToUpper() == "NAP")
                                        {
                                            string username = sDec[1].ToLower();
                                            var u = db.tbl_Accounts.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());
                                            if (u != null)
                                            {
                                                DateTime currentdate = DateTime.Now;
                                                tbl_Withdraw wd = new tbl_Withdraw();
                                                wd.UID = u.ID;
                                                wd.Username = u.Username;
                                                wd.Fullname = u.FullName;
                                                wd.Amount = item.amount.ToString();
                                                wd.BankName = item.subAccId;
                                                wd.PaymentMethod = 2;
                                                wd.AccountNumber = "";
                                                wd.Note = desCasso;
                                                wd.Status = 2;
                                                wd.Type = 1;
                                                wd.DateSend = currentdate;
                                                wd.CreatedDate = currentdate;
                                                wd.CreatedBy = "admin";
                                                wd.tid = item.tid;
                                                db.Add(wd);
                                                if (db.SaveChanges() > 0)
                                                {
                                                    Log.Information($"tạo lệnh nạp thành công");
                                                    double wallet = u.Wallet.Double();
                                                    string w = Converted.StringCeiling(wallet + item.amount);
                                                    if (AccountBusiness.UpdateWallet(u.ID, w))
                                                    {
                                                        Log.Information($"Update ví thành công");

                                                        tbl_HistoryPayWallet pay = new tbl_HistoryPayWallet();
                                                        pay.UID = u.ID;
                                                        pay.Username = u.Username;
                                                        pay.OrderID = 0;
                                                        pay.HContent = "Nạp tiền qua banking";
                                                        pay.Type = 2;
                                                        pay.TradeType = 9;
                                                        pay.Amount = Converted.String2Money(item.amount.ToString());
                                                        pay.MoneyPrevious = Converted.String2Money(u.Wallet);
                                                        pay.MoneyLeft = Converted.String2Money(w);
                                                        pay.CreatedDate = DateTime.Now;
                                                        pay.CreatedBy = "admin";
                                                        pay.Status = 1;
                                                        pay.IsActive = 1;
                                                        db.Add(pay);
                                                        if(db.SaveChanges() > 0) 
                                                        Log.Information($"Tạo lịch sử giao dịch thành công");

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Lỗi webhook {JsonConvert.SerializeObject(ex)}");
            }
            return Ok(HttpStatusCode.OK);
        }
    }
}
