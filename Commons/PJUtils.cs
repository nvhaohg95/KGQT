using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using KGQT.Models.temp;
using System.Xml;
using Supremes;
using System.Drawing;
using ExcelDataReader;
using System.Data;
using System.Reflection;

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

        public static string ShippingMethodName(int status)
        {
            if (status == 1) return "Nhanh";
            if (status == 2) return "Chậm";
            if (status == 3) return "Chính ngạch";
            return "";
        }

        public static string ShippingStatusHtml(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 0:
                    sReturn = "<span class=\"badge badge-pill badge-secondary p-2\">Chưa xác nhận</span>";
                    break;
                case 1:
                    sReturn = "<span class=\"badge badge-pill badge-info p-2\">Đã cập nhật MVĐ</span>";
                    break;
                case 2:
                    sReturn = "<span class=\"badge badge-pill bg-yellow-l1 p-2\">Hàng về kho TQ</span>";
                    break;
                case 3:
                    sReturn = "<span class=\"badge badge-pill bg-yellow-l2 p-2\">Đang trên đường về HCM</span>";
                    break;
                case 4:
                    sReturn = "<span class=\"badge badge-pill badge-warning p-2\">Hàng về tới HCM</span>";
                    break;
                case 5:
                    sReturn = "<span class=\"badge badge-pill badge-success p-2\">Đã nhận hàng</span>";
                    break;
                case 9:
                    sReturn = "<span class=\"badge badge-pill badge-danger p-2\">Đã hủy</span>";
                    break;
                case 10:
                    sReturn = "<span class=\"badge badge-pill badge-light p-2\">Thất lạc</span>";
                    break;
                case 11:
                    sReturn = "<span class=\"badge badge-pill badge-dark p-2\">Không nhận được hàng</span>";
                    break;
            }
            return sReturn;
        }

        public static string ShippingStatus(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 0:
                    sReturn = "Chưa xác nhận";
                    break;
                case 1:
                    sReturn = "Đã cập nhật MVĐ";
                    break;
                //case 2:
                //    sReturn = "Hàng về kho TQ";
                //    break;
                case 2:
                    sReturn = "Đang trên đường về HCM";
                    break;
                case 3:
                    sReturn = "Hàng về tới HCM";
                    break;
                case 4:
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

        public static string PackageStatus(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 1:
                    sReturn = "Vận chuyển quốc tế";
                    break;
                case 2:
                    sReturn = "Chờ giao";
                    break;
                case 3:
                    sReturn = "Đang giao";
                    break;
                case 4:
                    sReturn = "Đã nhận hàng";
                    break;
            }
            return sReturn;
        }

        public static List<ExcelModel> ExcelToJson(IFormFile file)
        {
            try
            {
                List<ExcelModel> oData = new List<ExcelModel>();

                #region Variable Declaration
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader excelReader = null;
                Stream FileStream = null;
                var json = "";

                FileStream = file.OpenReadStream();

                if (FileStream != null)
                {
                    excelReader = ExcelReaderFactory.CreateReader(FileStream);
                }

                if (excelReader != null)
                {
                    DataSet result = excelReader.AsDataSet();
                    if (result != null && result.Tables.Count > 0)
                    {
                        foreach (DataTable dtt in result.Tables)
                        {
                            var dt = dtt;
                            //dt.Columns["Column"].ColumnName = "ID";
                            dt.Rows.RemoveAt(0);

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var obj = new ExcelModel();
                                var count = dt.Rows[i].ItemArray.Count();
                                if (count > 1)
                                {
                                    obj.Key = dt.Rows[i][0].ToString();
                                    obj.Value = dt.Rows[i][1].ToString();

                                }
                                else
                                {
                                    obj.Key = dt.Rows[i][0].ToString();
                                }
                                oData.Add(obj);
                            }
                        }
                    }
                    return oData;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
            #endregion
            return null;
        }
    }
}
