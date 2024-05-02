using DocumentFormat.OpenXml.Office2010.Excel;
using KGQT.Models;
using System.Linq;

namespace KGQT.Business
{
    public static class FeeWeightBusiness
    {
        #region Get Page
        public static object[] GetPage(int type,int page, int pageSize = 10)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_FeeWeight>();
                int total = 0;
                int totalPage = 0;
                IQueryable<tbl_FeeWeight> query = db.tbl_FeeWeights;
                if(type > 0)
                    query = query.Where(x => x.Type == type);
                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    int pageNum = (page - 1) * pageSize;
                    lstData = query.OrderBy(x => x.CreatedDate)
                        .Skip(pageNum)
                        .Take(pageSize)
                        .ToList();
                }    
                return new object[] { lstData , total, totalPage };
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

        #region Get List
        public static List<tbl_FeeWeight> GetList(int type)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_FeeWeight>();
                var query = db.tbl_FeeWeights.Where(x => x.Type == type);
                var total = query.Count();  
                if(total > 0)
                {
                    lstData = query.OrderBy(x => x.WeightFrom).ToList();
                }  
                return lstData;
            }
        }
        #endregion

        #region Delete 
        public static bool Delete(int ID)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = db.tbl_FeeWeights.FirstOrDefault(x => x.ID == ID);
                if(data != null)
                {
                    db.Remove(data);
                    int kq = db.SaveChanges();
                    if(kq > 0)
                    {
                        return true;
                    }
                }    
            }
            return false;
        }
        #endregion

        #region Get By ID
        public static tbl_FeeWeight? GetByID(int ID)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = db.tbl_FeeWeights.FirstOrDefault(x => x.ID == ID);
                return data;
            }
        }
        #endregion

        #region Update
        public static bool Update(tbl_FeeWeight model, string modifiedBy)
        {
            using (var db = new nhanshiphangContext())
            {
                var data = db.tbl_FeeWeights.FirstOrDefault(x => x.ID == model.ID);
                if(data != null)
                {
                    data.Type = model.Type;
                    data.WeightFrom = model.WeightFrom;
                    data.WeightTo = model.WeightTo;
                    data.Amount = model.Amount;
                    data.ModifiedBy = modifiedBy;
                    data.ModifiedDate = DateTime.Now;
                    db.Update(data);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}
