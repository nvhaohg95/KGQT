﻿using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Drawing.Printing;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class WithDrawController : Controller
    {
        #region Nạp tiền
        public IActionResult Index(int ID,string userName, int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var oData = WithDrawBusiness.GetList(ID, 1, userName, status, fromDate, toDate, page, pageSize);
            var lstData = oData[0] as List<tbl_Withdraw>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
            ViewBag.id = ID;
            ViewBag.status = status;
            ViewBag.userName = userName;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            
            return View(lstData);
        }
        
        [HttpGet]
        public IActionResult Create(string user,int type)
        {
            ViewBag.user = user;
            ViewBag.type = type;
            return View();
        }
        #endregion

        #region Function
        [HttpPost]
        public IActionResult Insert(tbl_Withdraw model)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, model.Username);
            if (user != null)
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
            }
            model.Status = 2;
            model.Type = 1;
            model.CreatedBy = userLogin;
            model.CreatedDate = DateTime.Now;
            var s = BusinessBase.Add(model);
            if (s)
            {
                var u = BusinessBase.GetOne<tbl_Account>(x => x.ID == user.ID);
                string previousMoney = u.Wallet;
                u.Wallet += model.Amount;
                s = BusinessBase.Update(u);
                HistoryPayWallet.Insert(u.ID, u.Username, model.ID, model.Note, model.Amount, 1, 1, previousMoney, u.Wallet, userLogin);
            }
            return Ok(s);
        }



        [HttpPost]
        public object Arppoval(int id)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var result = TransactionBusiness.ApprovalRecharge(id, userLogin);
            return result;
        }
        [HttpGet]
        public IActionResult AutoComplete(string s)
        {
            var data = BusinessBase.GetList<tbl_Account>(x => x.Username.Contains(s)).ToList();
            return Json(data);
        }

        [HttpGet]
        public IActionResult AutoCompleteUser(string s)
        {
            var data = BusinessBase.GetList<tbl_Account>(x => x.RoleID != 1 && x.Username.Contains(s)).ToList();
            return Json(data);
        }
        #endregion

        #region Tạo lệnh nạp tiền
        [HttpPost]
        public DataReturnModel<bool> Create(tbl_Withdraw model)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var user = AccountBusiness.GetInfo(-1, model.Username);
            var result = new DataReturnModel<bool>();
            if (user != null && !string.IsNullOrEmpty(userLogin))
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
                model.Status = 2;
                model.CreatedBy = userLogin;
                model.CreatedDate = DateTime.Now;
                result = WithDrawBusiness.Insert(model, userLogin);
                result.Url = Url.Action("Index");
                return result;
            }
            result.IsError = true;
            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
            result.Data = false;
            return result;
        }
        #endregion

        #region Duyệt lệnh nạp tiền
        public DataReturnModel<tbl_Withdraw> Approval(int ID)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var result = WithDrawBusiness.Approval(ID, userLogin);
            return result;
        }
        #endregion

        #region Rút tiền
        [HttpGet]
        public ActionResult Refuse(int ID, string userName, int status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var oData = WithDrawBusiness.GetList(ID, 2,userName, status, fromDate, toDate, page, pageSize);
            var lstData = oData[0] as List<tbl_Withdraw>;
            var numberRecord = (int)oData[1];
            var numberPage = (int)oData[2];
            ViewBag.id = ID;
            ViewBag.userName = userName;
            ViewBag.status = status;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }
        #endregion

        #region Từ chối yêu cầu rút tiền
        [HttpPost]
        public DataReturnModel<bool> Refuse(int ID)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var result = WithDrawBusiness.Refuse(ID, userLogin);
            return result;
        }
        #endregion

        #region Đổi điểm Search
        public DataReturnModel<bool> BuySearches(string userName,int amount, bool free = false)
        {
            DataReturnModel<bool> result = new();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if(!string.IsNullOrEmpty(userLogin))
            {
                result = WithDrawBusiness.BuySearches(userName, amount, free, userLogin);
            }   
            else
            {
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
            }    
            return result;
        }
        #endregion
    }
}
