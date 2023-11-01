using KGQT.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        public static int UpdateStatusCNWH(List<ExcelModel> model, int status, string userName)
        {
            Log.Info("model", JsonConvert.SerializeObject(model));

            if (model.Count == 0) return -1;
            using (var db = new nhanshiphangContext())
            {
                List<string> ids = model.Select(x => x.Key).ToList();
                List<tbl_Package> lst = db.tbl_Packages.Where(x => ids.Contains(x.PackageCode) && x.Exported == false).ToList();
                Log.Info("List<tbl_Package>", JsonConvert.SerializeObject(lst));
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
