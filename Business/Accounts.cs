using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public class Accounts : BusinessBase
    {

        protected static KGNewContext _db = new KGNewContext();

        #region Select
        public static DataReturnModel Login(string Username, string Password)
        {
            Password = PJUtils.Encrypt("userpass", Password);
            var dtReturn = new DataReturnModel();
            var acc = _db.tbl_Accounts.FirstOrDefault(a => a.Username == Username);
            if (acc != null)
            {
                if (acc.Password == Password)
                {
                    dtReturn.IsError = false;
                    dtReturn.Data = GetFullInfo(acc, 0, "");
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

        public static DataReturnModel RegisterAccount(SignUpModel data)
        {
            var result = new DataReturnModel() { IsError = false, Message = "" };
            var exist = _db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
            if (exist != null)
            {
                result.IsError = true;
                result.Message = "Tên đăng nhập đã được sử dụng";
                return result;
            }
            var pass = PJUtils.Encrypt("userpass", data.PassWord);
            var user = new tbl_Account()
            {
                Username = data.UserName,
                Password = pass,
                Email = data.Email,
                RoleID = data.RoleID,
                UserLevel = data.UserLevel,
                Status = data.Status,
                Wallet = 0,
                CreatedDate = data.CraetedDate,
                CreatedBy = data.CraetedBy,
            };
            _db.Add(user);
            int kq = _db.SaveChanges();
            if (kq > 0)
            {
                var acc = _db.tbl_Accounts.FirstOrDefault(x => x.Username == data.UserName);
                if (acc != null)
                {
                    var accInfor = new tbl_AccountInfo()
                    {
                        UID = user.ID,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        Gender = data.Gender,
                        BirthDay = data.BirthDay,
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
                        result.Message = "Tạo tài khoản thành công";
                        result.Data = user;
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = Messages.AddError;
                    }
                }
            }
            else
            {
                result.IsError = true;
                result.Message = Messages.AddError;
            }
            return result;
        }

    }
}
