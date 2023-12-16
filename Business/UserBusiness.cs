using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using MailKit.Search;
using System.Text.RegularExpressions;

namespace KGQT.Business
{
    public static class UserBusiness
    {
        #region Get Page User
        public static object[] GetPage(string searchText = "", int page = 1, int pageSize = 5)
        {
            using (var db = new nhanshiphangContext())
            {
                var lst = new List<tbl_Account>();
                int total = 0;
                int totalPage = 0;
                var query = db.tbl_Accounts.AsQueryable();
                if (!string.IsNullOrEmpty(searchText))
                    query = query.Where(x => x.FullName.Contains(searchText) || x.Username.Contains(searchText));

                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    lst = query.OrderByDescending(x => x.CreatedDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }
                return new object[] { lst, total, totalPage };
            }
        }

        #endregion

        #region Get User By ID
        public static tbl_Account GetUserByID(int id)
        {
            using(var db = new nhanshiphangContext())
            {
                return db.tbl_Accounts.FirstOrDefault(x => x.ID == id);
            }
        }
        #endregion

        #region Add User
        public static DataReturnModel<AccountInfo> AddUser(AccountInfo data,string createdBy)
        {
            var reponse = new DataReturnModel<AccountInfo>();
            if (string.IsNullOrEmpty(data.UserID))
            {
                reponse.IsError = true;
                reponse.Message = "Mã người dùng không được bỏ trống!";
                return reponse;
            }
            if (string.IsNullOrEmpty(data.FullName))
            {
                reponse.IsError = true;
                reponse.Message = "Họ tên người dùng không được bỏ trống!";
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
                var kq = db.SaveChanges();
                if(kq > 0)
                {
                    reponse.IsError = false;
                    reponse.Message = "Thêm thành công!";
                    reponse.Data = data;
                    return reponse;
                }
            }
            reponse.IsError = true;
            reponse.Message = "Hệ thống thực thi không thành công. Vui lòng thử lại";
            return reponse;
        }
        #endregion

        #region Deletee User
        public static bool DeleteUser(int id)
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

        #region Get User By User Name
        public static tbl_Account GetUserByUserName(string username)
        {
            var user = new tbl_Account();

            using (var db = new nhanshiphangContext())
            {
                var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == username);
                if (acc != null)
                {
                    user.ID = acc.ID;
                    user.UserID = acc.UserID;
                    user.Username = acc.Username;
                    user.Password = PJUtils.Encrypt("userpass", acc.Password);
                    user.RoleID = acc.RoleID;
                    user.Wallet = acc.Wallet;
                }
            }
            return user;
        }
        #endregion

        #region Update User
        public static DataReturnModel<AccountInfo> Update(AccountInfo data,string userModifiedBy)
        {
            var result = new DataReturnModel<AccountInfo>();
            if(data != null)
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

        #region Update User
        public static DataReturnModel<AccountInfo> UpdateInfo(AccountInfo data)
        {
            var result = new DataReturnModel<AccountInfo>();
            if (data != null)
            {
                using (var db = new nhanshiphangContext())
                {
                    var acc = db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
                    if (acc != null)
                    {
                        acc.Email = data.Email;
                        acc.ModifiedBy = data.UserName;
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
