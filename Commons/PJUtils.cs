﻿using System.Text;
using System.Security.Cryptography;
using KGQT.Models.temp;
using ExcelDataReader;
using System.Data;
using System.Linq;
using Fasterflect;
using OfficeOpenXml;
using System.Security.Policy;
using System.Drawing;
using KGQT.Base;
using Newtonsoft.Json;
using System.Globalization;

namespace KGQT.Commons
{
    public class PJUtils
    {
        public static Dictionary<int, UserLogin> userLogins = new Dictionary<int, UserLogin>();
        public static string Encrypt(string key, string data)
        {
            data = data.Trim();
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new
            MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            tripdes.GenerateIV();
            MemoryStream ms = new MemoryStream();
            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(),
            CryptoStreamMode.Write);
            encStream.Write(Encoding.ASCII.GetBytes(data), 0,
            Encoding.ASCII.GetByteCount(data));
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            ms.Close();
            encStream.Close();
            return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0)).Trim();
        }
        public static string Decrypt(string key, string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";
            byte[] keydata = System.Text.Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new
            MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            byte[] cryptByte = Convert.FromBase64String(data);
            MemoryStream ms = new MemoryStream(cryptByte, 0, cryptByte.Length);
            ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();
            CryptoStream decStream = new CryptoStream(ms, cryptoTransform,
            CryptoStreamMode.Read);
            StreamReader read = new StreamReader(decStream);
            return (read.ReadToEnd());
        }
        public static bool ConvertStringToBool(string i)
        {
            i = i.ToLower();
            if (i == "1" || i == "true")
                return true;
            return false;
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};

            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public static bool CheckUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
            for (int i = 0; i < arr1.Length; i++)
            {
                if (text.Contains(arr1[i]))
                    return true;
            }
            return false;
        }

        public static string WareHouse(string ware)
        {
            if (ware == "1") return "Đông Quản";
            if (ware == "2") return "Phật Sơn";
            return "";
        }

        public static string ShippingMethodName(int status)
        {
            string text = "Nhanh";
            switch (status)
            {
                case 2:
                    text = "Chậm";
                    break;
                case 3:
                    text = "Bộ";
                    break;
                case 4:
                    text = "Bộ lô";
                    break;
                case 5:
                    text = "Biển";
                    break;
                default:
                    text = "Nhanh";
                    break;
            }
            return text;
        }

        public static string PackageStatusHtml(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 0:
                case 1:
                    sReturn = "<span class=\"btn-sm btn bg-secondary text-white\" style=\"font-size:12px\">Chưa xác nhận</span>";
                    break;
                case 2:
                    sReturn = "<span class=\"text-white btn btn-sm bg-warning\" style=\"font-size:12px\">Hàng về kho TQ</span>";
                    break;
                case 3:
                    sReturn = "<span class=\"text-white btn btn-sm bg-info\" style=\"font-size:12px\">Đang về HCM</span>";
                    break;
                case 4:
                    sReturn = "<span class=\"text-white btn btn-sm bg-primary\" style=\"font-size:12px\">Hàng về tới HCM</span>";
                    break;
                case 5:
                    sReturn = "<span class=\"text-white btn btn-sm bg-success\" style=\"font-size:12px\">Đã nhận hàng</span>";
                    break;
                case 9:
                    sReturn = "<span class=\"text-white btn btn-sm bg-danger\" style=\"font-size:12px\">Đã hủy</span>";
                    break;
                case 10:
                    sReturn = "<span class=\"text-white btn btn-sm bg-light\" style=\"font-size:12px\">Thất lạc</span>";
                    break;
                case 11:
                    sReturn = "<span class=\"text-white btn btn-sm bg-dark \" style=\"font-size:12px\">Không nhận được hàng</span>";
                    break;
            }
            return sReturn;
        }

        public static string TradeName(int type)
        {
            string name = "";
            switch (type)
            {
                case 1:
                    name = "Thanh đoán đơn hàng {0}";
                    break;
                case 2:
                    break;
            }
            return name;
        }

        public static string PackageStatus(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 0:
                case 1:
                    sReturn = "Chưa xác nhận";
                    break;
                case 2:
                    sReturn = "Hàng về kho TQ";
                    break;
                case 3:
                    sReturn = "Đang trên đường về HCM";
                    break;
                case 4:
                    sReturn = "Hàng về tới HCM";
                    break;
                case 5:
                    sReturn = "Đã nhận hàng";
                    break;
                case 9:
                    sReturn = "Đã hủy";
                    break;
                case 10:
                    sReturn = "Thất lạc";
                    break;
                case 11:
                    sReturn = "Không nhận được hàng";
                    break;
            }
            return sReturn;
        }

        public static string ShippingOrderStatusHtml(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 1:
                    sReturn = "<span class=\"text-white badge-pill btn-sm badge-warning\">Chờ thanh toán</span>";
                    break;
                case 2:
                    sReturn = "<span class=\"text-white badge-pill btn-sm bg-success\">Đã thanh toán</span>";
                    break;
                case 3:
                    sReturn = "<span class=\"text-white badge-pill btn-sm bg-danger\">Đã hủy</span>";
                    break;
                case 4:
                    sReturn = "<span class=\"text-white badge-pill btn-sm bg-info\">Khác</span>";
                    break;
            }
            return sReturn;
        }

        public static string WithDrawStatusHtml(int type)
        {
            string sReturn = "";
            switch (type)
            {
                case 1:
                    sReturn = "<span class=\"text-white badge-pill btn-sm badge-success\">Nạp tiền</span>";
                    break;
                case 2:
                    sReturn = "<span class=\"text-white badge-pill btn-sm bg-danger\">Rút tiền</span>";
                    break;
                case 3:
                    sReturn = "<span class=\"text-white badge-pill btn-sm bg-danger\">Đã hủy</span>";
                    break;
                case 4:
                    sReturn = "<span class=\"text-white badge-pill btn-sm bg-info\">Khác</span>";
                    break;
            }
            return sReturn;
        }

        public static string ShippingOrderStatus(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 1:
                    sReturn = "Chờ thanh toán";
                    break;
                case 2:
                    sReturn = "Đã thanh toán";
                    break;
                case 3:
                    sReturn = "Đã hủy";
                    break;
                case 4:
                    sReturn = "Khác";
                    break;
            }
            return sReturn;
        }

        public static string MovingMethod2Type(int method)
        {
            string type = "3";
            switch (method)
            {
                case 2:
                    type = "5";
                    break;
                case 3:
                    type = "8";
                    break;
                default:
                    type = "3";
                    break;
            }

            return type;
        }

        public static string TradeType(int type)
        {
            string s = "";
            switch (type)
            {
                case 1:
                    s = "Thanh toán đơn hàng";
                    break;
                case 2:
                    s = "Hoàn tiền";
                    break;
                case 3:
                    s = "Nạp tiền";
                    break;
                case 4:
                    s = "Rút tiền";
                    break;
                case 5:
                    s = "Hủy rút tiền";
                    break;
                case 6:
                    s = "Nạp tiền tại kho";
                    break;
                case 7:
                    s = "Rút tiền tại kho";
                    break;
            }
            return s;
        }

        public static List<ExcelModel> ReadExcelToJson(IFormFile file, string name)
        {
            if (file.ContentType == "application/vnd.ms-excel")
                return ReadXls2Json(file, name);
            else
            {
                Stream FileStream = file.OpenReadStream();
                List<ExcelModel> oData = new List<ExcelModel>();
                using (ExcelPackage pack = new ExcelPackage(FileStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = null;
                    if (!string.IsNullOrEmpty(name))
                        worksheet = (ExcelWorksheet)pack.Workbook.Worksheets.GetIndexer(new object[] { name });
                    else
                        worksheet = (ExcelWorksheet)pack.Workbook.Worksheets.LastOrDefault();

                    if (worksheet == null)
                    {
                        return null;
                    }
                    else
                    {
                        var rowCount = worksheet.Dimension.End.Row;
                        var columnCount = worksheet.Dimension.End.Column;
                        for (int column = 1; column < columnCount; column++)
                        {
                            for (int row = 2; row < rowCount; row++)
                            {
                                var v = worksheet.Cells[row, column].Value;
                                if (v != null)
                                    oData.Add(new ExcelModel()
                                    {
                                        Key = v.ToString()
                                    });
                            }
                        }
                    }
                }
                return oData;
            }
        }

        public static List<ExcelModel> ReadXls2Json(IFormFile file, string name)
        {
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
                        List<ExcelModel> oData = new List<ExcelModel>();
                        var rowCount = dt.Rows.Count;
                        var columnCount = dt.Columns.Count;
                        for (int column = 0; column < columnCount; column++)
                        {
                            for (int row = 0; row < rowCount; row++)
                            {
                                var v = dt.Rows[row].Field<string>(column);
                                if (v != null)
                                    oData.Add(new ExcelModel()
                                    {
                                        Key = v.ToString()
                                    });
                            }
                        }
                        return oData;
                    }
                    return null;
                }
            }
        }

        public static string MarkupValueExcel(IFormFile file, string name, List<tempExport> data)
        {
            if (file == null || data == null || data.Count == 0) return "";
            string fileName = file.FileName;
            try
            {
                using Stream FileStream = file.OpenReadStream();
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
                        return "";
                    }
                    else
                    {
                        #region Read value
                        foreach (var item in data)
                        {
                            worksheet.Cells[item.Row + 2, item.Column + 1].Style.Fill.SetBackground(Color.Green);
                        }
                        #endregion

                        #region save file was edit
                        try
                        {
                            string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                            pathRoot = Path.Combine(pathRoot, "ExportExcel", DateTime.Now.ToString("ddMMyyyy"));
                            if (!Directory.Exists(pathRoot))
                                Directory.CreateDirectory(pathRoot);

                            string sheet = worksheet.Name;
                            if (!string.IsNullOrEmpty(sheet))
                            {
                                var splitName = fileName.Split('.');

                                if (splitName.Length > 1)
                                {
                                    fileName = splitName[0] + "_" + sheet + "." + splitName[1];
                                }
                                else
                                    fileName = fileName + "_" + sheet;
                            }
                            pathRoot = pathRoot + "/" + fileName;

                            pack.SaveAs(pathRoot);
                            byte[] fileBytes = System.IO.File.ReadAllBytes(pathRoot);
                            return pathRoot;
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Export file excel", JsonConvert.SerializeObject(ex));
                            return "";
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #region Kiểm tra ngày lễ or chủ nhật
        public static bool IsHoliday(DateTime date)
        {
            // 1-1;30-4;1-5;2-9
            string ddmm = date.Day + "/" + date.Month;
            switch (ddmm)
            {
                case "1/1":
                case "30/4":
                case "1/5":
                case "2/9":
                    return true;
            }
            return false;
        }
        public static bool IsSunDay(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday)
                return true;
            return false;
        }

        public static DateTime LunnarDay(DateTime date)
        {
            DateTime fromDate = new DateTime(date.AddYears(-1).Year, 12, 28, new VietnameseCalendar());
            DateTime toDate = new DateTime(date.Year, 01, 06, new VietnameseCalendar());
            if (fromDate <= date && date <= toDate)
            {
                var subDays = toDate.Subtract(date).TotalDays;
                if (subDays > 0)
                    subDays += 1; // Dự trù thêm 1 ngày
                date = date.AddDays(subDays);
            }
            return date;

        }
        public static DateTime GetDeliveryDate(DateTime date, int type = 0)
        {
            switch (type)
            {
                case 1: // nhanh từ 3-6 ngày
                    date = date.AddDays(4);
                    break;
                case 2: // thường từ 5-10 ngày
                    date = date.AddDays(7);
                    break;
                case 3: // bộ
                    date = date.AddDays(11);
                    break;
                case 4: // lô
                    date = date.AddDays(20);
                    break;
            }

            bool isHoliday = IsHoliday(date);
            while (isHoliday)
            {
                date = date.AddDays(1);
                isHoliday = IsHoliday(date);
            }

            if (date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(1);


            if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1);

            date = LunnarDay(date);

            return date;
        }
        #endregion
    }
}