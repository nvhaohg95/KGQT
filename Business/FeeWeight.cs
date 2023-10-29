using KGQT.Models;

namespace KGQT.Business
{
    public static class FeeWeight
    {
        #region Get List FeeWeight
        public static object[] GetList(int page = 1, int pageSize = 20)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_FeeWeight>();
                decimal total = 0;
                decimal totalPage = 0;

                int pageNum = page - 1;
                IQueryable<tbl_FeeWeight> query = db.tbl_FeeWeights;
                total = query.Count();
                if (total > 0)
                {
                    query = query.OrderByDescending(x => x.CreatedDate)
                    .Skip(pageNum * pageSize)
                    .Take(pageSize);
                    lstData = query.ToList();
                    totalPage = Math.Ceiling(total / pageSize);
                }    
                    
                return new object[] {lstData , total, totalPage };
            }
        }
        #endregion
        #region Get Detail FeeWeight
        public static tbl_FeeWeight Get(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_FeeWeights.FirstOrDefault(x => x.ID == id);
            }
        }
        #endregion
    }
}
