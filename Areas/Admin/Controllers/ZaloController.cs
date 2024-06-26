﻿using KGQT.Base;
using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ZaloController : Controller
    {
        public IActionResult Index(string search, int page = 1)
        {
            var oData = ZaloBusiness.GetPage(search, page, 10);
            var lstData = oData[0] as List<tbl_ZaloFollewer>;
            ViewBag.numberPage = (int)oData[2];
            ViewBag.numberRecord = (int)oData[1];
            ViewBag.pageCurrent = page;
            return View(lstData);
        }

        public IActionResult Rules()
        { 
            return View();
        }

            public IActionResult Logs(string search, int page = 1)
        {
            var oData = ZaloBusiness.GetLogs(search, page, 10);
            var lstData = oData[0] as List<tbl_ZaloLog>;
            ViewBag.numberPage = (int)oData[2];
            ViewBag.numberRecord = (int)oData[1];
            ViewBag.pageCurrent = page;
            return View(lstData);
        }

        [HttpPost]
        public async Task<SendMessageResponse> SendRequestAsync(string uid)
        {
            var a = await ZaloCommon.RequestMoreInfoAsync(uid);
            return a;
        }
    }
}
