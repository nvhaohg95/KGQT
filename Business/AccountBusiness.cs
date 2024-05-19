using DocumentFormat.OpenXml.Office.CustomUI;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Serilog;
using System.Text.RegularExpressions;
using ILogger = Serilog.ILogger;

namespace KGQT.Business
{
    public class AccountBusiness
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));
        public static DataReturnModel<tbl_Account> Login(string userName, string password)
        {
            var result = new DataReturnModel<tbl_Account>();
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập tên tài khoản!";
                    return result;
                }
                if (string.IsNullOrEmpty(password))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập mật khẩu!";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var data = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName.ToLower());
                    if (data != null)
                    {
                        if(data.IsActive == false)
                        {
                            result.IsError = true;
                            result.Message = "Tài khoản của bạn đã bị khóa!";
                            return result;
                        }
                        var isCorrect = password == PJUtils.Decrypt("userpass", data.Password);
                        if (isCorrect)
                        {
                            result.IsError = false;
                            result.Data = data;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Mật khẩu không chính xác!";
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Tên tài khoản không chính xác!";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi đăng nhập", ex.Message);
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        public static DataReturnModel<tbl_Account> Register(SignUpModel data)
        {
            var result = new DataReturnModel<tbl_Account>();
            try
            {
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var regexPhone = new Regex(@"^[0-9]{8,20}$");
                if (string.IsNullOrEmpty(data.FullName))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập họ tên khách hàng!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Phone))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập số điện thoại khách hàng!";
                    return result;
                }
                if (!regexPhone.IsMatch(data.Phone))
                {
                    result.IsError = true;
                    result.Message = "Số điện thoại không hợp lệ!";
                    return result;
                }
                if (!string.IsNullOrEmpty(data.Email) && !regexEmail.IsMatch(data.Email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ email không hợp lệ!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Address))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ không được bỏ trống!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Username))
                {
                    result.IsError = true;
                    result.Message = "Tài khoản không được bỏ trống!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Password))
                {
                    result.IsError = true;
                    result.Message = "Mật khẩu không được bỏ trống!";
                    return result;
                }
                
                using (var db = new nhanshiphangContext())
                {
                    var isUserName = db.tbl_Accounts.Any(x => x.Username == data.Username.ToLower());
                    if (isUserName)
                    {
                        result.IsError = true;
                        result.Message = "Tên tài khoản đã được sử dụng!";
                        return result;
                    }
                    if(!string.IsNullOrEmpty(data.Email))
                    {
                        var isEmail = db.tbl_Accounts.Any(x => x.Email == data.Email);
                        if(isEmail)
                        {
                            result.IsError = true;
                            result.Message = "Địa chỉ Email đã được sử dụng!";
                            return result;
                        }
                    }    
                    var acc = new tbl_Account()
                    {
                        Username = data.Username.ToLower(),
                        Password = PJUtils.Encrypt("userpass", data.Password),
                        FullName = data.FullName,
                        Gender = data.Gender,
                        Phone = data.Phone,
                        Email = data.Email,
                        Address = data.Address,
                        Wallet = "0",
                        AvailableSearch = 20,
                        Status = 2,
                        RoleID = 4, 
                        IsActive = true,
                        TokenDevice = data.TokenDevice,
                        DeviceID = data.DeviceID,
                        DeviceName = data.DeviceName,
                        CreatedBy = data.Username.ToLower(),
                        CreatedDate = DateTime.Now
                    };
                    db.Add(acc);
                    int kq = db.SaveChanges();
                    if (kq > 0)
                    {
                        if (data.File != null)
                        {
                            if (!Directory.Exists(data.Path)) 
                                Directory.CreateDirectory(data.Path);
                            var bytes = FileService.ResizeImage(data.File);
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(data.File.FileName);
                            string path = Path.Combine(data.Path, fileName);
                            File.WriteAllBytes(path, bytes);
                            acc.IMG = "\\" + Path.Combine("uploads", "avatars", fileName);
                            db.Update(acc);
                            db.SaveChanges();
                        }
                        result.IsError = false;
                        result.Url = "/Auth/Login";
                        result.Message = "Tạo tài khoản thành công!";
                        result.Data = acc;
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Đăng ký tài khoản không thành công. Vui lòng thử lại!";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi đăng ký tài khoản", ex.Message);
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        public static async Task<DataReturnModel<object>> SendMailForgetPassword(MailSettings mailSetting, string email)
        {
            DataReturnModel<object> result = new DataReturnModel<object>();
            try
            {
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (string.IsNullOrEmpty(email))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập địa chỉ email!";
                    return result;
                }
                if (!regexEmail.IsMatch(email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ email không hợp lệ!";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Email == email);
                    if (acc != null)
                    {
                        if(acc.IsActive == false)
                        {
                            result.IsError = true;
                            result.Message = "Tài khoản của bạn đã bị khóa!";
                            return result;
                        }    

                        acc.Token = Guid.NewGuid().ToString();
                        var name = acc.FullName;
                        string sToken = acc.Token + "|" + DateTime.Now.ToString();
                        string link = "https://localhost:44330/auth/forgotpassword?id=&tk=";
                        string id = Helper.Base64Encode(acc.Username);
                        string token = Helper.Base64Encode(sToken);
                        string subject = "YÊU CẦU CẤP PHÁT LẠI MẬT KHẨU";
                        string body = "<div>Xin chào <b>{0}</b>.</div><br><div>Yêu cầu cấp phát lại mật khẩu của bạn đã được xác nhận, vui lòng truy cập đường link bên dưới để tiếp tục thay đổi mật khẩu của bạn. <span><a href='{1}'>link</a></span></div><div></div><br><div>Cám ơn.</div>";
                        link = $"https://localhost:44330/auth/forgotpassword?id={id}&tk={token}";
                        body = string.Format(body, name, link);

                        var status = await MailService.SendMailAsync(mailSetting, email, subject, body);
                        if (status)
                        {
                            result.IsError = false;
                            result.Message = "Yêu cầu của bạn đã được gửi.Vui lòng kiểm tra Email!";
                            db.Update(acc);
                            db.SaveChanges();
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống gửi mail đang tạm dừng hoạt động. Vui lòng thử lại sau!";
                            return result;
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Địa chỉ Email không tồn tại!";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi gửi mail quên mật khẩu", ex.Message);
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        public static tbl_Account? GetByUsernameAndToken(string userName, string token)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_Accounts.FirstOrDefault(x => x.Username == userName.ToLower() && x.Token == token);
            }
        }

        public static DataReturnModel<bool> ForgotPassword(ForgotPassWord data)
        {
            DataReturnModel<bool> result = new();
            try
            {
                var regex = new Regex("^[a-zA-Z0-9 ]*$");
                if (string.IsNullOrEmpty(data.PassWord) || string.IsNullOrEmpty(data.ConfirmPassword))
                {
                    result.IsError = true;
                    result.Message = "Mật khẩu không được bỏ trống!";
                    return result;
                }
                if (!regex.IsMatch(data.PassWord) || PJUtils.CheckUnicode(data.PassWord))
                {
                    result.IsError = true;
                    result.Message = "Mật khẩu không chứa ký tự đặc biệt!";
                    return result;
                }
                if (!regex.IsMatch(data.ConfirmPassword) || PJUtils.CheckUnicode(data.ConfirmPassword))
                {
                    result.IsError = true;
                    result.Message = "Mật khẩu không chứa ký tự đặc biệt!";
                    return result;
                }
                if (data.ConfirmPassword != data.PassWord)
                {
                    result.IsError = true;
                    result.Message = "Mật khẩu không chính xác!";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName.ToLower());
                    if (acc != null)
                    {
                        acc.Password = PJUtils.Encrypt("userpass", data.PassWord);
                        acc.Token = null;
                        acc.ModifiedBy = acc.Username;
                        acc.ModifiedDate = DateTime.Now;
                        db.Update(acc);
                        int kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            result.IsError = false;
                            result.Data = true;
                            result.Message = "Cập nhật mật khẩu thành công!";
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công.Vui lòng thử lại!";
                        }
                        return result;
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy thông tin tài khoản!";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi đăng đổi mật khẩu", ex.Message);
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        public static DataReturnModel<object> ChangePassword(ChangePassword data)
        {
            var result = new DataReturnModel<object>();
            if (string.IsNullOrEmpty(data.UserName))
            {
                result.IsError = true;
                result.Message = "Tài khoản không được bỏ trống";
                return result;
            }
            if (string.IsNullOrEmpty(data.OldPassword))
            {
                result.IsError = true;
                result.Message = "Mật khẩu không được bỏ trống";
                return result;
            }
            if (string.IsNullOrEmpty(data.NewPassword))
            {
                result.IsError = true;
                result.Message = "Mật khẩu mới không được bỏ trống";
                return result;
            }
            if (string.IsNullOrEmpty(data.ConfirmPassword))
            {
                result.IsError = true;
                result.Message = "Mật khẩu mới không được bỏ trống";
                return result;
            }
            if (data.ConfirmPassword != data.NewPassword)
            {
                result.IsError = true;
                result.Message = "Mật khẩu không trùng khớp";
                return result;
            }
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(ac => ac.Username.ToLower() == data.UserName.ToLower());
                if (acc != null)
                {
                    string password = PJUtils.Decrypt("userpass", acc.Password);
                    if (password == data.OldPassword)
                    {
                        acc.Password = PJUtils.Encrypt("userpass", data.NewPassword);
                        acc.ModifiedBy = acc.Username;
                        acc.ModifiedDate = DateTime.Now;
                        db.Update(acc);
                        int kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            result.IsError = false;
                            result.Message = "Cập nhật thành công!";
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Cập nhật không thành công. Vui lòng thử lại!";
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Mật khẩu không chính xác. Vui lòng kiểm tra lại!";
                        return result;
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                }
                return result;
            }

        }

        public static object[] GetPage(string searchText = "", int page = 1, int pageSize = 10)
        {
            using (var db = new nhanshiphangContext())
            {
                var lst = new List<tbl_Account>();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_Accounts.Where(x => x.IsActive == true);
                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.Username.Contains(searchText) || x.FullName.Contains(searchText) || x.Email.Contains(searchText));
                total = query.Count();
                totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
                lst = query.Select(x => new tbl_Account
                {
                    ID = x.ID,
                    UserID = x.UserID,
                    Username = x.Username,
                    Phone = x.Phone,
                    Password = x.Password,
                    FullName = x.FullName,
                    Wallet = x.Wallet,
                    IMG = x.IMG
                }).ToList();
                return new object[] { lst, total, totalPage };
            }
        }

        public static tbl_Account? GetInfo(int? id, string? userName)
        {
            using (var db = new nhanshiphangContext())
            {
                tbl_Account? account = null;
                var query = db.tbl_Accounts.Where(x => x.IsActive == true);
                if (id != null && id > 0)
                    account = query.FirstOrDefault(x => x.ID == id);
                if (!string.IsNullOrEmpty(userName))
                    account = query.FirstOrDefault(x => x.Username == userName.ToLower());
                if (account != null)
                {
                    account.Password = PJUtils.Decrypt("userpass", account.Password);
                }
                return account;
            }
        }

        public static DataReturnModel<AccountInfo> Create(AccountInfo data, string createdBy)
        {
            var reponse = new DataReturnModel<AccountInfo>();
            try
            {
                if (string.IsNullOrEmpty(data.FullName))
                {
                    reponse.IsError = true;
                    reponse.Message = "Họ tên không được bỏ trống!";
                    return reponse;
                }
                if (string.IsNullOrEmpty(data.Phone))
                {
                    reponse.IsError = true;
                    reponse.Message = "Số điện thoại không được bỏ trống!";
                    return reponse;
                }
                if (string.IsNullOrEmpty(data.UserName))
                {
                    reponse.IsError = true;
                    reponse.Message = "Tài khoản không được bỏ trống!";
                    return reponse;
                }
                if (string.IsNullOrEmpty(data.Password))
                {
                    reponse.IsError = true;
                    reponse.Message = "Mật khẩu không được bỏ trống!";
                    return reponse;
                }
                if (string.IsNullOrEmpty(data.Address))
                {
                    reponse.IsError = true;
                    reponse.Message = "Địa chỉ không được bỏ trống!";
                    return reponse;
                }
                var regex = new Regex("^[a-zA-Z0-9 ]*$");
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var regexPhone = new Regex(@"^[0-9]{8,20}$");
                if (!string.IsNullOrEmpty(data.Email) && !regexEmail.IsMatch(data.Email))
                {
                    reponse.IsError = true;
                    reponse.Message = "Địa chỉ email không hợp lệ.";
                    return reponse;
                }
                if (!regexPhone.IsMatch(data.Phone))
                {
                    reponse.IsError = true;
                    reponse.Message = "Số điện thoại không hợp lệ.";
                    return reponse;
                }
                
                using (var db = new nhanshiphangContext())
                {
                    if (!string.IsNullOrEmpty(data.UserID))
                    {
                        var isUser = db.tbl_Accounts.Any(x => x.UserID == data.UserID);
                        if (isUser)
                        {
                            reponse.IsError = true;
                            reponse.Message = $"Mã định danh {data.UserID} đã tồn tại!";
                            return reponse;
                        }
                    }
                    var isUserName = db.tbl_Accounts.Any(x => x.Username == data.UserName.ToLower());
                    if (isUserName)
                    {
                        reponse.IsError = true;
                        reponse.Message = $"Tên tài khoản {data.UserName} đã tồn tại!";
                        return reponse;
                    }
                    var isEmail = db.tbl_Accounts.Any(x => x.Email == data.Email);
                    if (isEmail)
                    {
                        reponse.IsError = true;
                        reponse.Message = $"Địa chỉ email {data.Email} đã tồn tại!";
                        return reponse;
                    }
                    var acc = new tbl_Account()
                    {
                        FullName = data.FullName,
                        Gender = data.Gender,
                        Email = data.Email,
                        Phone = data.Phone,
                        Address = data.Address,
                        Wallet = "0",
                        Username = data.UserName.ToLower(),
                        Password = PJUtils.Encrypt("userpass", data.Password),
                        UserID = data.UserID,
                        RoleID = data.RoleID,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = createdBy
                    };
                    db.Add(acc);
                    var kq = db.SaveChanges();
                    if (kq > 0)
                    {
                        reponse.IsError = false;
                        reponse.Message = "Tạo tài khoản thành công!";
                        reponse.Data = data;
                        return reponse;
                    }
                    else
                    {
                        reponse.IsError = true;
                        reponse.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                        return reponse;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi tạo người dùng",ex.Message);
                reponse.IsError = true;
                reponse.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                return reponse;
            }
        }

        public static DataReturnModel<bool> Delete(int id)
        {
            var result = new DataReturnModel<bool>();
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
                if (acc != null)
                {
                    acc.IsActive = false;
                    acc.ModifiedDate = DateTime.Now;
                    db.Update(acc);
                    var kq = db.SaveChanges();
                    if (kq > 0)
                    {
                        result.IsError = false;
                        result.Message = "Xóa thành công!";
                        result.Data = true;
                        return result;
                    }
                }
                result.IsError = true;
                result.Message = "Xóa không thành công!";
                result.Data = false;
                return result;
            }
        }

        public static bool UpdateWallet(int id, string money)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
                if (acc != null)
                {
                    acc.Wallet = money;
                    db.Update(acc);
                    return db.SaveChanges() > 0;
                }
            }
            return false;
        }

        public static bool UpdateSearch(int id, int times)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
                if (acc != null)
                {
                    acc.AvailableSearch = times;
                    db.Update(acc);
                    return db.SaveChanges() > 0;
                }
            }
            return false;
        }

        public static List<UserRole> GetListUserRole()
        {
            List<UserRole> lst = new List<UserRole>();
            using (var db = new nhanshiphangContext())
            {
                lst = db.tbl_Roles.OrderBy(x => x.RoleID).Select(x => new UserRole()
                {
                    RoleID = x.RoleID,
                    RoleName = x.RoleName
                }).ToList();
            }
            return lst;
        }


        public static tbl_Account? GetOne(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_Accounts.FirstOrDefault(y => y.ID == id);
            }
        }

        public static tbl_Account? GetOne(string username)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_Accounts.FirstOrDefault(y => y.Username == username.ToLower());
            }
        }

        #region Update Info
        public static DataReturnModel<tbl_Account> Update(tbl_Account data, IFormFile fileImg, string createdBy)
        {
            var result = new DataReturnModel<tbl_Account>();
            try
            {
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var regexPhone = new Regex(@"^[0-9]{8,20}$");
                if (string.IsNullOrEmpty(data.FullName))
                {
                    result.IsError = true;
                    result.Message = "Họ tên khác hàng không được bỏ trống!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Address))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ không được bỏ trống!";
                    return result;
                }
                if (!string.IsNullOrEmpty(data.Email) && !regexEmail.IsMatch(data.Email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ Email không hợp lệ!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Phone))
                {
                    result.IsError = true;
                    result.Message = "Số điện thoại không được bỏ trống!";
                    return result;
                }
                if (!regexPhone.IsMatch(data.Phone))
                {
                    result.IsError = true;
                    result.Message = "Số điện thoại không hợp lệ!";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username.ToLower());
                    if (acc != null)
                    {
                        if (!string.IsNullOrEmpty(data.Email) && acc.Email != data.Email)
                        {
                            var isEmail = db.tbl_Accounts.Any(x => x.Email == data.Email);
                            if (isEmail)
                            {
                                result.IsError = true;
                                result.Message = string.Format("Địa chỉ Email {0} đã được sử dụng!", data.Email);
                                return result;
                            }
                            else
                                acc.Email = data.Email;
                        }
                        if (!string.IsNullOrEmpty(data.UserID) && acc.UserID != data.UserID)
                        {
                            var isUserID = db.tbl_Accounts.Any(x => x.Username == data.UserID);
                            if(isUserID)
                            {
                                result.IsError = true;
                                result.Message = string.Format("Mã định danh {0} đã được sử dụng!", data.UserID);
                                return result;
                            }
                            else
                                acc.UserID = data.UserID;
                        }
                        if(data.RoleID != null)
                        {
                            acc.RoleID = data.RoleID;
                        }
                        if (fileImg != null)
                        {
                            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "uploads", "avatars");
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var bytes = FileService.ResizeImage(fileImg);
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileImg.FileName);
                            path = Path.Combine(path, fileName);
                            File.WriteAllBytes(path, bytes);
                            acc.IMG = "\\" + Path.Combine("uploads", "avatars", fileName);
                        }
                        acc.FullName = data.FullName;
                        acc.Gender = data.Gender;
                        acc.Phone = data.Phone;
                        acc.Address = data.Address;
                        acc.ModifiedBy = createdBy;
                        acc.ModifiedDate = DateTime.Now;
                        db.Update(acc);
                        var kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            result.IsError = false;
                            result.Data = data;
                            result.Message = "Cập nhật thành công!";
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                            return result;
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy thông tin tài khoản!";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Lỗi cập nhật thông tin người dùng.", ex.Message);
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        #endregion


        public static tbl_Account? GetByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_Accounts.FirstOrDefault(x => x.Username == userName.ToLower() && x.IsActive == true);
            }
        }

    }
}
