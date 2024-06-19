using KGQT.Base;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IO.Packaging;

namespace KGQT.Controllers
{
    public class PackageController : Controller
    {
        #region View
        public IActionResult Index(int status, string ID, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var oData = PackagesBusiness.GetPage(status, ID, fromDate, toDate, page, pageSize, userLogin);
            var lstPackage = oData[0];
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.status = status;
            ViewBag.ID = ID;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstPackage);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var model = PackagesBusiness.GetOne(id);
            if (model.Username.ToLower() != userLogin.ToLower())
            {
                model = null;
                return View(model);
            }

            var user = BusinessBase.GetOne<tbl_Account>(x => x.ID == model.UID);
            if (user != null)
            {
                model.Username = user.Username;
                model.Phone = user.Phone;
                model.Address = user.Address;
                model.Email = user.Email;
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult QueryOrderStatus(string code)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var data = PackagesBusiness.GetStatusOrder(code, userLogin);
            return View(data);
        }

        [HttpGet]
        public object CheckAvailableSearch(string code)
        {
            var pack = PackagesBusiness.GetOne(code);
            if (pack == null) return new { error = 1, msg = "Không tìm thấy kiện !" };
            if (string.IsNullOrEmpty(pack.Username))
                return new { error = 1, msg = "Kiện chưa kê khác khách hàng!" };
            var user = AccountBusiness.GetOne(pack.Username);
            if (user == null)
                return new { error = 2, msg = "Khách hàng này không có trong hệ thống, bạn có muốn tiếp tục!" };

            return new { error = 3, data = user.AvailableSearch, firsttime = !pack.AutoQuery, times = pack.SearchBaiduTimes };
        }
        #endregion

        #region CRUD
        [HttpPost]
        public DataReturnModel<int> Create(tbl_Package form)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var data = PackagesBusiness.CustomerAdd(form, userLogin);
            return data;
        }

        [HttpPost]
        public DataReturnModel<bool> Cancel(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var data = PackagesBusiness.Cancel(id, userLogin);
            return data;
        }

        [HttpPost]
        public DataReturnModel<bool> Restore(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var data = PackagesBusiness.Restore(id, userLogin);
            return data;
        }

        /// <summary>
        /// xoa đơn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public DataReturnModel<bool> Delete(int id)
        {
            var data = PackagesBusiness.Delete(id);
            return data;
        }

        /// <summary>
        /// ChangeAutoQuery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public DataReturnModel<bool> ChangeAutoQuery(int id, bool check)
        {
            var data = PackagesBusiness.ChangeAutoQuery(id, check);
            return data;
        }


        /// <summary>
        /// Thêm ghi chú
        /// </summary>
        /// <param name="id"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost]
        public DataReturnModel<bool> SaveNote(int id, string note)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var dt = PackagesBusiness.SaveNote(id, note, userLogin);
            return dt;
        }
        #endregion
    }
}
