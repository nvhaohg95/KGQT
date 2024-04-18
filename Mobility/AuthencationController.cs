using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;

namespace KGQT.Mobility
{
    [Route("api/[controller]")]
    public class AuthencationController
    {
        #region Authencation
        [HttpPost]
        [Route("login")]
        public object Login([FromBody] RequestModel model) 
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model)) {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            } 
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : "";
            string? passWord = dataRequest.ContainsKey("passWord") ? dataRequest["passWord"].ToString() : "";
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord)) {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            DataReturnModel<tbl_Account> result = AccountBusiness.Login(userName, passWord);
            if (!result.IsError)
            {
                var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == userName);
                if (user.IsActive == false)
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Người không tồn tại. Vui lòng thử lại!";
                    return oRequest;
                }
            }
            return result;
        }

        [HttpPost]
        [Route("register")]
        public object RegisterAccount([FromBody] SignUpModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (model == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
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
            var result = AccountBusiness.Register(model);
            return result;
        }

        [HttpPost]
        [Route("deleteaccount")]
        public object DeleteAccount([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : "";
            var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == userName);
            if (user == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Người dùng không còn tồn tại!";
                return oRequest;
            }
            user.IsActive = false;
            var update = BusinessBase.Update(user);
            if (!update)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            else
            {
                oRequest.IsError = false;
                oRequest.Message = "Xóa tài khoản thành công!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("changepassword")]
        public object ChangePassword([FromBody] ChangePassword model)
        {
            var oRequest = new DataReturnModel<object>();
            if (model == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            var result = AccountBusiness.ChangePassword(model);
            return result;
        }

        [HttpPost]
        [Route("getconfig")]
        public object GetConfig()
        {
            var result = BusinessBase.GetOne<tbl_Configuration>(x => x.Websitename == "Trakuaidi");
            return result;
        }
        #endregion

        #region HomePage

        [HttpPost]
        [Route("getuser")]
        public object[] GetUser([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : "";
            var result = BusinessBase.GetOne<tbl_Account>(x => x.Username == userName);
            if (result == null) {
                oRequest.IsError = true;
                oRequest.Message = "Người dùng không còn tồn tại!";
                return new object[] { true, oRequest };
            }
            if (!string.IsNullOrEmpty(result.IMG))
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "wwwroot\\" + result.IMG;
                using (Image image = Image.FromFile(path))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        result.IMG = base64String;
                    }
                }
            }
            return new object[] { false, result };
        }

        [HttpPost]
        [Route("dashboard")]
        public object[] GetDashBoard([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true,oRequest  };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true,oRequest };
            }
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : "";
            if (string.IsNullOrEmpty(userName))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true,oRequest};
            }
            var lstpack = PackagesBusiness.GetAllStatus(userName);
            var lstship = ShippingOrder.GetAllStatus(userName);
            return new object[] {false, lstpack, lstship };
        }
        #endregion

        #region PackagePage
        [HttpPost]
        [Route("package")]
        public object[] GetPackage([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            int status = dataRequest.ContainsKey("status") ? Int32.Parse(dataRequest["status"].ToString()) : 0;
            string? ID = dataRequest.ContainsKey("id") ? dataRequest["id"].ToString() : null;
            int pageNum = dataRequest.ContainsKey("pageNum") ? Int32.Parse(dataRequest["pageNum"].ToString()) : 0;
            int pageSize = dataRequest.ContainsKey("pageSize") ? Int32.Parse(dataRequest["pageSize"].ToString()) : 0;
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : null;
            if(string.IsNullOrEmpty(userName))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var oData = PackagesBusiness.GetPage(status, ID, null, null, pageNum, pageSize, userName);
            return new object[] { false, oData };
        }

        [HttpPost]
        [Route("checkstatus")]
        public object[] CheckStatusPackage([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            string? code = dataRequest.ContainsKey("id") ? dataRequest["id"].ToString() : null;
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : null;
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userName))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var data = PackagesBusiness.GetStatusOrder(code, userName);
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == code);
            return new object[] {false, data, oPack };
        }

        [HttpPost]
        [Route("cancel")]
        public object[] CancelPackage([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            int ID = dataRequest.ContainsKey("id") ? Int32.Parse(dataRequest["id"].ToString()) : 0;
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : null;
            if (string.IsNullOrEmpty(userName))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var data = PackagesBusiness.Cancel(ID, userName);
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.ID == ID);
            return new object[] { false, data, oPack };
        }

        [HttpPost]
        [Route("createpackage")]
        public object[] CreatePackage([FromBody] tbl_Package model)
        {
            var oRequest = new DataReturnModel<object>();
            if (model == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var data = PackagesBusiness.CustomerAdd(model, model.Username);
            var oPack = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == model.PackageCode);
            return new object[] {false, data, oPack };
        }
		#endregion

		#region OrderPage
		[HttpPost]
		[Route("order")]
		public object[] GetOrder([FromBody] RequestModel model)
		{
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            int status = dataRequest.ContainsKey("status") ? Int32.Parse(dataRequest["status"].ToString()) : 0;
            string? ID = dataRequest.ContainsKey("id") ? dataRequest["id"].ToString() : null;
            int pageNum = dataRequest.ContainsKey("pageNum") ? Int32.Parse(dataRequest["pageNum"].ToString()) : 0;
            int pageSize = dataRequest.ContainsKey("pageSize") ? Int32.Parse(dataRequest["pageSize"].ToString()) : 0;
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : null;
            if (string.IsNullOrEmpty(userName))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var oData = ShippingOrder.GetPage(status, ID, null, null, pageNum, pageSize, userName);
            return new object[] { false, oData };
        }

        [HttpPost]
        [Route("getlistpackage")]
        public object GetListPackage([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            string recID = dataRequest.ContainsKey("recID") ? dataRequest["recID"].ToString() : "";
            var lstPacks = BusinessBase.GetList<tbl_Package>(x => x.TransID == recID);
            return new object[] { false, lstPacks };
        }

        [HttpPost]
        [Route("payment")]
        public async Task<object> Payment([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            int id = dataRequest.ContainsKey("id") ? Int32.Parse(dataRequest["id"].ToString()) : 0;
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : null;
            string? token = dataRequest.ContainsKey("token") ? dataRequest["token"].ToString() : null;
            var result = ShippingOrder.Payment(id, userName);
            oRequest.IsError = result.IsError;
            oRequest.Message = result.Message;
            if (!oRequest.IsError)
            {
                var oOrder = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
                if (oOrder != null)
                {
                    oRequest.Data = oOrder;
                    if (!string.IsNullOrEmpty(token))
                    {
                        string body = $"Thanh toán cho đơn {oOrder?.ShippingOrderCode} thành công";
                        //await SendFCMAsync(body, token, null);
                    }
                }

            }
            return oRequest;
        }
        #endregion

        #region HistoryWallet
        [HttpPost]
        [Route("historywallet")]
        public object[] GetHistoryWallet([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            int status = dataRequest.ContainsKey("status") ? Int32.Parse(dataRequest["status"].ToString()) : 0;
            int ID = dataRequest.ContainsKey("id") ? Int32.Parse(dataRequest["id"].ToString()) : 0;
            int pageNum = dataRequest.ContainsKey("pageNum") ? Int32.Parse(dataRequest["pageNum"].ToString()) : 0;
            int pageSize = dataRequest.ContainsKey("pageSize") ? Int32.Parse(dataRequest["pageSize"].ToString()) : 0;
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : null;
            if (string.IsNullOrEmpty(userName))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var oData = HistoryPayWallet.GetPage(userName, ID, status, null, null, pageNum, pageSize);
            return new object[] { false, oData };
        }
        #endregion

        #region Notification
        [HttpPost]
        [Route("notification")]
        public object[] GetNotification([FromBody] RequestModel model)
        {
            var oRequest = new DataReturnModel<object>();
            if (!ValidateModelRequest(model))
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
            if (dataRequest == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
            int status = dataRequest.ContainsKey("status") ? Int32.Parse(dataRequest["status"].ToString()) : 0;
            string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : null;
            int pageNum = dataRequest.ContainsKey("pageNum") ? Int32.Parse(dataRequest["pageNum"].ToString()) : 0;
            int pageSize = dataRequest.ContainsKey("pageSize") ? Int32.Parse(dataRequest["pageSize"].ToString()) : 0;
            var user = AccountBusiness.GetInfo(-1, userName);
            var oData = NotificationBusiness.GetPage(user.ID, status, null, null, pageNum, pageSize);
            return new object[] { false, oData };
        }
        #endregion

        #region WithDraw
        [HttpPost]
        [Route("createwithdraw")]
        public object CreateWithDraw([FromBody] tbl_Withdraw model)
        {
            var oRequest = new DataReturnModel<object>();
            if (model == null)
            {
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            var user = AccountBusiness.GetInfo(-1, model.Username);
            if (user != null && model != null)
            {
                model.UID = user.ID;
                model.Fullname = user.FullName;
                model.Status = 1;
                model.CreatedBy = model.Username;
                model.CreatedDate = DateTime.Now;
                if (model.Type == 1)
                {
                    return WithDrawBusiness.Insert(model, model.Username);
                }
                else
                {
                    return WithDrawBusiness.Insert2(model, model.Username);
                }
            }
            else
            {
                oRequest.IsError = true;
                oRequest.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại sau!";
                oRequest.Data = false;
                return oRequest;
            }
        }
        #endregion

        #region Function
        private bool ValidateModelRequest(RequestModel model)
        {
            if (model == null) return false;
            if (string.IsNullOrEmpty(model.DataRequest)) return false;
            return true;
        }
        #endregion

    }
}
