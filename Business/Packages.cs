﻿using KGQT.Base;
using KGQT.Models;
using KGQT.Models.temp;
using Newtonsoft.Json;

namespace KGQT.Business
{
    public static class Packages
    {
        #region Select
        public static object[] GetAllStatus(string username)
        {
            using (var db = new nhanshiphangContext())
            {
                IQueryable<tbl_Package> qry = db.tbl_Packages;
                if (!string.IsNullOrEmpty(username))
                    qry = qry.Where(x => x.Username == username);

                int st1 = qry.Where(x => x.Status == 3).Count();
                int st2 = qry.Where(x => x.Status == 4).Count();
                int st3 = qry.Where(x => x.Status == 5).Count();
                return new object[] { st1, st2, st3 };
            }
        }
        public static bool CheckExist(string package)
        {
            using (var db = new nhanshiphangContext())
                return db.tbl_Packages.Any(x => x.PackageCode == package);
        }
        public static object[] GetPage(int status, string ID, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 2, string userName = "")
        {
            using (var db = new nhanshiphangContext())
            {
                var lstData = new List<tbl_Package>();
                int count = 0;
                int totalPage = 0;
                IQueryable<tbl_Package> qry = db.tbl_Packages;
                if (status > 0)
                    qry = qry.Where(x => x.Status == status);
                if (!string.IsNullOrEmpty(ID))
                    qry = qry.Where(x => x.PackageCode.Contains(ID));
                if (fromDate != null)
                    qry = qry.Where(x => x.CreatedDate >= fromDate);
                if (toDate != null)
                    qry = qry.Where(x => x.CreatedDate <= toDate);
                if (!string.IsNullOrEmpty(userName))
                    qry = qry.Where(x => x.Username == userName);
                count = qry.Count();
                if (count > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)count / pageSize));
                    lstData = qry.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return new object[] { lstData, count, totalPage };
            }
        }
        #endregion

        #region CRUD
        public static object[] UpdateStatusCNWH(List<ExcelModel> model, int status, string userName)
        {
            if (model.Count == 0) return new object[] { -1 };
            using (var db = new nhanshiphangContext())
            {
                List<string> ids = model.Select(x => x.Key).ToList();
                List<tbl_Package> lst = db.tbl_Packages.Where(x => ids.Contains(x.PackageCode) && x.Exported == false).ToList();
                Log.Info("Cập nhật xuất kho China", "Danh sách kiện khớp với file :" + JsonConvert.SerializeObject(lst));
                if (lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        var dt = DateTime.Now;
                        item.Status = status;
                        item.ExportedCNWH = dt;
                        item.DateExpectation = UpdateExp(item);
                        item.Exported = true;
                        item.ModifiedBy = userName;
                        item.ModifiedDate = DateTime.Now;
                        db.Update(item);
                    }
                    var s = db.SaveChanges() > 0;
                    if (s)
                    {
                        return new object[] { 1, lst.Select(x => x.PackageCode).ToList() };
                    }
                }
                return new object[] { 0 };
            }
        }

        public static DateTime UpdateExp(tbl_Package item)
        {
            var dt = DateTime.Now;

            if (item.MovingMethod == 1)
            {
                return dt.AddDays(6);
            }

            if (item.MovingMethod == 2)
            {
                return dt.AddDays(10);
            }

            if (item.MovingMethod == 3)
            {
                return dt.AddDays(15);
            }
            return dt;
        }
        #endregion
    }
}
