using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public static class Packages
    {
        public static bool CheckExist(string package)
        {
            using (var db = new nhanshiphangContext())
                return db.tbl_Packages.Any(x => x.PackageCode == package);
        }

        public static List<tbl_Package> GetPage(string username = "")
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_Package> qry = db.tbl_Packages;
                if (!string.IsNullOrEmpty(username))
                {
                    qry = qry.Where(x => x.Username == username);
                }
                return qry.ToList();
            }
        }


        #region CRUD
        public static bool UpdateStatusCNWH(List<ExcelModel> model, int status, string userName)
        {
            if (model.Count == 0) return false;
            using (var db = new nhanshiphangContext())
            {
                List<string> ids = model.Select(x => x.Key).ToList();
                var lst = db.tbl_Packages.Where(x => ids.Contains(x.PackageCode)).ToList();
                foreach (var item in lst)
                {
                    item.Status = status;
                    if (status == 1)
                    {
                        item.ComfirmDate = DateTime.Now;
                    }
                    else
                    {
                        var exportDate = model.FirstOrDefault(x => x.Key == item.PackageCode).Value;
                        if (exportDate != null)
                        {
                            var dt = DateTime.Parse(exportDate);
                            item.ExportedCNWH = dt;
                            if(item.MovingMethod == 1)
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
                        }
                    }
                    item.ModifiedBy = userName;
                    item.ModifiedDate = DateTime.Now;
                    db.Update(item);
                }
                return db.SaveChanges() > 0;
            }
        }
        #endregion
    }
}
