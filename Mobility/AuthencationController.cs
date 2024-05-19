﻿using DocumentFormat.OpenXml.Wordprocessing;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Builder.Extensions;
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
        public async Task<object> Login([FromBody] RequestModel model) 
        {
            try
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
                string? passWord = dataRequest.ContainsKey("passWord") ? dataRequest["passWord"].ToString() : "";
                string? token = dataRequest.ContainsKey("token") ? dataRequest["token"]?.ToString() : null;
                string? deviceName = dataRequest.ContainsKey("deviceName") ? dataRequest["deviceName"]?.ToString() : "";
                string? deviceID = dataRequest.ContainsKey("deviceID") ? dataRequest["deviceID"]?.ToString() : "";
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord))
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                    return oRequest;
                }
                DataReturnModel<tbl_Account> result = AccountBusiness.Login(userName, passWord);
                if (!result.IsError)
                {
                    var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == userName);
                    if(user == null)
                    {
                        oRequest.IsError = true;
                        oRequest.Message = "Người dùng không tồn tại. Vui lòng thử lại!";
                        return oRequest;
                    }
                    if (user.IsActive == false)
                    {
                        oRequest.IsError = true;
                        oRequest.Message = "Người dùng không tồn tại. Vui lòng thử lại!";
                        return oRequest;
                    }
                    if (!string.IsNullOrEmpty(user.DeviceID) && deviceID != user.DeviceID)
                    {
                        if (!string.IsNullOrEmpty(user.TokenDevice))
                        {
                            string body = "Tài khoản của bạn đã được đăng nhập tại nơi khác!";
                            string message = "Tài khoản của bạn đã được đăng nhập ở nơi khác! Vui lòng đăng nhập lại.";
                            if (!string.IsNullOrEmpty(deviceName))
                                message += $"Phát hiện tài khoản đang được đăng nhập tại {deviceName}";
                            var data = new DataReturnModel<object>();
                            data.IsError = true;
                            data.Message = message;
                            data.TypeRequest = "loginerror";
                            Dictionary<string, string> datas = new Dictionary<string, string>();
                            datas.Add("data", JsonConvert.SerializeObject(data));
                            await Helper.SendFCMAsync(body, user.TokenDevice, datas);
                        }
                    }
                    user.TokenDevice = token;
                    user.DeviceName = deviceName;
                    user.DeviceID = deviceID;
                    BusinessBase.Update(user);
                }
                return result;
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("checklogin")]
        public object CheckLoginAccount([FromBody] RequestModel model)
        {
            try
            {
                var oRequest = new DataReturnModel<object>();
                if (!ValidateModelRequest(model))
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                    return oRequest;
                }
                var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
                string? userName = dataRequest.ContainsKey("userName") ? dataRequest["userName"].ToString() : "";
                string? deviceName = dataRequest.ContainsKey("deviceName") ? dataRequest["deviceName"]?.ToString() : "";
                string? deviceID = dataRequest.ContainsKey("deviceID") ? dataRequest["deviceID"]?.ToString() : "";
                string? token = dataRequest.ContainsKey("token") ? dataRequest["token"]?.ToString() : null;
                if (string.IsNullOrEmpty(deviceID))
                {
                    oRequest.IsError = true;
                    return oRequest;
                }
                else
                {
                    var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == userName);
                    if (user != null)
                    {
                        if (user.IsActive == false)
                        {
                            oRequest.IsError = true;
                            oRequest.Message = "Người dùng không tồn tại. Vui lòng thử lại!";
                            return oRequest;
                        }
                        if (!string.IsNullOrEmpty(user.DeviceID))
                        {
                            if (deviceID.Equals(user.DeviceID))
                            {
                                user.TokenDevice = token;
                                BusinessBase.Update(user);
                                oRequest.IsError = false;
                            }
                            else
                            {
                                string message = "Tài khoản của bạn đã được đăng nhập ở nơi khác! Vui lòng đăng nhập lại.";
                                if (!string.IsNullOrEmpty(user.DeviceName))
                                    message += $"Phát hiện tài khoản đang được đăng nhập tại {user.DeviceName}";
                                oRequest.Message = message;
                                oRequest.IsError = true;
                            }
                        }
                        else
                        {
                            user.DeviceID = deviceID;
                            user.DeviceName = deviceName;
                            user.TokenDevice = token;
                            BusinessBase.Update(user);
                            oRequest.IsError = false;
                        }
                    }
                    else
                    {
                        oRequest.IsError = true;
                        oRequest.Message = "Người dùng không tồn tại. Vui lòng thử lại!";
                    }
                    return oRequest;
                }
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("register")]
        public object RegisterAccount([FromBody] SignUpModel model)
        {
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("updateaccount")]
        public object UpdateInfoAccount([FromBody] SignUpModel model)
        {
            try
            {
                IFormFile file = null;
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
                    file = new FormFile(stream, 0, bytes.Length, model.FileName, model.FileName);
                    if (file != null)
                    {
                        model.File = file;
                        model.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "uploads", "avatars");
                    }
                }
                tbl_Account data = new tbl_Account();
                data.FullName = model.FullName;
                data.Gender = model.Gender;
                data.Email = model.Email;
                data.Phone = model.Phone;
                data.Address = model.Address;
                data.Username = model.Username;
                var result = AccountBusiness.Update(data, file,"");
                return result;
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("deleteaccount")]
        public object DeleteAccount([FromBody] RequestModel model)
        {
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("logoutaccount")]
        public object LogOutAccount([FromBody] RequestModel model)
        {
            try
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
                if (user != null)
                {
                    user.TokenDevice = "";
                    user.DeviceID = "";
                    user.DeviceName = "";
                    BusinessBase.Update(user);
                }
                oRequest.IsError = false;
                return oRequest;
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("changepassword")]
        public object ChangePassword([FromBody] ChangePassword model)
        {
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("getconfig")]
        public object GetConfig()
        {
            try
            {
                var result = BusinessBase.GetOne<tbl_Configuration>(x => x.Websitename == "Trakuaidi");
                return result;
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }
        #endregion

        #region HomePage

        [HttpPost]
        [Route("getuser")]
        public object[] GetUser([FromBody] RequestModel model)
        {
            try
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
                if (result == null)
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Người dùng không còn tồn tại!";
                    return new object[] { true, oRequest };
                }
                if (!string.IsNullOrEmpty(result.IMG))
                {
                    try
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "wwwroot\\" + result.IMG;
                        byte[] imageBytes = File.ReadAllBytes(path);
                        string base64String = Convert.ToBase64String(imageBytes);
                        result.IMG = base64String;
                    }
                    catch (Exception ex)
                    {
                        result.IMG = "";
                    }
                }
                return new object[] { false, result };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("dashboard")]
        public object[] GetDashBoard([FromBody] RequestModel model)
        {
            try
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
                if (string.IsNullOrEmpty(userName))
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                    return new object[] { true, oRequest };
                }
                var lstpack = PackagesBusiness.GetAllStatus(userName);
                var lstship = ShippingOrder.GetAllStatus(userName);
                return new object[] { false, lstpack, lstship };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }
        #endregion

        #region PackagePage
        [HttpPost]
        [Route("package")]
        public object[] GetPackage([FromBody] RequestModel model)
        {
            try
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
                var oData = PackagesBusiness.GetPage(status, ID, null, null, pageNum, pageSize, userName);
                return new object[] { false, oData };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("checkavailable")]
        public object[] CheckAvailableSearch([FromBody] RequestModel model)
        {
            try
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
                if (string.IsNullOrEmpty(code))
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                    return new object[] { true, oRequest };
                }
                var pack = PackagesBusiness.GetOne(code);
                if (pack == null)
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Không tìm thấy kiện!";
                    return new object[] { true, oRequest };
                }
                if (string.IsNullOrEmpty(pack.Username))
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Kiện chưa kê khác khách hàng!";
                    return new object[] { true, oRequest };
                }
                var user = AccountBusiness.GetOne(pack.Username);
                if (user == null)
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Người dùng không còn tồn tại!";
                    return new object[] { true, oRequest };
                }

                return new object[] { false, user.AvailableSearch};
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("checkstatus")]
        public object[] CheckStatusPackage([FromBody] RequestModel model)
        {
            try
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
                return new object[] { false, data };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("changeautoquery")]
        public object ChangeAutoQuery([FromBody] RequestModel model)
        {
            try
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
                int id = dataRequest.ContainsKey("id") ? Int32.Parse(dataRequest["id"].ToString()) : 0;
                bool check = dataRequest.ContainsKey("check") ? (bool)dataRequest["check"] : false;
                if (id == 0)
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                    return new object[] { true, oRequest };
                }
                var result = PackagesBusiness.ChangeAutoQuery(id, check);
                return new object[] { false, result };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }

        [HttpPost]
        [Route("cancel")]
        public object[] CancelPackage([FromBody] RequestModel model)
        {
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("restorepackage")]
        public object[] RestorePackage([FromBody] RequestModel model)
        {
            try
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
                var data = PackagesBusiness.Restore(ID, userName);
                var oPack = BusinessBase.GetOne<tbl_Package>(x => x.ID == ID);
                return new object[] { false, data, oPack };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("deletepackage")]
        public object[] DeletePackage([FromBody] RequestModel model)
        {
            try
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
                var data = PackagesBusiness.Delete(ID);
                return new object[] { false, data};
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("createpackage")]
        public object[] CreatePackage([FromBody] tbl_Package model)
        {
            try
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
                /*if (!data.IsError)
                {
                    var s = data.Message.Replace("</br>", ",");
                    data.Message = s.Substring(0, s.Length - 1);

                }*/
                return new object[] { false, data, oPack };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("updatenotepackage")]
        public object[] UpdateNotePackage([FromBody] tbl_Package model)
        {
            try
            {
                var oRequest = new DataReturnModel<object>();
                if (model == null)
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                    return new object[] { true, oRequest };
                }
                var data = PackagesBusiness.SaveNote(model.ID,model.Note, model.Username);
                var oPack = BusinessBase.GetOne<tbl_Package>(x => x.ID == model.ID);
                return new object[] { false, data, oPack };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("getonepackage")]
        public object GetOnePackage([FromBody] RequestModel model)
        {
            try
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

                var result = BusinessBase.GetOne<tbl_Package>(x => x.PackageCode == code && x.Username == userName);
                return new object[] { false, result };
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }
        #endregion

        #region OrderPage
        [HttpPost]
		[Route("order")]
		public object[] GetOrder([FromBody] RequestModel model)
		{
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("getlistpackage")]
        public object GetListPackage([FromBody] RequestModel model)
        {
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }

        [HttpPost]
        [Route("payment")]
        public async Task<object> Payment([FromBody] RequestModel model)
        {
            try
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
                var result = ShippingOrder.Payment(id, userName);
                oRequest.IsError = result.IsError;
                oRequest.Message = result.Message;
                if (!oRequest.IsError)
                {
                    var oOrder = BusinessBase.GetOne<tbl_ShippingOrder>(x => x.ID == id);
                    if (oOrder != null)
                    {
                        oRequest.Data = oOrder;
                    }

                }
                return oRequest;
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
            
        }
        #endregion

        #region HistoryWallet
        [HttpPost]
        [Route("historywallet")]
        public object[] GetHistoryWallet([FromBody] RequestModel model)
        {
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }
        #endregion

        #region Notification
        [HttpPost]
        [Route("notification")]
        public object[] GetNotification([FromBody] RequestModel model)
        {
            try
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return new object[] { true, oRequest };
            }
        }
        #endregion

        #region WithDraw
        [HttpPost]
        [Route("createwithdraw")]
        public async Task<object> CreateWithDraw([FromBody] RequestModel model)
        {
            try
            {
                var oRequest = new DataReturnModel<object>();
                if (model == null)
                {
                    oRequest.IsError = true;
                    oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                    return oRequest;
                }
                var dataRequest = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.DataRequest);
                string data = (dataRequest.ContainsKey("data") ? dataRequest["data"].ToString() : null);
                string? token = dataRequest.ContainsKey("token") ? dataRequest["token"]?.ToString() : null;
                tbl_Withdraw tbl = JsonConvert.DeserializeObject<tbl_Withdraw>(data);
                var user = AccountBusiness.GetInfo(-1, tbl.Username);
                if (user != null && tbl != null)
                {
                    tbl.UID = user.ID;
                    tbl.Fullname = user.FullName;
                    tbl.Status = 1;
                    tbl.CreatedBy = tbl.Username;
                    tbl.CreatedDate = DateTime.Now;
                    if (tbl.Type == 1)
                    {
                        var result = WithDrawBusiness.Insert(tbl, tbl.Username);
                        oRequest.IsError = result.IsError;
                        oRequest.Message = result.Message;
                        /*if (!oRequest.IsError)
                        {
                            if (!string.IsNullOrEmpty(token))
                            {
                                string body = $"Yêu cầu rút tiền của bạn đã được gửi đi";
                                await SendFCMAsync(body, token, null);
                            }
                        }*/
                        return oRequest;
                    }
                    else
                    {
                        var result = WithDrawBusiness.Insert2(tbl, tbl.Username);
                        oRequest.IsError = result.IsError;
                        oRequest.Message = result.Message;
                        /*if (!oRequest.IsError)
                        {
                            if (!string.IsNullOrEmpty(token))
                            {
                                string body = $"Yêu cầu nạp tiền của bạn đã được gửi đi";
                                await SendFCMAsync(body, token, null);
                            }
                        }*/
                        return oRequest;
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
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
                return oRequest;
            }
        }
        #endregion

        #region FeeWeight
        [HttpPost]
        [Route("getfeeweight")]
        public object GetFeeWeight()
        {
            try
            {
                var oRequest = new DataReturnModel<List<tbl_FeeWeight>>();
                List<tbl_FeeWeight> result = BusinessBase.Get<tbl_FeeWeight>().ToList();
                oRequest.IsError = false;
                oRequest.Data = result;
                return oRequest;
            }
            catch (Exception)
            {
                var oRequest = new DataReturnModel<object>();
                oRequest.IsError = true;
                oRequest.Message = "Đã có lỗi trong quá trình thực thi hệ thống. Vui lòng thử lại!";
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
        /*private async Task<object> SendFCMAsync(string body, string token, Dictionary<string, string> data)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "Firebase\\" + "trakuaidi-app-firebase-key.json";
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(path)
                    });
                }

                // This registration token comes from the client FCM SDKs.
                //var registrationToken = "dLz14KR5QdmuioC_LZ0y8j:APA91bHduPYeNfEOAFHpOEhUmFAQrhZVvrhpok95mJulgQzF6OFYY99HukLTCAxNQ9lGpxhcdD0BE8WMqDKuVP2ahi4Z8wZT81qk8-3juViDjAa35oVWu650R90nMgPyOKAXL3D6GN-0";
                // See documentation on defining a message payload.
                var message = new Message()
                {

                    Data = data,
                    Token = token,
                    Android = new AndroidConfig()
                    {
                        Notification = new AndroidNotification()
                        {
                            ChannelId = "Trakuaidi",
                            Priority = NotificationPriority.HIGH,
                            Title = "Trakuaidi xin thông báo",
                            Body = body,
                        },
                    }
                };

                // Send a message to the device corresponding to the provided
                // registration token.
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                // Response is a message ID string.
                return response;
            }
            catch (Exception)
            {
                return "";
            }
            
        }*/
        #endregion

    }
}
