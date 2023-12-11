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
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using System.Globalization;

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

        public static DataReturnModel CreateWithFileExcel(IFormFile file, string name, string accesser)
        {
            var data = new DataReturnModel();
            Stream FileStream = file.OpenReadStream();
            List<ExcelModel> oData = new List<ExcelModel>();
            using (ExcelPackage pack = new ExcelPackage(FileStream))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = null;
                if (!string.IsNullOrEmpty(name))
                    worksheet = (ExcelWorksheet)pack.Workbook.Worksheets.GetIndexer(new object[] { name });
                else
                    worksheet = pack.Workbook.Worksheets.LastOrDefault();

                if (worksheet == null)
                {
                    data.IsError = true;
                    data.Message = "Không tìm thấy sheet";
                    return data;
                }
                else
                {
                    var rowCount = worksheet.Dimension.Rows;
                    var columnCount = worksheet.Dimension.Columns;
                    for (int row = 1; row < rowCount; row++)
                    {
                        var p = new tbl_Package();
                        for (int col = 0; col < columnCount; col++)
                        {
                            var v = worksheet.Cells[row, col].Value;
                            if (v != null)
                            {
                                switch (row)
                                {
                                    case 0:
                                        p.CreatedDate = DateTime.Parse(v.ToString());
                                        break;
                                    case 1:
                                        var user = BusinessBase.GetOne<tbl_Account>(x => x.Username == v.ToString());
                                        if (user != null)
                                        {
                                            p.Username = user.Username;
                                            p.UID = user.ID;
                                        }
                                        break;
                                    case 2:
                                        if (Converted.ToInt(v.ToString()) == 3)
                                            p.MovingMethod = 1;
                                        else
                                            p.MovingMethod = 2;
                                        break;
                                    case 3:
                                        p.PackageCode = v.ToString();
                                        break;
                                    case 4:
                                        break;
                                    case 5:
                                        p.Note = v.ToString();
                                        break;
                                }
                            }

                            if (string.IsNullOrEmpty(p.PackageCode)) continue;
                            p.CreatedBy = accesser;
                            BusinessBase.Add(p);
                        }
                    }
                }
                return data;
            }
        }
        public static DataReturnModel CreateWithFileExcel2(IFormFile file, string name, string accesser)
        {
            var data = new DataReturnModel();
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
                        for (int row = 0; row < rowCount; row++)
                        {
                            var p = new tbl_Package();
                            var date = Converted.ToDate(dt.Rows[row][0].ToString());
                            string customer = dt.Rows[row][1].ToString();
                            int type = Converted.ToInt(dt.Rows[row][2].ToString());
                            string code = dt.Rows[row][3].ToString();
                            string note = dt.Rows[row][5].ToString();

                            if (string.IsNullOrEmpty(code)) continue;

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
                        }

                    }
                    return data;
                }
            }
        }

        public static DataReturnModel CreateWithFileCsv(IFormFile file)
        {
            var data = new DataReturnModel();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
            };

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<CsvPackageImportMap>();
                    try
                    {
                        var dt = csv.GetRecords<CsvPackageImport>().ToList();
                    }
                    catch (Exception ex) { }
                }
            };
            return data;
        }
        #endregion
    }
}
