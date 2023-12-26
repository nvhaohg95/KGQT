using KGQT.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static KGQT.HomeController;

namespace KGQT.Business
{
    public class AccountBusiness
    {
        #region Login
        public static DataReturnModel<tbl_Account> Login(string userName, string password)
        {
            var dtReturn = new DataReturnModel<tbl_Account>();
            if (string.IsNullOrEmpty(userName))
            {
                dtReturn.IsError = true;
                dtReturn.Message = "Vui lòng nhập tên tài khoản!";
                return dtReturn;
            }
            if (string.IsNullOrEmpty(password))
            {
                dtReturn.IsError = true;
                dtReturn.Message = "Vui lòng nhập mật khẩu!";
                return dtReturn;
            }
            try
            {
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(a => a.Username == userName);     
                    if (acc != null)
                    {
                        var isCorrect = password == PJUtils.Decrypt("userpass", acc.Password);
                        if (isCorrect)
                        {
                            dtReturn.IsError = false;
                            dtReturn.Data = acc;
                            return dtReturn;
                        }
                        else
                        {
                            dtReturn.IsError = true;
                            dtReturn.Message = Messages.PasswordNotCorrect;
                            return dtReturn;
                        }

                    }
                    else
                    {
                        dtReturn.IsError = true;
                        dtReturn.Message = Messages.UsernameNotCorrect;
                        return dtReturn;
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Info("account base", JsonConvert.SerializeObject(ex));
                return dtReturn;
            }
        }

        #endregion

        #region Register 
        public static DataReturnModel<tbl_Account> Register(SignUpModel data)
        {
            var result = new DataReturnModel<tbl_Account>();
            try
            {
                if(data == null)
                {
                    result.IsError = true;
                    result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                    return result;
                }
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var regexPhone = new Regex(@"^[0-9]{8,20}$");
                if(string.IsNullOrEmpty(data.FullName))
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
                    result.Type = 1;
                    result.Message = "Số điện thoại không hợp lệ!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Email))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập Email khách hàng!";
                    return result;
                }
                if (!regexEmail.IsMatch(data.Email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ email không hợp lệ!";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Address))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập địa chỉ khách hàng!";
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
                    var isUserName = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username);
                    if (isUserName != null)
                    {
                        result.IsError = true;
                        result.Message = "Tên tài khoản đã được sử dụng.";
                        return result;
                    }
                    var acc = new tbl_Account()
                    {
                        Username = data.Username,
                        Password = PJUtils.Encrypt("userpass", data.Password),
                        FullName = data.FullName,
                        Gender = data.Gender,
                        Phone = data.Phone,
                        Email = data.Email,
                        Address = data.Address,
                        Wallet = 0,
                        Status = 2,
                        RoleID = 1,
                        CreatedDate = DateTime.Now,
                        CreatedBy = data.Username
                    };
                    db.Add(acc);
                    int kq = db.SaveChanges();
                    if (kq > 0)
                    {
                        if (data.File != null)
                        {
                            var bytes = FileService.ResizeImage(data.File);
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(data.File.FileName);
                            string path = Path.Combine(data.Path, fileName);
                            File.WriteAllBytes(path, bytes);
                            data.IMG = "/uploads/avatars/" + fileName;
                        }
                        result.IsError = false;
                        result.Type = 2;
                        result.Message = "Tạo tài khoản thành công!";
                        result.Data = acc;
                        return result;
                    }
                    else
                    {
                        result.IsError = true;
                        result.Type = 2;
                        result.Message = "Tạo tài khoản không thành công.";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Type = 2;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                return result;
            }
        }

        #endregion

        #region Send Mail Forget Password
        public static async Task<DataReturnModel<object>> SendMailForgetPassword(MailSettings mailSetting, string email)
        {
            DataReturnModel<object> result = new DataReturnModel<object>();
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    result.IsError = true;
                    result.Message = "Vui lòng nhập địa chỉ email.";
                    return result;
                }
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (!regexEmail.IsMatch(email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ email không hợp lệ.";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Email == email);
                    if (acc != null)
                    {
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

                        //send mail
                        var status = await MailService.SendMailAsync(mailSetting, email, subject, body);
                        if (status)
                        {
                            result.IsError = false;
                            result.Message = "Yêu cầu của bạn đã được gửi đi.Vui lòng kiểm tra email!";
                            db.Update(acc);
                            db.SaveChanges();
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
                        result.Message = "Địa chỉ Email chưa đăng ký. Vui lòng kiểm tra lại!";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại!";
                return result;
            }
        }

        #endregion

        #region Get Username & Token
        public static tbl_Account? GetByUsernameAndToken(string userName, string token)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_Accounts.FirstOrDefault(x => x.Username == userName && x.Token == token);
            }
        }

        #endregion

        #region Forgot Password
        public static DataReturnModel<object> ForgotPassword(ForgotPassWord data)
        {
            DataReturnModel<object> result = new DataReturnModel<object>();
            var regex = new Regex("^[a-zA-Z0-9 ]*$");
            if (string.IsNullOrEmpty(data.PassWord))
            {
                result.IsError = true;
                result.Type = 1;
                result.Message = "Mật khẩu không được bỏ trống";
                return result;
            }
            if (string.IsNullOrEmpty(data.ConfirmPassword))
            {
                result.IsError = true;
                result.Type = 1;
                result.Message = "Mật khẩu không được bỏ trống";
                return result;
            }
            if (!regex.IsMatch(data.PassWord) || PJUtils.CheckUnicode(data.PassWord))
            {
                result.IsError = true;
                result.Type = 1;
                result.Message = "Mật khẩu không chứa ký tự đặc biệt";
                return result;
            }
            if (!regex.IsMatch(data.ConfirmPassword) || PJUtils.CheckUnicode(data.ConfirmPassword))
            {
                result.IsError = true;
                result.Type = 1;
                result.Message = "Mật khẩu không chứa ký tự đặc biệt";
                return result;
            }
            if (data.ConfirmPassword != data.PassWord)
            {
                result.IsError = true;
                result.Type = 1;
                result.Message = "Mật khẩu không chính xác";
                return result;
            }
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
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
                        result.Type = 2;
                        result.Message = "Cập nhật mật khẩu thành công!";
                        return result;
                    }
                    else
                    {
                        result.IsError = true;
                        result.Type = 2;
                        result.Message = "Hệ thống thực thi không thành công.Vui lòng thử lại";
                        return result;
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Type = 2;
                    result.Message = "Không tìm thấy thông tin tài khoản.";
                    return result;
                }
            }
        }

        #endregion

        #region Change Password
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
                var acc = db.tbl_Accounts.FirstOrDefault(ac => ac.Username == data.UserName);
                if (acc != null)
                {
                    string oldPassword = PJUtils.Encrypt("userpass", data.OldPassword);
                    if (acc.Password == oldPassword)
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

        #endregion

        #region Get Page
        public static object[] GetPage(string searchText = "", int page = 1, int pageSize = 10)
        {
            using (var db = new nhanshiphangContext())
            {
                var lst = new List<tbl_Account>();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_Accounts.AsQueryable();
                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.Username.Contains(searchText) || x.FullName.Contains(searchText) || x.Email.Contains(searchText));
                total = query.Count();
                totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
                lst = query.Select(x=> new tbl_Account {
                    ID = x.ID,
                    UserID = x.UserID,
                    Username = x.Username,
                    FullName = x.FullName,
                    Wallet = x.Wallet,
                    IMG = x.IMG
                }).ToList();
                return new object[] { lst, total, totalPage };
            }
        }

        #endregion

        #region Get Info By ID || UserName
        public static tbl_Account? GetInfo(int? id, string? userName)
        {

            using (var db = new nhanshiphangContext())
            {
                tbl_Account account = null;
                if (id != null && id > 0)
                    account = db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
                if (!string.IsNullOrEmpty(userName))
                    account = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName);
                if(account != null)
                {
                    account.Password = PJUtils.Decrypt("userpass", account.Password);
                }    
                return account;
            }
        }

        #endregion

        #region Create
        public static DataReturnModel<AccountInfo> Create(AccountInfo data, string createdBy)
        {
            var reponse = new DataReturnModel<AccountInfo>();
            if (string.IsNullOrEmpty(data.FullName))
            {
                reponse.IsError = true;
                reponse.Message = "Tên người dùng không được bỏ trống!";
                return reponse;
            }
            if (string.IsNullOrEmpty(data.Email))
            {
                reponse.IsError = true;
                reponse.Message = "Địa chỉ email không được bỏ trống!";
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
            var regex = new Regex("^[a-zA-Z0-9 ]*$");
            var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var regexPhone = new Regex(@"^[0-9]{8,20}$");
            if (!regexEmail.IsMatch(data.Email))
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
            if (!regex.IsMatch(data.UserName) || PJUtils.CheckUnicode(data.UserName))
            {
                reponse.IsError = true;
                reponse.Message = "Tài khoản không chứa ký tự đặc biệt.";
                return reponse;
            }
            if (!regex.IsMatch(data.Password) || PJUtils.CheckUnicode(data.Password))
            {
                reponse.IsError = true;
                reponse.Message = "Mật khẩu không chứa ký tự đặc biệt.";
                return reponse;
            }
            using (var db = new nhanshiphangContext())
            {
                // check userID
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
                var isUserName = db.tbl_Accounts.Any(x => x.Username == data.UserName);
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
                    UserID = data.UserID,
                    Username = data.UserName,
                    Password = PJUtils.Encrypt("userpass", data.Password),
                    Email = data.Email,
                    Wallet = 0,
                    RoleID = data.RoleID,
                    CreatedDate = DateTime.Now,
                    CreatedBy = createdBy
                };
                db.Add(acc);
                var kq1 = db.SaveChanges();
                if (kq1 > 0)
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

        #endregion

        #region Delete
        public static bool Delete(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
                if (acc != null)
                {
                    db.Remove(acc);
                    var kq = db.SaveChanges();
                    return kq > 0;
                }
                return false;
            }
        }

        #endregion

        #region Update
        public static DataReturnModel<AccountInfo> Update(AccountInfo data, string userModifiedBy)
        {
            var result = new DataReturnModel<AccountInfo>();
            if (data != null)
            {
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
                    if (acc != null)
                    {
                        acc.FullName = data.FullName;
                        acc.Phone = data.Phone;
                        acc.RoleID = data.RoleID;
                        acc.Email = data.Email;
                        acc.ModifiedBy = userModifiedBy;
                        acc.ModifiedDate = DateTime.Now;
                        db.Update(acc);
                        var kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            result.IsError = false;
                            result.Data = data;
                            result.Message = "Cập nhật thành công";
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                            return result;
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy thông tin tài khoản";
                        return result;
                    }
                }
            }
            else
            {
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                return result;
            }
        }

        #endregion

        #region Get List User Role
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

        #endregion


        #region Update Info
        public static DataReturnModel<tbl_Account> UpdateInfo(tbl_Account data)
        {
            var result = new DataReturnModel<tbl_Account>();
            if (data != null)
            {
                if (string.IsNullOrEmpty(data.FullName))
                {
                    result.IsError = true;
                    result.Message = "Họ tên không được bỏ trống";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ Email không được bỏ trống";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Phone))
                {
                    result.IsError = true;
                    result.Message = "Số điện thoại không được bỏ trống";
                    return result;
                }
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var regexPhone = new Regex(@"^[0-9]{8,20}$");
                if (!regexEmail.IsMatch(data.Email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ email không hợp lệ.";
                    return result;
                }
                if (!regexPhone.IsMatch(data.Phone))
                {
                    result.IsError = true;
                    result.Message = "Số điện thoại không hợp lệ.";
                    return result;
                }
                using (var db = new nhanshiphangContext())
                {
                    var isEmail = db.tbl_Accounts.Any(x => x.Email == data.Email && x.Username != data.Username);
                    if (isEmail)
                    {
                        result.IsError = true;
                        result.Message = "Địa chỉ email này đã được sử dụng";
                        return result;
                    }
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.Username);
                    if (acc != null)
                    {
                        acc.FullName = data.FullName;
                        acc.Gender = data.Gender;
                        acc.Email = data.Email;
                        acc.Phone = data.Phone;
                        acc.RoleID = data.RoleID;
                        acc.Address = data.Address;
                        acc.ModifiedBy = data.Username;
                        acc.ModifiedDate = DateTime.Now;
                        db.Update(acc);
                        var kq = db.SaveChanges();
                        if (kq > 0)
                        {
                            result.IsError = false;
                            result.Data = data;
                            result.Message = "Cập nhật thành công";
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                            return result;
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Không tìm thấy thông tin tài khoản";
                        return result;
                    }
                }
            }
            else
            {
                result.IsError = true;
                result.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
                return result;
            }
        }

        #endregion
    }
}
