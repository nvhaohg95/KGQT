﻿using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]

    public class AccountController : Controller 
    {

        #region Index
        [HttpGet]
        public IActionResult Index(string searchText, int page = 1, int pageSize = 10)
        {
            var oData = AccountBusiness.GetPage(searchText, page, pageSize);
            var lstData = oData[0] as List<tbl_Account>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.searchText = searchText;
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            ViewBag.lstRoles = AccountBusiness.GetListUserRole();
            return View(lstData);
        }
        #endregion

        #region Detail
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var user = AccountBusiness.GetInfo(id,"");
            ViewData["ID"] = id;
            ViewBag.lstRoles = AccountBusiness.GetListUserRole();
            return View(user);
        }

        #endregion

        #region Create
        [HttpPost]
        public JsonResult Create(AccountInfo data)
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var reponse = AccountBusiness.Create(data, userLogin);
            return Json(reponse);
        }

        #endregion

        #region Update
        [HttpPost]
        public object Update(SignUpModel data)
        {
            DataReturnModel<tbl_Account> result = new();
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            if(data != null && !string.IsNullOrEmpty(userLogin))
            {
                result = AccountBusiness.Update(data, data.File, userLogin);
            }
            else
            {
                result.IsError = true;
                result.Message = "Cập nhật không thành công. Vui lòng thử lại!";
            }
            return result;
        }
        #endregion

        #region Delete
        [HttpPost]
        public DataReturnModel<bool> Delete(int id)
        {
            var result = AccountBusiness.Delete(id);
            return result;
        }

        #endregion

        #region Get Infor
        [HttpGet]
        public IActionResult Info()
        {
            var cookieService = new CookieService(HttpContext);
            var tkck = cookieService.Get("tkck");
            var userModel = JsonConvert.DeserializeObject<UserModel>(tkck);
            string userLogin = userModel != null ? userModel.UserName : "";
            var accInfo = AccountBusiness.GetInfo(-1, userLogin);
            ViewData["userName"] = userLogin;
            return View(accInfo);
        }

        #endregion
    }
}
