using KGQT.Models;

namespace KGQT.Business
{
    public class BigPackage
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
                    query = query.Where(x => x.CreatedDate <= toDate);

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

        #endregion
    }
}
