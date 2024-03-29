﻿using KGQT.Business;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KGQT.Controllers
{
    public class AccountController : Controller
    {
        #region Info
        public IActionResult Info()
        {
            var userName = HttpContext.Session.GetString("user");
            var accInfo = AccountBusiness.GetInfo(-1,userName);
            ViewData["userName"] = userName;
            ViewData["lstRoles"] = AccountBusiness.GetListUserRole();
            return View(accInfo);
        }

        #endregion

        #region Update Info
        [HttpPost]
        public object UpdateInfo(string jsData, IFormFile file)
        {
            var data = JsonConvert.DeserializeObject<tbl_Account>(jsData);
            if (data == null)
            {
                return new DataReturnModel<tbl_Account>() { IsError = true, Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!" };
            }
            var reponse = AccountBusiness.UpdateInfo(data, file);
            return reponse;
        }

        #endregion
    }
}
