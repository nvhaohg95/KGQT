using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using System.Text.RegularExpressions;

namespace KGQT.Business
{
    public class AccountBusiness
    {
        #region Login
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
            using (var db = new nhanshiphangContext())
            {

                var acc = db.tbl_Accounts.FirstOrDefault(a => a.Username == userName);
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
        }

        #endregion

        #region Get By UserName
        public static tbl_Account? GetByUserName(string Username)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc =  db.tbl_Accounts.Where(a => a.Username == Username).FirstOrDefault();
                if (acc != null)
                    return acc;
                else
                    return null;
            }
        }

        #endregion

        #region Get By ID
        public static tbl_Account? GetByID(int ID)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(a => a.ID == ID);
                if (acc != null)
                    return acc;
                else
                    return null;
            }
        }

        #endregion

        #region Get Info By ID || UserName
        public static UserLogin? GetInfo(int? id, string? userName)
        {

            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.ID == id || x.Username == userName);
                if (acc != null)
                {
                    var us = GetFullInfo(acc, -1, "");
                    return us;
                }
                else
                    return null;
            }
        }

        #endregion

        #region Get Info By Account || ID || UserName
        public static UserLogin GetFullInfo(tbl_Account? acc, int? id, string? userName)
        {

            if (acc == null && id > 0)
                acc = GetByID(id.Value);


            if (acc == null && !string.IsNullOrEmpty(userName))
                acc = GetByUserName(userName);

            if(acc != null)
            {
                using (var db = new nhanshiphangContext())
                {
                    UserLogin us = new UserLogin();

                    var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
                    if (accInfo != null)
                    {
                        us = new UserLogin()
                        {
                            ID = acc.ID,
                            Username = acc.Username,
                            Status = acc.Status,
                            Wallet = acc.Wallet,
                            UserLevel = acc.UserLevel,
                            BirthDay = accInfo.BirthDay,
                            LastName = accInfo.LastName,
                            FirstName = accInfo.FirstName,
                            Longitude = accInfo.Longitude,
                            Email = acc.Email,
                            Address = accInfo.Address,
                            Phone = accInfo.Phone,
                            Latitude = accInfo.Latitude,
                            Gender = accInfo.Gender,
                            IMG = accInfo.IMG,
                            RoleID = acc.RoleID,
                            IsActive = acc.IsActive,
                            IsOutCityCustomer = acc.IsOutCityCustomer,
                            IsNotReceiveMail = acc.IsNotReceiveMail,
                            CreateDate = acc.CreatedDate,
                            UserID = acc.UserID,
                        };
                        return us;
                    }
                }
            }    
            return null;
        }

        #endregion

        #region Get User Login
        public static UserLogin? GetUserLogin(string userName)
        {
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == userName);
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
                    var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
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
        }

        #endregion

        #region Register Account
        public static DataReturnModel RegisterAccount(SignUpModel data)
        {
            var result = new DataReturnModel();
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

                using (var db = new nhanshiphangContext())
                {
                    var isUserName = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
                    if (isUserName != null)
                    {
                        result.IsError = true;
                        result.Key = "UserName";
                        result.Type = 1;
                        result.Message = "Tên tài khoản đã được sử dụng";
                        return result;
                    }
                    var acc = new tbl_Account()
                    {
                        Username = data.UserName,
                        UserID = data.UserName,
                        Password = PJUtils.Encrypt("userpass", data.PassWord),
                        Email = data.Email,
                        Wallet = 0,
                        Status = 1,
                        RoleID = 4,
                        CreatedDate = DateTime.Now,
                        CreatedBy = data.UserName
                    };
                    db.Add(acc);
                    int kq1 = db.SaveChanges();
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
                        db.Add(accInfor);
                        int kq2 = db.SaveChanges();
                        if (kq2 > 0)
                        {
                            result.IsError = false;
                            result.Type = 2;
                            result.Message = "Tạo tài khoản thành công!";
                            result.Data = acc;
                            return result;
                        }
                        else
                        {
                            db.Remove(acc);
                            db.SaveChanges();
                            result.IsError = true;
                            result.Type = 2;
                            result.Message = "Tạo tài khoản không thành công.";
                            return result;
                        }
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
                result.Message = "Hệ thống thực thi không thành công.";
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
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Email == email);
                    if (acc != null)
                    {
                        acc.Token = Guid.NewGuid().ToString();
                        var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
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
                            db.Update(acc);
                            db.SaveChanges();
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công.";
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
                result.Message = "Hệ thống thực thi không thành công.";
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
                    int isSave = db.SaveChanges();
                    if(isSave > 0)
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
                        result.Message = "Hệ thống thực thi không thành công.";
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
            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(ac => ac.Username == data.UserName);
                if (acc != null)
                {
                    string oldPassword = PJUtils.Encrypt("userpass", data.OldPassword);
                    if (acc.Password == oldPassword)
                    {
                        acc.Password = PJUtils.Encrypt("userpass", data.NewPassword);
                        db.Update(acc);
                        int isSave = db.SaveChanges();
                        if (isSave > 0)
                        {
                            result.IsError = false;
                            result.Message = "Cập nhật thành công!";
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Cập nhật không thành công. Vui lòng kiểm tra lại!";
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
                    result.Message = "Hệ thống thực thi không thành công.";
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
                var lst = new List<AccountInfo>();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_AccountInfos.AsQueryable();
                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.FirstName.Contains(searchText) || x.LastName.Contains(searchText));
                total = query.Count();
                totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
                var accInfos = query.ToList();
                if (accInfos.Count > 0)
                {
                    lst = (from accInfo in accInfos
                           join acc in db.tbl_Accounts.ToList()
                           on accInfo.UID equals acc.ID
                           orderby acc.CreatedDate descending
                           select new AccountInfo()
                           {
                               ID = acc.ID,
                               UserID = acc.UserID,
                               FirstName = accInfo.FirstName,
                               LastName = accInfo.LastName,
                               BirthDay = accInfo.BirthDay,
                               Email = accInfo.Email,
                               Phone = accInfo.Phone,
                               IMG = accInfo.IMG
                           }).ToList();
                }
                return new object[] { lst, total, totalPage };
            }
        }
        #endregion

        #region Create
        public static DataReturnModel Create(AccountInfo data, string createdBy)
        {
            var reponse = new DataReturnModel();
            if (string.IsNullOrEmpty(data.UserID))
            {
                reponse.IsError = true;
                reponse.Message = "Mã người dùng không được bỏ trống!";
                return reponse;
            }
            if (string.IsNullOrEmpty(data.FirstName))
            {
                reponse.IsError = true;
                reponse.Message = "Họ và tên người dùng không được bỏ trống!";
                return reponse;
            }
            if (string.IsNullOrEmpty(data.LastName))
            {
                reponse.IsError = true;
                reponse.Message = "Họ và tên người dùng không được bỏ trống!";
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
                var isUser = db.tbl_Accounts.Any(x => x.UserID == data.UserID);
                if (isUser)
                {
                    reponse.IsError = true;
                    reponse.Message = $"Mã định danh {data.UserID} đã tồn tại!";
                    return reponse;
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
                    var accInfo = new tbl_AccountInfo()
                    {
                        UID = acc.ID,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        BirthDay = data.BirthDay,
                        Gender = data.Gender,
                        Email = data.Email,
                        Phone = data.Phone,
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.Now
                    };
                    db.Add(accInfo);
                    var kq2 = db.SaveChanges();
                    if (kq2 > 0)
                    {
                        reponse.IsError = false;
                        reponse.Message = "Tạo tài khoản thành công!";
                        reponse.Data = data;
                        return reponse;
                    }
                    else
                    {
                        reponse.IsError = true;
                        reponse.Message = "Hệ thông thực thi không thành công.";
                        return reponse;
                    }    
                }
                else
                {
                    reponse.IsError = true;
                    reponse.Message = "Hệ thông thực thi không thành công.";
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
                try
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
                    var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == id);
                    db.Remove(acc);
                    db.Remove(accInfo);
                    var isSave = db.SaveChanges();
                    if (isSave > 0)
                        return true;
                    return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        #endregion

        #region Update
        public static DataReturnModel Update(AccountInfo data, string userModifiedBy)
        {
            var result = new DataReturnModel();
            if (data != null)
            {
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
                    if (acc != null)
                    {
                        acc.Email = data.Email;
                        acc.ModifiedBy = userModifiedBy;
                        acc.ModifiedDate = DateTime.Now;
                        db.Update(acc);
                        var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
                        if (acc != null)
                        {
                            accInfo.FirstName = data.FirstName;
                            accInfo.LastName = data.LastName;
                            accInfo.Gender = data.Gender;
                            accInfo.BirthDay = data.BirthDay;
                            accInfo.Email = data.Email;
                            accInfo.Phone = data.Phone;
                            accInfo.Address = data.Address;
                            accInfo.Phone = data.Phone;
                            accInfo.ModifiedBy = userModifiedBy;
                            accInfo.ModifiedDate = DateTime.Now;
                            db.Update(accInfo);
                        }
                        var isSave = db.SaveChanges();
                        if (isSave > 0)
                        {
                            result.IsError = false;
                            result.Data = data;
                            result.Message = "Cập nhật thành công";
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Cập nhật không thành công";
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
                result.Message = "Cập nhật không thành công";
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

        #region Get Info
        public static AccountInfo GetInfo(string username)
        {
            var user = new AccountInfo();

            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == username);
                if (acc != null)
                {
                    user.ID = acc.ID;
                    user.UserID = acc.UserID;
                    user.UserName = acc.Username;
                    user.Password = PJUtils.Encrypt("userpass", acc.Password);
                    user.RoleID = acc.RoleID;
                    user.Wallet = acc.Wallet;
                    var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
                    if (accInfo != null)
                    {
                        user.IMG = accInfo.IMG;
                        user.BirthDay = accInfo.BirthDay;
                        user.Gender = accInfo.Gender;
                        user.Phone = accInfo.Phone;
                        user.Email = accInfo.Email;
                        user.Address = accInfo.Address;
                        user.FirstName = accInfo.FirstName;
                        user.LastName = accInfo.LastName;
                    }
                }
            }
            return user;
        }

        #endregion

        #region Update Info
        public static DataReturnModel UpdateInfo(AccountInfo data)
        {
            var result = new DataReturnModel();
            if (data != null)
            {
                if(string.IsNullOrEmpty(data.FirstName))
                {
                    result.IsError = true;
                    result.Message = "Họ không được để trống";
                    return result;
                }
                if (string.IsNullOrEmpty(data.LastName))
                {
                    result.IsError = true;
                    result.Message = "Tên không được để trống";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Email))
                {
                    result.IsError = true;
                    result.Message = "Địa chỉ Email không được để trống";
                    return result;
                }
                if (string.IsNullOrEmpty(data.Phone))
                {
                    result.IsError = true;
                    result.Message = "Số điện thoại không được để trống";
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
                    var isEmail = db.tbl_Accounts.Any(x => x.Email == data.Email);
                    if(isEmail)
                    {
                        result.IsError = true;
                        result.Message = "Địa chỉ email này đã được sử dụng.";
                        return result;
                    }    
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
                    if (acc != null)
                    {
                        acc.Email = data.Email;
                        acc.ModifiedBy = data.UserName;
                        acc.ModifiedDate = DateTime.Now;
                        db.Update(acc);
                        var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
                        if (acc != null)
                        {
                            accInfo.FirstName = data.FirstName;
                            accInfo.LastName = data.LastName;
                            accInfo.Gender = data.Gender;
                            accInfo.BirthDay = data.BirthDay;
                            accInfo.Email = data.Email;
                            accInfo.Phone = data.Phone;
                            accInfo.Address = data.Address;
                            accInfo.Phone = data.Phone;
                            accInfo.ModifiedBy = data.UserName;
                            accInfo.ModifiedDate = DateTime.Now;
                            db.Update(accInfo);
                        }
                        var isSave = db.SaveChanges();
                        if (isSave > 0)
                        {
                            result.IsError = false;
                            result.Data = data;
                            result.Message = "Cập nhật thành công";
                            return result;
                        }
                        else
                        {
                            result.IsError = true;
                            result.Message = "Hệ thống thực thi không thành công";
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
                result.Message = "Cập nhật không thành công";
                return result;
            }
        }

        #endregion
    }
}
