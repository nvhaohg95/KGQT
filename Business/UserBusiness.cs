using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using MailKit.Search;
using System.Text.RegularExpressions;

namespace KGQT.Business
{
    public static class UserBusiness
    {
        #region Get List User
        public static object[] GetPage(string searchText = "", int page = 1, int pageSize = 5)
        {
            using (var db = new nhanshiphangContext())
            {
                var lst = new List<AccountInfo>();
                int total = 0;
                int totalPage = 0;
                var query = from acc in db.tbl_Accounts.ToList().DefaultIfEmpty()
                            join accInfo in db.tbl_AccountInfos.ToList().DefaultIfEmpty()
                            on acc.ID equals accInfo.UID
                            select new AccountInfo()
                            {
                                ID = acc.ID,
                                UserID = acc.UserID,
                                FirstName = accInfo.FirstName,
                                LastName = accInfo.LastName,
                                FullName = accInfo.FirstName + " " + accInfo.LastName,
                                BirthDay = accInfo.BirthDay,
                                Gender = accInfo.Gender,
                                Email = accInfo.Email,
                                Phone = accInfo.Phone,
                                IMG = accInfo.IMG
                            };
                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.FirstName.Contains(searchText) || x.LastName.Contains(searchText));

                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    lst = query.OrderByDescending(x => x.UserName)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }
                return new object[] { lst, total, totalPage };
            }
        }

        #endregion

        #region Get User By ID
        public static AccountInfo GetUser(int id)
        {
            using(var db = new nhanshiphangContext())
            {
                var user = new AccountInfo();
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
                var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == id);
                if (acc != null)
                {
                    user.ID = acc.ID;
                    user.UserID = acc.UserID;
                    user.UserName = acc.Username;
                    user.Password = acc.Password;
                    user.RoleID = acc.RoleID;
                }
                if(accInfo != null)
                {
                    user.FirstName = accInfo.FirstName;
                    user.LastName = accInfo.LastName;
                    user.FullName = accInfo.FirstName + " " + accInfo.LastName;
                    user.BirthDay = accInfo.BirthDay;
                    user.Gender = accInfo.Gender;
                    user.Email = accInfo.Email;
                    user.Phone = accInfo.Phone;
                    user.IMG = accInfo.IMG;
                }
                return user;
            }
        }
        #endregion

        #region Get User Infor
        public static AccountInfo GetUserInfor(string username)
        {
            var data = new AccountInfo();

            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == username);
                if(acc != null)
                {
                    data.ID = acc.ID;
                    data.UserID = acc.UserID;
                    data.UserName = acc.Username;
                    data.Password = PJUtils.Encrypt("userpass", acc.Password);
                    data.RoleID = acc.RoleID;
                    var accInfo = db.tbl_AccountInfos.FirstOrDefault(x => x.UID == acc.ID);
                    if(accInfo != null)
                    {
                        data.IMG = accInfo.IMG;
                        data.BirthDay = accInfo.BirthDay;
                        data.Gender = accInfo.Gender;
                        data.Phone = accInfo.Phone;
                        data.Email = accInfo.Email;
                        data.Address = accInfo.Address;
                        data.FirstName = accInfo.FirstName;
                        data.LastName = accInfo.LastName;
                        data.FullName = string.Concat(accInfo.FirstName, " ", accInfo.LastName);
                    }
                }
            }
            return data;
        }
        #endregion

        #region Add User
        public static DataReturnModel AddUser(AccountInfo data,string createdBy)
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
                if(isUser)
                {
                    reponse.IsError = true;
                    reponse.Message = $"Mã {data.UserID} đã tồn tại!";
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
                if(kq1 > 0)
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
                    if(kq2 > 0)
                    {
                        reponse.IsError = false;
                        reponse.Message = "Thêm thành công!";
                        reponse.Data = data;
                        return reponse;
                    }
                }
            }
            reponse.IsError = true;
            reponse.Message = "Thêm không thành công!";
            return reponse;
        }
        #endregion

        #region Deletee User
        public static bool DeleteUser(int id)
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


    }
}
