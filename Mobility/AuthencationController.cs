﻿using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace KGQT.Mobility
{
    [Route("api/[controller]")]
    public class AuthencationController
    {
        #region Authencation
        [HttpGet]
        [Route("login")]
        public object Login([FromQuery] string userName, [FromQuery] string passWord) 
        {
            var result = AccountBusiness.Login(userName, passWord);
            return result;
        }    

        [HttpPost]
        [Route("register")]
        public object RegisterAccount([FromBody] SignUpModel model)
        {
            if (!string.IsNullOrEmpty(model.Base64String))
            {
                byte[] bytes = Convert.FromBase64String(model.Base64String);
                MemoryStream stream = new MemoryStream(bytes);
                IFormFile file = new FormFile(stream, 0, bytes.Length, model.FileName, model.FileName);
                if (file != null)
                {
                    model.File = file;
                    model.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "uploads", "avatars");
                }
            }
            var data = AccountBusiness.Register(model);
            return data;
        }
        #endregion

        #region HomePage
        [HttpGet]
        [Route("dashboard")]
        public object[] GetDashBoard([FromQuery] string userName)
        {
            var lstpack = Packages.GetAllStatus(userName);
            var lstship = ShippingOrder.GetAllStatus(userName);
            //var result = AccountBusiness.Login(userName, passWord);
            return new object[] { lstpack, lstship };
        }
        #endregion

        #region PackagePage
        [HttpGet]
        [Route("package")]
        public object[] GetPackage([FromQuery] int status, [FromQuery] string ID, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, 
            [FromQuery] int pageNum, [FromQuery] int pageSize, [FromQuery] string userName)
        {
            var oData = Packages.GetPage(status, ID, fromDate, toDate, pageNum, pageSize, userName);
            return oData;
        }

        [HttpGet]
        [Route("checkstatus")]
        public object[] CheckStatusPackage([FromQuery] string ID,[FromQuery] string userName)
        {
            var data = Packages.GetStatusOrder(ID, userName);
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == ID && x.Status < 2);
            return new object[] { data, oPack };
        }

        [HttpGet]
        [Route("cancel")]
        public object[] CancelPackage([FromQuery] int ID, [FromQuery] string userName)
        {
            var data = Packages.Cancel(ID, userName);
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.ID == ID);
            return new object[] { data, oPack };
        }

        [HttpPost]
        [Route("createpackage")]
        public object[] CreatePackage([FromBody] tbl_Package model)
        {
            var data = Packages.CustomerAdd(model, model.Username);
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == model.PackageCode);
            return new object[] { data, oPack };
        }
		#endregion

		#region OrderPage
		[HttpGet]
		[Route("order")]
		public object[] GetOrder([FromQuery] int status, [FromQuery] string ID, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
			[FromQuery] int pageNum, [FromQuery] int pageSize, [FromQuery] string userName)
		{
			var oData = ShippingOrder.GetPage(status, ID, fromDate, toDate, pageNum, pageSize, userName);
			return oData;
		}

        [HttpGet]
        [Route("getlistpackage")]
        public object GetListPackage([FromQuery] int id)
        {
            var lstPacks = BusinessBase.GetList<tbl_Package>(x => x.TransID == id);
            return lstPacks;
        }

        public object Payment([FromQuery] int id, [FromQuery] string username)
        {
            if (id == 0) return false;
            var oOrder = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
            if (oOrder == null) return false;

            var oUser = BusinessBase.GetOne<tbl_Account>(x => x.Username == username);

            if (oUser == null) return false;

            bool check = oUser.Wallet > oOrder.TotalPrice;
            if (check)
            {
                oOrder.Status = 2;
                oOrder.ModifiedBy = username;
                oOrder.ModifiedDate = DateTime.Now;

                var s = BusinessBase.Update(oOrder);
                if (s)
                {
                    oUser = BusinessBase.GetOne<tbl_Account>(x => x.UserID == oUser.UserID);
                    var pay = oUser.Wallet - oOrder.TotalPrice;
                    oUser.Wallet = pay;
                    BusinessBase.Update(oUser);

                    #region Logs
                    BusinessBase.TrackLog(oUser.ID, oOrder.ID, "{0} đã thanh toán cho đơn hàng {1}", 1, oOrder.Username);
                    HistoryPayWallet.Insert(oUser.ID, oUser.Username, oOrder.ID, "", oOrder.TotalPrice.Value, 1, 1, pay.Value, username);
                    #endregion

                    var packs = BusinessBase.GetList<tbl_Package>(x => x.TransID == id);
                    foreach (var pack in packs)
                    {
                        pack.Status = 5;
                        pack.ModifiedBy = username;
                        pack.ModifiedDate = DateTime.Now;
                        BusinessBase.Update(pack);

                        BusinessBase.TrackLog(oUser.ID, pack.ID, "{0} đã thanh toán cho kiện {1}", 1, username);
                    }
                    return new { res = false };
                }
            }
            else
            {
                var pay = oOrder.TotalPrice - oUser.Wallet;
                return new { error = true, mssg = string.Format("{0:N0}đ", pay).Replace(",", ".") };
            }
            return false;
        }
        #endregion

    }
}
