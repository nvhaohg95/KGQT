using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public static class UserBusiness
    {
        public static List<AccountInfo> GetListUser()
        {
            using(var db = new nhanshiphangContext())
            {
                var lst = new List<AccountInfo>();
                lst = (from acc in db.tbl_Accounts.ToList().DefaultIfEmpty()
                       join accInfo in db.tbl_AccountInfos.ToList().DefaultIfEmpty()
                       on acc.ID equals accInfo.UID
                       select new AccountInfo() 
                       {
                            ID = acc.ID,
                            UserID = acc.UserID,
                            UserName = accInfo.FirstName + " " + accInfo.LastName,
                            BirthDay = accInfo.BirthDay,
                            Gender = accInfo.Gender,
                            Email = accInfo.Email,
                            Phone = accInfo.Phone,
                            IMG = accInfo.IMG
                       }
                       ).ToList();
                return lst; 
            }
        }
    }
}
