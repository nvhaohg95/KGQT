using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using KGQT.Models.temp;
using System.Xml;
using Supremes;
using System.Drawing;

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
                case 1:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-gray text-white\">Đã cập nhật MVĐ</div>";
                    break;
                case 2:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-yellow-l1 text-white\">Hàng về kho TQ</div>";
                    break;
                case 3:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-yellow-l2 text-white\">Đang trên đường về HCM</div>";
                    break;
                case 4:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-warning text-white\">Hàng về tới HCM</div>";
                    break;
                case 5:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-success text-white\">Đã nhận hàng</div>";
                    break;
                case 6:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-danger text-white\">Đã hủy</div>";
                    break;
                case 7:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-light text-white\">Thất lạc</div>";
                    break;
                case 8:
                    sReturn = "<div class=\"text-center p-1 mb-2 bg-light text-white\">Không nhận được hàng</div>";
                    break;
            }
            return sReturn;
        }
        public static string ShippingStatus(int status)
        {
            string sReturn = "";
            switch (status)
            {
                case 1:
                    sReturn = "Đã cập nhật MVĐ";
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
                case 6:
                    sReturn = ">Đã hủy";
                    break;
                case 7:
                    sReturn = "Thất lạc";
                    break;
                case 8:
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
    }
}
