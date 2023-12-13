using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using Fasterflect;
using KGQT.Base;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;

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
                        item.DateExpectation = CaclDateExpectation(item);
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

        public static DateTime CaclDateExpectation(tbl_Package item)
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

        public static DataReturnModel<object> CreateWithFileExcel(IFormFile file, string name, string accesser)
        {
            var data = new DataReturnModel<object>();
            List<string> err = new List<string>();
            List<string> exist = new List<string>();
            using (Stream stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = true,
                        ConfigureDataTable = (data) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTableCollection table = result.Tables;
                    DataTable dt = table[name];
                    if (dt == null)
                        dt = table[table.Count - 1];
                    if (dt != null)
                    {

                        var rowCount = dt.Rows.Count;
                        int success = 0;
                        for (int row = 0; row < rowCount; row++)
                        {
                            var p = new tbl_Package();
                            var date = Converted.ToDate(dt.Rows[row][0].ToString());
                            string customer = dt.Rows[row][1].ToString();
                            int type = Converted.ToInt(dt.Rows[row][2].ToString());
                            string code = dt.Rows[row][3].ToString();
                            string note = dt.Rows[row][5].ToString();

                            if (string.IsNullOrEmpty(code)) continue;

                            if (code.IndexOf("-") > 0) continue;
                            if (BusinessBase.Exist<tbl_Package>(x => x.PackageCode == code))
                            {
                                exist.Add(code);
                                continue;
                            }

                            var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == customer);
                            if (user != null)
                            {
                                p.Username = user.Username;
                                p.UID = user.ID;
                            }

                            if (type == 3)
                                p.MovingMethod = 1;
                            else if (type == 5)
                                p.MovingMethod = 2;

                            p.PackageCode = code;
                            p.Note = note;
                            p.CreatedDate = date;
                            p.CreatedBy = accesser;
                            if (!BusinessBase.Add(p))
                            {
                                err.Add(code);
                            }
                            else
                                success++;
                        }

                        if (success > 0)
                        {
                            string sSuccess = string.Format("Đã thêm thành công {0} trong tổng số {1}", success, rowCount);
                            data.IsError = false;
                            data.Data = new { success = sSuccess, exist = string.Join("; ", exist), error = string.Join("; ", err) };
                            return data;
                        }
                        else
                        {
                            data.IsError = true;
                            data.Message = "Thêm mới không thành công !";
                            return data;
                        }
                    }
                    data.IsError = true;
                    data.Message = "Không đọc được thông tin file";
                    return data;
                }
            }
        }
        public static DataReturnModel<string> ExportChinaWareHouse(IFormFile file, string name, string accesser)
        {
            var data = new DataReturnModel<string>();
            List<tempExportChina> lstTemp = new List<tempExportChina>();
            using (Stream stream = file.OpenReadStream())
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        UseColumnDataType = true,
                        ConfigureDataTable = (data) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    DataTableCollection table = result.Tables;
                    DataTable dt = table[name];
                    if (dt == null)
                        dt = table[table.Count - 1];
                    if (dt != null)
                    {
                        var rows = dt.Rows.Count;
                        var colums = 2;
                        for (int col = 0; col < colums; col++)
                        {
                            int date = 1;
                            if (col == 0)
                                date = 3;
                            else
                                date = 5;
                            for (int row = 0; row < rows; row++)
                            {
                                lstTemp.Add(new tempExportChina
                                {
                                    Column = col,
                                    Row = row,
                                    MovingMethod = col + 1,
                                    Date = date,
                                    Value = dt.Rows[row][col].ToString()
                                });
                            }
                        }
                    }
                }
            }
            if (lstTemp.Count > 0)
            {
                var codes = lstTemp.Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => x.Value).ToList();
                if (codes.Count > 0)
                {
                    using (var db = new nhanshiphangContext())
                    {
                        var lstPackage = db.tbl_Packages.Where(x => codes.Contains(x.PackageCode) && x.Exported != true).ToList();
                        if (lstPackage.Count > 0)
                        {
                            int i = 0;
                            foreach (var item in lstPackage)
                            {
                                var temp = lstTemp.FirstOrDefault(x => x.Value == item.PackageCode);
                                var dt = DateTime.Now;
                                item.Status = 3;
                                item.MovingMethod = temp.MovingMethod;
                                item.ExportedCNWH = dt;
                                item.DateExpectation = dt.AddDays(temp.MovingMethod);
                                item.Exported = true;
                                item.ModifiedBy = accesser;
                                item.ModifiedDate = DateTime.Now;
                                db.Update(item);

                                temp.Mapped = true;
                                lstTemp[i] = temp;
                                i++;
                            }
                        }
                        var s = db.SaveChanges() > 0;
                        if (s)
                        {
                            string fileName = PJUtils.MarkupValueExcel(file, name, lstTemp.Where(x => x.Mapped).ToList());
                            data.IsError = false;
                            data.Message = "Cập nhật thành công, bạn có muốn tải file đối chiếu?";
                            data.Data = fileName;
                            return data;
                        }
                    }
                }
            }
            else
            {
                data.IsError = true;
                data.Message = "Mã vận đơn đã được cập nhật trước đó !";
                return data;
            }
            data.IsError = true;
            data.Message = "Không đọc được thông tin file";
            return data;
        }
        #endregion
    }
}
