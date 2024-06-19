using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public class BigPackageBusiness
    {
        #region Get
        public static object[] GetPage(string code, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {

            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_BigPackage>();
                int totalPage = 1;
                int total = 0;
                IQueryable<tbl_BigPackage> query = db.tbl_BigPackages;

                if (!string.IsNullOrEmpty(code))
                    query = query.Where(x => x.BigPackageCode == code);

                if (fromDate != null)
                    query = query.Where(x => x.CreatedDate >= fromDate);

                if (toDate != null)
                {
                    toDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(x => x.CreatedDate < toDate);
                }

                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    lstData = query.OrderByDescending(x => x.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize).ToList();
                }

                return new object[] { lstData, total, totalPage };
            }
        }

        public static bool Delete(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                var p = db.tbl_BigPackages.FirstOrDefault(x => x.ID == id);
                db.tbl_BigPackages.Remove(p);
                return db.SaveChanges() > 0;
            }
        }

        public static DataReturnModel<bool> DeleteAllPack(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                var dt = new DataReturnModel<bool>();
                bool exist = db.tbl_Packages.Any(x => x.Status == 5);
                if (exist)
                {
                    dt.IsError = true;
                    dt.Message = "Có kiện đã được nhận không thể xóa";
                    return dt;
                }

                var p = db.tbl_BigPackages.FirstOrDefault(x => x.ID == id);
                db.RemoveRange(db.tbl_Packages.Where(x => x.BigPackage == id));
                db.tbl_BigPackages.Remove(p);
                if( db.SaveChanges() > 0)
                {
                    dt.IsError = false;
                    dt.Message = "Xóa thành công";
                    return dt;
                }
                dt.IsError = true;
                dt.Message = "Xóa không thành công, vui lòng thử lại sau";
                return dt;
            }
        }
        #endregion
    }
}
