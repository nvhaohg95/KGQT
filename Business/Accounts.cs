using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Text.RegularExpressions;

namespace KGQT.Business
{
    public class Accounts : BusinessBase
    {

        protected static KGNewContext _db = new KGNewContext();

        #region Select
        public static DataReturnModel Login(string userName, string password)
        {
            var dtReturn = new DataReturnModel();
            if (string.IsNullOrEmpty(userName))
            {
                dtReturn.IsError = true;
                dtReturn.Type = 1;
                dtReturn.Key = "UserName";
                dtReturn.Message = "Vui lòng nhập tên tài khoản";
                return dtReturn;
            }
            if (string.IsNullOrEmpty(password))
            {
                dtReturn.IsError = true;
                dtReturn.Type = 1;
                dtReturn.Key = "PassWord";
                dtReturn.Message = "Vui lòng nhập mật khẩu";
                return dtReturn;
            }
            var acc = _db.tbl_Accounts.FirstOrDefault(a => a.Username == userName);
            if (acc != null)
            {
                password = PJUtils.Encrypt("userpass", password);
                if (acc.Password == password)
                {
                    dtReturn.IsError = false;
                    dtReturn.Data = GetFullInfo(acc, 0, "");
                    return dtReturn;
                }
                else
                {
                    dtReturn.IsError = true;
                    dtReturn.Type = 1;
                    dtReturn.Key = "PassWord";
                    dtReturn.Message = Messages.PasswordNotCorrect;
                    return dtReturn;
                }
            }
            else
            {
                dtReturn.IsError = true;
                dtReturn.Type = 1;
                dtReturn.Key = "UserName";
                dtReturn.Message = Messages.CannotFindUser;
                return dtReturn;
            }
        }

        public static tbl_Account GetByUserName(string Username)
        {
            tbl_Account acc = _db.tbl_Accounts.Where(a => a.Username == Username).FirstOrDefault();
            return acc;
        }

        public static tbl_Account GetByID(int ID)
        {
            tbl_Account acc = _db.tbl_Accounts.FirstOrDefault(a => a.ID == ID);
            return acc;
        }

        public static UserLogin GetFullInfo(tbl_Account? acc, int? id, string userName)
        {
            var us = new UserLogin();

            if (acc == null && id > 0)
                acc = GetByID(id.Value);


            if (acc == null && !string.IsNullOrEmpty(userName))
                acc = GetByUserName(userName);

            var accInfo = _db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
            if (accInfo == null) return null;
            us.ID = acc.ID;
            us.Username = acc.Username;
            us.Status = acc.Status;
            us.Wallet = acc.Wallet;
            us.UserLevel = acc.UserLevel;
            us.BirthDay = accInfo.BirthDay;
            us.LastName = accInfo.LastName;
            us.FirstName = accInfo.FirstName;
            us.Longitude = accInfo.Longitude;
            us.Email = acc.Email;
            us.Address = accInfo.Address;
            us.Phone = accInfo.Phone;
            us.Latitude = accInfo.Latitude;
            us.Gender = accInfo.Gender;
            us.IMG = accInfo.IMG;
            us.RoleID = acc.RoleID;
            us.IsActive = acc.IsActive;
            us.IsOutCityCustomer = acc.IsOutCityCustomer;
            us.IsNotReceiveMail = acc.IsNotReceiveMail;
            us.CreateDate = acc.CreatedDate;
            return us;
        }

        public static tbl_Account GetByEmail(string Email)
        {
            tbl_Account acc = _db.tbl_Accounts.FirstOrDefault(a => a.Email == Email);
            if (acc != null)
                return acc;
            else
                return null;
        }

        public static int GetTotalBeforeAccept(int UID)
        {
            //int total = _db.tbl_OrderShops.Where(x => x.UID == UID && ((x.Status < 7 && x.Status > 1) || x.Status == 12)).Count();

            //return total;
            return 0;
        }

        public static List<tbl_Account> GetAllByRoleID(int RoleID)
        {

            List<tbl_Account> las = new List<tbl_Account>();
            las = _db.tbl_Accounts.Where(a => a.RoleID == RoleID).ToList();
            return las;
        }

        #endregion

        #region CRUD
        public static string UpdateIsActive(int ID, bool IsActive, string Email)
        {
            var a = _db.tbl_Accounts.FirstOrDefault(ac => ac.ID == ID);
            if (a != null)
            {
                a.IsActive = IsActive;
                a.Email = Email;
                _db.Update(a);
                string kq = _db.SaveChanges().ToString();
                return kq;
            }
            else
                return null;
        }

        public static string UpdateWallet(int ID, double Wallet)
        {
            var a = _db.tbl_Accounts.FirstOrDefault(ac => ac.ID == ID);
            if (a != null)
            {
                a.Wallet = Wallet;
                _db.Update(a);
                string kq = _db.SaveChanges().ToString();
                return kq;
            }
            else
                return null;
        }

        public static string UpdateUserLevel(int ID, int UserLevel, DateTime ModifiedDate, string ModifiedBy)
        {
            var a = _db.tbl_Accounts.Where(ac => ac.ID == ID).SingleOrDefault();
            if (a != null)
            {
                a.UserLevel = UserLevel;
                a.ModifiedBy = ModifiedBy;
                a.ModifiedDate = ModifiedDate;
                _db.Update(a);
                string kq = _db.SaveChanges().ToString();
                return kq;
            }
            else
                return null;
        }
        #endregion

        #region Get User Login
        public static UserLogin GetUserLogin(string userName)
        {
            var acc = _db.tbl_Accounts.FirstOrDefault(x => x.Username == userName);
            if (acc != null)
            {
                var user = new UserLogin()
                {
                    ID = acc.ID,
                    Username = acc.Username,
                    Status = acc.Status,
                    Wallet = acc.Wallet,
                    UserLevel = acc.UserLevel,
                    Email = acc.Email,
                    RoleID = acc.RoleID,
                    IsActive = acc.IsActive,
                    IsOutCityCustomer = acc.IsOutCityCustomer,
                    IsNotReceiveMail = acc.IsNotReceiveMail,
                    CreateDate = acc.CreatedDate
                };
                var accInfo = _db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
                if (accInfo != null)
                {
                    user.BirthDay = accInfo.BirthDay;
                    user.LastName = accInfo.LastName;
                    user.FirstName = accInfo.FirstName;
                    user.Longitude = accInfo.Longitude;
                    user.Address = accInfo.Address;
                    user.Phone = accInfo.Phone;
                    user.Latitude = accInfo.Latitude;
                    user.Gender = accInfo.Gender;
                    user.IMG = accInfo.IMG;
                }
                return user;
            }
            return null;
        }

        #endregion

        #region Register Account
        public static DataReturnModel RegisterAccount(SignUpModel data)
        {
            var result = new DataReturnModel() { IsError = false, Message = "" };
            try
            {
                var regex = new Regex("^[a-zA-Z0-9 ]*$");
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var regexPhone = new Regex(@"^[0-9]{8,20}$");

                if (!regex.IsMatch(data.UserName) || PJUtils.CheckUnicode(data.UserName))
                {
                    result.IsError = true;
                    result.Key = "UserName";
                    result.Type = 1;
                    result.Message = "Tài khoản không chứa ký tự đặc biệt.";
                    return result;
                }
                if (!regex.IsMatch(data.PassWord) || PJUtils.CheckUnicode(data.PassWord))
                {
                    result.IsError = true;
                    result.Key = "PassWord";
                    result.Type = 1;
                    result.Message = "Mật khẩu không chứa ký tự đặc biệt.";
                    return result;
                }
                if (data.UserName.Contains(" "))
                {
                    result.IsError = true;
                    result.Key = "UserName";
                    result.Type = 1;
                    result.Message = "Tài khoản không chứa khoảng trắng.";
                    return result;
                }
                if (data.PassWord.Contains(" "))
                {
                    result.IsError = true;
                    result.Key = "PassWord";
                    result.Type = 1;
                    result.Message = "Mật khẩu không chứa khoảng trắng.";
                    return result;
                }
                if (!regexEmail.IsMatch(data.Email))
                {
                    result.IsError = true;
                    result.Key = "Email";
                    result.Type = 1;
                    result.Message = "Địa chỉ email không hợp lệ.";
                    return result;
                }
                if (!regexPhone.IsMatch(data.Phone))
                {
                    result.IsError = true;
                    result.Key = "Phone";
                    result.Type = 1;
                    result.Message = "Số điện thoại không hợp lệ.";
                    return result;
                }

                var isUserName = _db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
                var isEmail = _db.tbl_Accounts.FirstOrDefault(x => x.Email == data.Email);
                if (isUserName != null)
                {
                    result.IsError = true;
                    result.Key = "UserName";
                    result.Type = 1;
                    result.Message = "Tên tài khoản đã được sử dụng.";
                    return result;
                }
                if (isEmail != null)
                {
                    result.IsError = true;
                    result.Key = "Email";
                    result.Type = 1;
                    result.Message = "Địa chỉ email đã được sử dụng.";
                    return result;
                }

                var acc = new tbl_Account()
                {
                    Username = data.UserName,
                    Password = PJUtils.Encrypt("userpass", data.PassWord),
                    Email = data.Email,
                    Wallet = 0,
                    CreatedDate = DateTime.Now,
                    CreatedBy = data.UserName
                };
                _db.Add(acc);
                int kq1 = _db.SaveChanges();
                if (kq1 > 0)
                {
                    if (data.File != null)
                    {
                        var bytes = FileService.ResizeImage(data.File);
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(data.File.FileName);
                        string path = Path.Combine(data.Path, fileName);
                        File.WriteAllBytes(path, bytes);
                        data.IMG = "/uploads/avatars/" + fileName;
                    }
                    var accInfor = new tbl_AccountInfo()
                    {
                        UID = acc.ID,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        Gender = data.Gender,
                        BirthDay = data.BirthDay,
                        IMG = data.IMG,
                        Email = data.Email,
                        Phone = data.Phone,
                        Address = data.Address,
                        CreatedDate = acc.CreatedDate,
                        CreatedBy = acc.CreatedBy
                    };
                    _db.Add(accInfor);
                    int kq2 = _db.SaveChanges();
                    if (kq2 > 0)
                    {
                        result.IsError = false;
                        result.Type = 2;
                        result.Message = "Tạo tài khoản thành công!";
                        result.Data = acc;
                        return result;
                    }
                    _db.Remove(acc);
                    _db.SaveChanges();
                }

                result.IsError = true;
                result.Type = 2;
                result.Message = "Tạo tài khoản không thành công.";
                return result;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Type = 2;
                result.Message = "Tạo tài khoản không thành công.";
                return result;
            }

        }

        #endregion

        #region Send Mail Forget Password
        public static async Task<DataReturnModel> SendMailForgetPassword(MailSettings mailSetting, string email)
        {
            DataReturnModel result = new DataReturnModel();
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
                var acc = _db.tbl_Accounts.FirstOrDefault(x => x.Email == email);
                if (acc != null)
                {
                    acc.Token = Guid.NewGuid().ToString();
                    var accInfo = _db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
                    var name = acc.Username;
                    string sToken = acc.Token + "|" + DateTime.Now.ToString();
                    string link = "https://localhost:44330/auth/forgotpassword?id=&tk=";
                    string id = Helper.Base64Encode(acc.Username);
                    string token = Helper.Base64Encode(sToken);
                    string subject = "YÊU CẦU CẤP PHÁT LẠI MẬT KHẨU";
                    string body = "<div>Xin chào <b>{0}</b>.</div><br><div>Yêu cầu cấp phát lại mật khẩu của bạn đã được xác nhận, vui lòng truy cập đường link bên dưới để tiếp tục thay đổi mật khẩu của bạn. <span><a href='{1}'>link</a></span></div><div></div><br><div>Cám ơn.</div>";
                    if (accInfo != null)
                        name = accInfo.FirstName + " " + accInfo.LastName;
                    link = $"https://localhost:44330/auth/forgotpassword?id={id}&tk={token}";
                    body = string.Format(body, name, link);

                    //send mail
                    var status = await MailService.SendMailAsync(mailSetting, email, subject, body);
                    if (status)
                    {
                        result.IsError = false;
                        result.Message = "Yêu cầu của bạn đã được gửi đi.Vui lòng kiểm tra email!";
                        _db.Update(acc);
                        _db.SaveChanges();
                        return result;
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Hệ thống gửi mail đang cập nhật, vui lòng thử lại sau.";
                        return result;
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Email này chưa đăng ký. Vui lòng kiểm tra lại!";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = "Hệ thống gửi mail đang cập nhật, vui lòng thử lại sau.";
                return result;
            }

        }

        #endregion

        #region Get Account by Username & Token
        public static tbl_Account GetByUsernameAndToken(string userName, string token)
        {
            return _db.tbl_Accounts.FirstOrDefault(x => x.Username == userName && x.Token == token);
        }
        #endregion

        #region Forgot Password
        public static DataReturnModel ForgotPassword(ForgotPassWord data)
        {
            DataReturnModel result = new DataReturnModel();
            var regex = new Regex("^[a-zA-Z0-9 ]*$");

            if (string.IsNullOrEmpty(data.PassWord))
            {
                result.IsError = true;
                result.Type = 1;
                result.Key = "PassWord";
                result.Message = "Vui lòng nhập mật khẩu";
                return result;
            }
            if (string.IsNullOrEmpty(data.ConfirmPassword))
            {
                result.IsError = true;
                result.Type = 1;
                result.Key = "ConfirmPassword";
                result.Message = "Vui lòng nhập mật khẩu";
                return result;
            }
            if (!regex.IsMatch(data.PassWord) || PJUtils.CheckUnicode(data.PassWord))
            {
                result.IsError = true;
                result.Key = "PassWord";
                result.Type = 1;
                result.Message = "Mật khẩu không chứa ký tự đặc biệt";
                return result;
            }
            if (!regex.IsMatch(data.ConfirmPassword) || PJUtils.CheckUnicode(data.ConfirmPassword))
            {
                result.IsError = true;
                result.Key = "PassWord";
                result.Type = 1;
                result.Message = "Mật khẩu không chứa ký tự đặc biệt";
                return result;
            }
            if (data.ConfirmPassword != data.PassWord)
            {
                result.IsError = true;
                result.Type = 1;
                result.Key = "ConfirmPassword";
                result.Message = "Mật khẩu không chính xác";
                return result;
            }
            var acc = _db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
            if (acc != null)
            {
                acc.Password = PJUtils.Encrypt("userpass", data.PassWord);
                acc.Token = null;
                acc.ModifiedBy = acc.Username;
                acc.ModifiedDate = DateTime.Now;
                _db.Update(acc);
                _db.SaveChanges();

                result.IsError = false;
                result.Type = 2;
                result.Message = "Cập nhật mật khẩu thành công!";
                return result;
            }
            result.IsError = true;
            result.Type = 2;
            result.Message = "Không tìm thấy thông tin tài khoản.";
            return result;
        }
        #endregion

        #region Change Password
        public static DataReturnModel ChangePassword(ChangePassword data)
        {
            var result = new DataReturnModel();
            if (string.IsNullOrEmpty(data.UserName))
            {
                result.IsError = true;
                result.Message = "Tài khoảng không được bỏ trống";
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
                result.Message = "Mật khẩu không được bỏ trống";
                return result;
            }
            if (data.ConfirmPassword != data.NewPassword)
            {
                result.IsError = true;
                result.Message = "Mật khẩu không trùng khớp";
                return result;
            }
            var acc = _db.tbl_Accounts.FirstOrDefault(ac => ac.Username == data.UserName);
            if (acc != null)
            {
                string oldPassword = PJUtils.Encrypt("userpass", data.OldPassword);
                if (acc.Password == oldPassword)
                {
                    acc.Password = PJUtils.Encrypt("userpass", data.NewPassword);
                    _db.Update(acc);
                    int kq = _db.SaveChanges();
                    if (kq > 0)
                    {
                        result.IsError = false;
                        result.Message = "Cập nhật thành công!";
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Cập nhật không thành công";
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "Mật khẩu không chính xác";
                    return result;
                }
            }
            else
            {
                result.IsError = true;
                result.Message = "Cập nhật không thành công";
            }
            return result;
        }
        #endregion

    }
}
