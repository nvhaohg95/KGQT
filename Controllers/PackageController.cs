using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class PackageController : Controller
    {
        #region View
        public IActionResult Index(int status, string ID, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 2)
        {
            var username = HttpContext.Session.GetString("user");
            var oData = Packages.GetPage(status, ID, fromDate, toDate, page, pageSize, username);
            var lstPackage = oData[0];
            int totalRecord = (int)oData[1];
            int totalPage = (int)oData[2];
            ViewData["status"] = status;
            ViewData["page"] = page;
            ViewData["totalRecord"] = totalRecord;
            ViewData["totalPage"] = totalPage;
            ViewData["ID"] = ID;
            ViewData["fromDate"] = fromDate;
            ViewData["toDate"] = toDate;
            return View(lstPackage);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var model = BusinessBase.GetOne<tbl_Package>(x => x.ID == id);
            return View(model);
        }

        [HttpGet]
        public IActionResult QueryOrderStatus(string code)
        {
            tmpChinaOrderStatus data = new tmpChinaOrderStatus();
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == code && x.Status < 2);
            if (oPack != null)
            {
                var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == oPack.UID);

                if (oPack.SearchBaiduTimes == null)
                {
                    oPack.SearchBaiduTimes = 0;
                    var wallet = user.Wallet - 500;
                    BusinessBase.Update(user);

                    #region Logs
                    HistoryPayWallet.Insert(user.ID, user.Username, oPack.ID, "", 500, 1, 1, wallet.Value, HttpContext.Session.GetString("user"));
                    #endregion
                }
                if (oPack.SearchBaiduTimes > 5)
                {
                    data.success = false;
                    data.reason = "Bạn đã vượt quá số lần tìm kiếm cho phép";
                    return View(data);
                }
                using (HttpClient client = new HttpClient())
                {
                    string url = string.Format(Config.Settings.ApiUrl, Config.Settings.ApiKey, code);
                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string sData = response.Content.ReadAsStringAsync().Result;
                        data = JsonConvert.DeserializeObject<tmpChinaOrderStatus>(sData);
                    }
                    else
                    {
                        data.success = false;
                        data.reason = "Không thể kết nối đến server!";
                    }
                }

                oPack.SearchBaiduTimes++;
                BusinessBase.Update(oPack);
            }
            else
            {
                data.reason = "Đơn hàng chưa được tạo trên hệ thống tracking.nhanshiphang.vn";
            }
            data.nu = code;
            return View(data);
        }
        #endregion

        #region CRUD
        [HttpPost]
        public bool Create(tbl_Package form)
        {
            var userLogin = HttpContext.Session.GetString("user");
            var user = AccountBusiness.GetInfo(-1, userLogin);
            form.PackageCode = form.PackageCode.Trim().Replace("\'", "").Replace(" ", "");
            form.Status = 0;
            form.UID = user.ID;
            form.Username = user.Username;
            form.FullName = user.FirstName + " " + user.LastName;
            form.Phone = user.Phone;
            form.Email = user.Email;
            form.Address = user.Address;
            form.CreatedDate = DateTime.Now;
            form.CreatedBy = user.Username;

            if (form.IsInsurance.HasValue && form.IsInsurance == true)
            {
                form.IsInsurancePrice = form.DeclarePrice * 0.05;
            }

            var s = BusinessBase.Add(form);
            if (s)
            {
                BusinessBase.TrackLog(user.ID, form.ID, "{0} đã tạo kiện", 0, user.Username);
            }
            return s;
        }

        [HttpPost]
        public bool Test()
        {
            return true;
        }
        #endregion

        #region Function
        [HttpGet]
        public bool CheckPackage(string package)
        {
            return Packages.CheckExist(package);
        }
        #endregion
    }
}
