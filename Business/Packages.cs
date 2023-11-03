using KGQT.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;

namespace KGQT.Business
{
    public static class Packages
    {
        public static bool CheckExist(string package)
        {
            using (var db = new nhanshiphangContext())
                return db.tbl_Packages.Any(x => x.PackageCode == package);
        }

        public static object[] GetPage(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 2)
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_Package>();
                int count = 0;
                int totalPage = 0;
                IQueryable<tbl_Package> qry = db.tbl_Packages;
                if(status > 0)
                    qry = qry.Where(x => x.Status == status);
                if (!string.IsNullOrEmpty(ID))
                    qry = qry.Where(x => x.PackageCode == ID);
                if (fromDate != null)
                    qry = qry.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                    qry = qry.Where(x => x.CreatedDate <= toDate);

                count = qry.Count();
                if(count > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    lstData = qry.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }    
                return new object[] {lstData, count, totalPage};
            }
        }


        #region CRUD
        public static int UpdateStatusCNWH(List<ExcelModel> model, int status, string userName)
        {
            if (model.Count == 0) return -1;
            using (var db = new nhanshiphangContext())
            {
                List<string> ids = model.Select(x => x.Key).ToList();
                List<tbl_Package> lst = db.tbl_Packages.Where(x => ids.Contains(x.PackageCode) && x.Exported == false).ToList();
                Log.Info("Cập nhật xuất kho China","Danh sách kiện khớp với file :"+ JsonConvert.SerializeObject(lst));
                if (lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        item.Status = status;
                        var dt = DateTime.Now;
                        item.ExportedCNWH = dt;
                        if (item.MovingMethod == 1)
                        {
                            item.DateExpectation = dt.AddDays(6);
                        }

                        if (item.MovingMethod == 2)
                        {
                            item.DateExpectation = dt.AddDays(10);
                        }

                        if (item.MovingMethod == 3)
                        {
                            item.DateExpectation = dt.AddDays(15);
                        }
                        item.Exported = true;
                        item.ModifiedBy = userName;
                        item.ModifiedDate = DateTime.Now;
                        db.Update(item);
                    }
                    return db.SaveChanges();
                }
                return 0;
            }
        }
        #endregion
    }
}
