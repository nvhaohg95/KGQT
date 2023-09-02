using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore;
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
                    dtReturn.Key = "Password";
                    dtReturn.Type = 1;
                    dtReturn.Message = Messages.PasswordNotCorrect;
                    return dtReturn;
                }
            }
            else
            {
                dtReturn.IsError = true;
                dtReturn.Key = "Username";
                dtReturn.Type = 1;
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

        public static string ChangePassword(int ID, string password)
        {
            var a = _db.tbl_Accounts.FirstOrDefault(ac => ac.ID == ID);
            if (a != null)
            {
                a.Password = PJUtils.Encrypt("userpass", password);
                _db.Update(a);
                int kq = _db.SaveChanges();
                return kq.ToString();
            }
            return null;
        }

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
                var regexUserName = new Regex("^[a-zA-Z0-9 ]*$");
                var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var regexPhone = new Regex(@"^[0-9]{8,20}$");

                if (!regexUserName.IsMatch(data.UserName) || PJUtils.CheckUnicode(data.UserName))
                {
                    result.IsError = true;
                    result.Key = "UserName";
                    result.Type = 1;
                    result.Message = "Tài khoản không chứa ký tự đặc biệt.";
                    return result;
                }
                if (!regexUserName.IsMatch(data.PassWord) || PJUtils.CheckUnicode(data.PassWord))
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
                if(!regexPhone.IsMatch(data.Phone))
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
                        var bytes = PJUtils.ResizeImage(data.File);
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
                    _db.tbl_Accounts.Remove(acc);
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

        #region Forgot Password
        public static DataReturnModel ForgotPassword(ForgotPasswordModel data)
        {
            DataReturnModel result = new DataReturnModel();

            return result;
        }
        #endregion

    }
}
