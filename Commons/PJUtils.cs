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

        public static string Decrypt(string key, string data)
        {
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

        public static bool SendMail(string strFrom, string pass, string strTo, string strSubject, string strMsg, string cc)
        {
            try
            {
                // Create the mail message
                MailMessage objMailMsg = new MailMessage(strFrom, strTo);

                objMailMsg.BodyEncoding = Encoding.UTF8;
                objMailMsg.Subject = strSubject;
                objMailMsg.CC.Add(cc);
                objMailMsg.IsBodyHtml = true;
                objMailMsg.Body = strMsg;
                SmtpClient objSMTPClient = new SmtpClient();

                objSMTPClient.Host = "202.43.110.136";
                objSMTPClient.Port = 25;
                objSMTPClient.EnableSsl = false;
                objSMTPClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                objSMTPClient.Credentials = new NetworkCredential(strFrom, pass);
                objSMTPClient.Timeout = 20000;
                objSMTPClient.Send(objMailMsg);
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SendMailGmail(string strFrom, string pass, string strTo, string strSubject, string strMsg, string cc)
        {
            try
            {
                string fromAddress = strFrom;
                string mailPassword = pass;       // Mail id password from where mail will be sent.
                string messageBody = strMsg;


                // Create smtp connection.
                SmtpClient client = new SmtpClient();
                client.Port = 587;//outgoing port for the mail.
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                client.Credentials = new System.Net.NetworkCredential(fromAddress, mailPassword);


                // Fill the mail form.
                var send_mail = new MailMessage();
                send_mail.IsBodyHtml = true;
                //address from where mail will be sent.
                send_mail.From = new MailAddress(strFrom);
                //address to which mail will be sent.           
                send_mail.To.Add(new MailAddress(strTo));
                //subject of the mail.
                send_mail.Subject = strSubject;
                send_mail.Body = messageBody;
                client.Send(send_mail);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string RemoveHTMLTags(string content)
        {
            var cleaned = string.Empty;
            try
            {
                StringBuilder textOnly = new StringBuilder();
                using (var reader = XmlNodeReader.Create(new System.IO.StringReader("<xml>" + content + "</xml>")))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Text)
                            textOnly.Append(reader.ReadContentAsString());
                    }
                }
                cleaned = textOnly.ToString();
            }
            catch
            {
                //A tag is probably not closed. fallback to regex string clean.
                string textOnly = string.Empty;
                Regex tagRemove = new Regex(@"<[^>]*(>|$)");
                Regex compressSpaces = new Regex(@"[\s\r\n]+");
                textOnly = tagRemove.Replace(content, string.Empty);
                textOnly = compressSpaces.Replace(textOnly, " ");
                cleaned = textOnly;
            }

            return cleaned;
        }

        public static string TranslateText(string input, string languagePair)
        {
            string returnstr = input;
            try
            {
                string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
                request.Method = "GET";
                var content = String.Empty;
                HttpStatusCode statusCode;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var contentType = response.ContentType;
                    Encoding encoding = null;
                    if (contentType != null)
                    {
                        var match = Regex.Match(contentType, @"(?<=charset\=).*");
                        if (match.Success)
                            encoding = Encoding.GetEncoding(match.ToString());
                    }

                    encoding = encoding ?? Encoding.UTF8;

                    statusCode = ((HttpWebResponse)response).StatusCode;
                    using (var reader = new StreamReader(stream, encoding))
                        content = reader.ReadToEnd();
                }
                var doc = Dcsoup.Parse(content);
                var scoreDiv = doc.Select("html").Select("span[id=result_box]").Html;
                returnstr = scoreDiv;
            }
            catch
            {

            }

            return returnstr;
        }

        public static string GetIcon(object o)
        {
            if (o == null)
                return "/no_thumbnails.gif";
            if (!string.IsNullOrEmpty(o.ToString()))
                return o.ToString();
            return "/no_thumbnails.gif";
        }
        public static string SubString(string title, int length)
        {
            if (string.IsNullOrEmpty(title))
                return "";

            if (!title.Contains(" "))
            {
                if (title.Length > length)
                    title = title.Substring(0, length - 1) + "...";
            }
            else if (title.Length >= length)
            {
                int i = length - 1;
                while (title.Substring(i--, 1) != " " && i > 0) ;
                if (i == 0)
                    return title.Substring(0, length - 4) + " ...";
                else
                    return title.Substring(0, i + 1) + " ...";
            }

            return title;
        }
        public static string RandomString(int numberrandom)
        {
            //var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var chars = "0123456789";
            var stringChars = new char[numberrandom];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
        public static string RandomStringWithText(int numberrandom)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            //var chars = "0123456789";
            var stringChars = new char[numberrandom];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        public static bool ConvertStringToBool(string i)
        {
            i = i.ToLower();
            if (i == "1" || i == "true")
                return true;
            return false;
        }
        public static string StatusToRequest(object i)
        {
            if (i != null)
            {
                if (i.ToString() == "1")
                {
                    return "<span class='yellow'>Chưa kích hoạt</span>";
                }
                else if (i.ToString() == "2")
                {
                    return "<span class='blue'>Đã kích hoạt</span>";
                }
                else
                {
                    return "<span class='red'>Đang bị khóa</span>";
                }

            }
            else return "<span class='red'>Đang bị khóa</span>";
        }
        public static string IntToRequestAdmin(int i)
        {
            if (i == 0)
                return "<span class=\"bg-red\">Chưa đặt cọc</span>";
            else if (i == 1)
                return "<span class=\"bg-black\">Hủy đơn hàng</span>";
            else if (i == 2)
                return "<span class=\"bg-bronze\">Đã đặt cọc</span>";
            else if (i == 3)
                return "<span class=\"bg-green\">Chờ duyệt đơn</span>";
            else if (i == 4)
                return "<span class=\"bg-green\">Đã duyệt đơn</span>";
            else if (i == 5)
                return "<span class=\"bg-green\">Đã đặt hàng</span>";
            else if (i == 6)
                return "<span class=\"bg-green\">Đã nhận hàng tại TQ</span>";
            else if (i == 7)
                return "<span class=\"bg-green\">Đã nhận hàng tại VN</span>";
            else if (i == 8)
                return "<span class=\"bg-yellow\">Chờ thanh toán</span>";
            else if (i == 9)
                return "<span class=\"bg-blue\">Khách đã thanh toán</span>";
            else if (i == 10)
                return "<span class=\"bg-blue\">Khách đã nhận hàng</span>";
            else
                return "";

        }
        public static string IntToRequestClient(int i)
        {
            //if (i == 0)
            //    return "<span class=\"bg-red\">Chưa đặt cọc</span>";
            //else if (i == 1)
            //    return "<span class=\"bg-black\">Hủy đơn hàng</span>";
            //else if (i == 2)
            //    return "<span class=\"bg-bronze\">Đã đặt cọc</span>";
            //else if (i >= 3 && i < 8)
            //    return "<span class=\"bg-green\">Đang xử lý</span>";
            //else if (i == 8)
            //    return "<span class=\"bg-yellow\">Chờ thanh toán</span>";
            //else if (i == 9)
            //    return "<span class=\"bg-blue\">Đã xong</span>";
            //else if (i == 10)
            //    return "<span class=\"bg-blue\">Đã giao hàng</span>";
            //else
            //    return "";
            if (i == 0)
                return "<span class=\"bg-red\">Chưa đặt cọc</span>";
            else if (i == 1)
                return "<span class=\"bg-black\">Hủy đơn hàng</span>";
            else if (i == 2)
                return "<span class=\"bg-bronze\">Đã đặt cọc</span>";
            else if (i == 3)
                return "<span class=\"bg-green\">Chờ duyệt đơn</span>";
            else if (i == 4)
                return "<span class=\"bg-green\">Đã duyệt đơn</span>";
            else if (i == 5)
                return "<span class=\"bg-green\">Đã đặt hàng</span>";
            else if (i == 6)
                return "<span class=\"bg-green\">Đã nhận hàng tại TQ</span>";
            else if (i == 7)
                return "<span class=\"bg-green\">Đã nhận hàng tại VN</span>";
            else if (i == 8)
                return "<span class=\"bg-yellow\">Chờ thanh toán</span>";
            else if (i == 9)
                return "<span class=\"bg-blue\">Khách đã thanh toán</span>";
            else if (i == 10)
                return "<span class=\"bg-blue\">Khách đã nhận hàng</span>";
            else
                return "";


        }
        public static string BoolToRequest(object i)
        {
            if (i != null)
            {
                return ConvertStringToBool(i.ToString()) == true ? "<span class='red'>Đang yêu cầu</span>" : "<span class='blue'>Không</span>";
            }
            else return "<span class='blue'>Không</span>";
        }
        public static string ShowStatusPayHistory(int status)
        {
            if (status == 2)
                return "<span class=\"bg-bronze\">Đặt cọc</span>";
            else if (status == 3)
                return "<span class=\"bg-yellow\">Đặt cọc</span>";
            else if (status == 12)
                return "<span class=\"bg-red\">Sản phẩm hết hàng hoặc giảm giá trả lại cọc</span>";
            else
                return "<span class=\"bg-blue\">Thanh toán</span>";
        }

        public static string BoolToStatus(string i)
        {
            return ConvertStringToBool(i) == true ? "<span class='show-stat-s'>Hiện</span>" : "<span class='show-stat-w'>Ẩn</span>";

        }
        public static string GetTradeType(int TradeType)
        {
            if (TradeType == 1)
            {
                return "Đặt cọc";
            }
            else if (TradeType == 2)
            {
                return "Hoàn tiền";
            }
            else if (TradeType == 3)
            {
                return "Thanh toán";
            }
            else if (TradeType == 4)
            {
                return "Nạp tiền";
            }
            else if (TradeType == 5)
            {
                return "Rút tiền";
            }
            else if (TradeType == 6)
            {
                return "Hủy lệnh rút tiền";
            }
            else if (TradeType == 7)
            {
                //return "Nhận lại tiền khiếu nại";
                return "Khiếu nại";
            }
            else if (TradeType == 8)
            {
                //return "Thu phí lưu kho";
                return "Lưu kho";
            }
            else if (TradeType == 9)
            {
                return "Phí giao hàng";
            }
            else if (TradeType == 10)
            {
                return "Truy thu";
            }
            else if (TradeType == 11)
            {
                return "Nạp tiền tại kho";
            }
            else if (TradeType == 12)
            {
                return "Rút tiền tại kho";
            }
            else
            {
                return "...";
            }

        }
        public static string GetHistoryPayWalletTradeType(int TradeType)
        {
            if (TradeType == 1)
            {
                return "Đặt cọc";
            }
            else if (TradeType == 2)
            {
                return "Hoàn tiền";
            }
            else if (TradeType == 3)
            {
                return "Thanh toán";
            }
            else if (TradeType == 4)
            {
                return "Nạp tiền";
            }
            else if (TradeType == 5)
            {
                return "Rút tiền";
            }
            else if (TradeType == 6)
            {
                return "Hủy lệnh rút tiền";
            }
            else if (TradeType == 7)
            {
                //return "Nhận lại tiền khiếu nại";
                return "Khiếu nại";
            }
            else if (TradeType == 8)
            {
                //return "Thu phí lưu kho";
                return "Lưu kho";
            }
            else if (TradeType == 9)
            {
                return "Phí giao hàng";
            }
            else if (TradeType == 10)
            {
                return "Truy thu";
            }
            else if (TradeType == 11)
            {
                return "Nạp tiền tại kho";
            }
            else if (TradeType == 12)
            {
                return "Rút tiền tại kho";
            }
            else
            {
                return "...";
            }


        }
        public static string BoolToStatusShow(string i)
        {
            return ConvertStringToBool(i) == true ? "<span class='show-stat-w'>Ẩn</span>" : "<span class='show-stat-s'>Hiện</span>";

        }
        public static string ReturnStatusWithdraw(int status)
        {

            if (status == 1)
            {
                return "<span class='bg-red'>Chưa duyệt</span>";
            }
            else if (status == 5)
            {
                return "<span class='bg-yellow'>Đang giải quyết</span>";
            }
            else if (status == 2)
            {
                return "<span class='bg-blue'>Thành công</span>";
            }
            else if (status == 4)
            {
                return "<span class='bg-yellow'>Đối soát 48h</span>";
            }
            else
            {
                return "<span class='bg-black'>Hủy</span>";
            }
        }
        public static string ReturnStatusHistoryExport(int status)
        {
            if (status == 1)
            {
                return "<span class='bg-red'>Chưa xử lý</span>";
            }
            else if (status == 2)
            {
                return "<span class='bg-blue'>Đã xử lý</span>";
            }
            else
            {
                return "Hủy lệnh";
            }
        }
        public static string ReturnRoleName(string name)
        {
            if (name == "Store")
            {
                return "<span class='yellow'>Cửa hàng</span>";
            }
            else if (name == "Customer")
            {
                return "<span class=''>Người dùng</span>";
            }
            return name;
        }
        public static string ReturnSymbol(int Type)
        {
            if (Type == 1)
            {
                return "-";
            }
            else
                return "+";
        }

        public static string ReturnStatusRequest(string status)
        {
            if (status == "1")
            {
                return "<span class='bg-red'>Chờ tiếp nhận</span>";
            }
            else if (status == "2")
            {
                return "<span class='bg-yellow'>Đã tiếp nhận</span>";
            }
            else if (status == "3")
            {
                return "<span class='bg-green'>Giao bù</span>";
            }
            else if (status == "4")
            {
                return "<span class='bg-orange'>Chấp nhận</span>";
            }
            else if (status == "5")
            {
                return "<span class='bg-blue'>Đã hoàn</span>";
            }
            else
            {
                return "<span class='bg-black'>Từ chối</span>";
            }
        }
        public static string ReturnStatusOrder(int status)
        {
            if (status == 0)
            {
                return "<span class='bg-black'>Đã hủy</span>";
            }
            else if (status == 1)
            {
                return "<span class='bg-red'>Chưa đặt cọc</span>";
            }
            else if (status == 2)
            {
                return "<span class='bg-blue'>Đã đặt cọc</span>";
            }
            else if (status == 3)
            {
                return "<span class='bg-orange'>Đang đặt hàng</span>";
            }
            else if (status == 4)
            {
                return "<span class='bg-yellow'>Đã đặt hàng</span>";
            }
            else if (status == 12)
            {
                return "<span class='bg-pink'>Đang về kho Sài Gòn</span>";
            }
            else if (status == 5)
            {
                return "<span class='bg-red'>Đã về kho Sài Gòn</span>";
            }
            else if (status == 6)
            {
                return "<span class='bg-bronze'>Yêu cầu giao hàng</span>";
            }
            else if (status == 13)
            {
                return "<span class='bg-bronze'>Đã giao hàng</span>";
            }
            else
            {
                return "<span class='bg-green'>Đã nhận hàng</span>";
            }
        }


        //public static string ReturnSubStatusOrder(string Value, bool isCheckProduct)
        //{
        //    string data = "";

        //    if (!string.IsNullOrEmpty(Value))
        //    {
        //        var list = Value.Split(',').ToList();
        //        for (int j = 0; j < list.Count; j++)
        //        {
        //            int status = list[j].ToInt(0);
        //            if (status == 0)
        //            {
        //                return "";
        //            }
        //            else if (status == 1)
        //            {
        //                data += "<span class='bg-red'>Chờ khách phản hồi</span></br>";
        //            }
        //            else if (status == 2)
        //            {
        //                data += "<span class='bg-bronze'>Đơn nhiều kiện</span></br>";
        //            }
        //            else if (status == 3)
        //            {
        //                if (isCheckProduct == true)
        //                    data += "<span class='bg-bronze'>Thiếu hàng</span></br>";
        //            }
        //            else if (status == 4)
        //            {
        //                data += "<span class='bg-bronze'>Sai mẫu</span></br>";
        //            }

        //        }
        //    }


        //    return data;
        //}

        //barcode 128

        private const int CQuietWidth = 10;

        private static readonly int[,] CPatterns =
                                                   {
                                                     { 2, 1, 2, 2, 2, 2, 0, 0 }, // 0
                                                     { 2, 2, 2, 1, 2, 2, 0, 0 }, // 1
                                                     { 2, 2, 2, 2, 2, 1, 0, 0 }, // 2
                                                     { 1, 2, 1, 2, 2, 3, 0, 0 }, // 3
                                                     { 1, 2, 1, 3, 2, 2, 0, 0 }, // 4
                                                     { 1, 3, 1, 2, 2, 2, 0, 0 }, // 5
                                                     { 1, 2, 2, 2, 1, 3, 0, 0 }, // 6
                                                     { 1, 2, 2, 3, 1, 2, 0, 0 }, // 7
                                                     { 1, 3, 2, 2, 1, 2, 0, 0 }, // 8
                                                     { 2, 2, 1, 2, 1, 3, 0, 0 }, // 9
                                                     { 2, 2, 1, 3, 1, 2, 0, 0 }, // 10
                                                     { 2, 3, 1, 2, 1, 2, 0, 0 }, // 11
                                                     { 1, 1, 2, 2, 3, 2, 0, 0 }, // 12
                                                     { 1, 2, 2, 1, 3, 2, 0, 0 }, // 13
                                                     { 1, 2, 2, 2, 3, 1, 0, 0 }, // 14
                                                     { 1, 1, 3, 2, 2, 2, 0, 0 }, // 15
                                                     { 1, 2, 3, 1, 2, 2, 0, 0 }, // 16
                                                     { 1, 2, 3, 2, 2, 1, 0, 0 }, // 17
                                                     { 2, 2, 3, 2, 1, 1, 0, 0 }, // 18
                                                     { 2, 2, 1, 1, 3, 2, 0, 0 }, // 19
                                                     { 2, 2, 1, 2, 3, 1, 0, 0 }, // 20
                                                     { 2, 1, 3, 2, 1, 2, 0, 0 }, // 21
                                                     { 2, 2, 3, 1, 1, 2, 0, 0 }, // 22
                                                     { 3, 1, 2, 1, 3, 1, 0, 0 }, // 23
                                                     { 3, 1, 1, 2, 2, 2, 0, 0 }, // 24
                                                     { 3, 2, 1, 1, 2, 2, 0, 0 }, // 25
                                                     { 3, 2, 1, 2, 2, 1, 0, 0 }, // 26
                                                     { 3, 1, 2, 2, 1, 2, 0, 0 }, // 27
                                                     { 3, 2, 2, 1, 1, 2, 0, 0 }, // 28
                                                     { 3, 2, 2, 2, 1, 1, 0, 0 }, // 29
                                                     { 2, 1, 2, 1, 2, 3, 0, 0 }, // 30
                                                     { 2, 1, 2, 3, 2, 1, 0, 0 }, // 31
                                                     { 2, 3, 2, 1, 2, 1, 0, 0 }, // 32
                                                     { 1, 1, 1, 3, 2, 3, 0, 0 }, // 33
                                                     { 1, 3, 1, 1, 2, 3, 0, 0 }, // 34
                                                     { 1, 3, 1, 3, 2, 1, 0, 0 }, // 35
                                                     { 1, 1, 2, 3, 1, 3, 0, 0 }, // 36
                                                     { 1, 3, 2, 1, 1, 3, 0, 0 }, // 37
                                                     { 1, 3, 2, 3, 1, 1, 0, 0 }, // 38
                                                     { 2, 1, 1, 3, 1, 3, 0, 0 }, // 39
                                                     { 2, 3, 1, 1, 1, 3, 0, 0 }, // 40
                                                     { 2, 3, 1, 3, 1, 1, 0, 0 }, // 41
                                                     { 1, 1, 2, 1, 3, 3, 0, 0 }, // 42
                                                     { 1, 1, 2, 3, 3, 1, 0, 0 }, // 43
                                                     { 1, 3, 2, 1, 3, 1, 0, 0 }, // 44
                                                     { 1, 1, 3, 1, 2, 3, 0, 0 }, // 45
                                                     { 1, 1, 3, 3, 2, 1, 0, 0 }, // 46
                                                     { 1, 3, 3, 1, 2, 1, 0, 0 }, // 47
                                                     { 3, 1, 3, 1, 2, 1, 0, 0 }, // 48
                                                     { 2, 1, 1, 3, 3, 1, 0, 0 }, // 49
                                                     { 2, 3, 1, 1, 3, 1, 0, 0 }, // 50
                                                     { 2, 1, 3, 1, 1, 3, 0, 0 }, // 51
                                                     { 2, 1, 3, 3, 1, 1, 0, 0 }, // 52
                                                     { 2, 1, 3, 1, 3, 1, 0, 0 }, // 53
                                                     { 3, 1, 1, 1, 2, 3, 0, 0 }, // 54
                                                     { 3, 1, 1, 3, 2, 1, 0, 0 }, // 55
                                                     { 3, 3, 1, 1, 2, 1, 0, 0 }, // 56
                                                     { 3, 1, 2, 1, 1, 3, 0, 0 }, // 57
                                                     { 3, 1, 2, 3, 1, 1, 0, 0 }, // 58
                                                     { 3, 3, 2, 1, 1, 1, 0, 0 }, // 59
                                                     { 3, 1, 4, 1, 1, 1, 0, 0 }, // 60
                                                     { 2, 2, 1, 4, 1, 1, 0, 0 }, // 61
                                                     { 4, 3, 1, 1, 1, 1, 0, 0 }, // 62
                                                     { 1, 1, 1, 2, 2, 4, 0, 0 }, // 63
                                                     { 1, 1, 1, 4, 2, 2, 0, 0 }, // 64
                                                     { 1, 2, 1, 1, 2, 4, 0, 0 }, // 65
                                                     { 1, 2, 1, 4, 2, 1, 0, 0 }, // 66
                                                     { 1, 4, 1, 1, 2, 2, 0, 0 }, // 67
                                                     { 1, 4, 1, 2, 2, 1, 0, 0 }, // 68
                                                     { 1, 1, 2, 2, 1, 4, 0, 0 }, // 69
                                                     { 1, 1, 2, 4, 1, 2, 0, 0 }, // 70
                                                     { 1, 2, 2, 1, 1, 4, 0, 0 }, // 71
                                                     { 1, 2, 2, 4, 1, 1, 0, 0 }, // 72
                                                     { 1, 4, 2, 1, 1, 2, 0, 0 }, // 73
                                                     { 1, 4, 2, 2, 1, 1, 0, 0 }, // 74
                                                     { 2, 4, 1, 2, 1, 1, 0, 0 }, // 75
                                                     { 2, 2, 1, 1, 1, 4, 0, 0 }, // 76
                                                     { 4, 1, 3, 1, 1, 1, 0, 0 }, // 77
                                                     { 2, 4, 1, 1, 1, 2, 0, 0 }, // 78
                                                     { 1, 3, 4, 1, 1, 1, 0, 0 }, // 79
                                                     { 1, 1, 1, 2, 4, 2, 0, 0 }, // 80
                                                     { 1, 2, 1, 1, 4, 2, 0, 0 }, // 81
                                                     { 1, 2, 1, 2, 4, 1, 0, 0 }, // 82
                                                     { 1, 1, 4, 2, 1, 2, 0, 0 }, // 83
                                                     { 1, 2, 4, 1, 1, 2, 0, 0 }, // 84
                                                     { 1, 2, 4, 2, 1, 1, 0, 0 }, // 85
                                                     { 4, 1, 1, 2, 1, 2, 0, 0 }, // 86
                                                     { 4, 2, 1, 1, 1, 2, 0, 0 }, // 87
                                                     { 4, 2, 1, 2, 1, 1, 0, 0 }, // 88
                                                     { 2, 1, 2, 1, 4, 1, 0, 0 }, // 89
                                                     { 2, 1, 4, 1, 2, 1, 0, 0 }, // 90
                                                     { 4, 1, 2, 1, 2, 1, 0, 0 }, // 91
                                                     { 1, 1, 1, 1, 4, 3, 0, 0 }, // 92
                                                     { 1, 1, 1, 3, 4, 1, 0, 0 }, // 93
                                                     { 1, 3, 1, 1, 4, 1, 0, 0 }, // 94
                                                     { 1, 1, 4, 1, 1, 3, 0, 0 }, // 95
                                                     { 1, 1, 4, 3, 1, 1, 0, 0 }, // 96
                                                     { 4, 1, 1, 1, 1, 3, 0, 0 }, // 97
                                                     { 4, 1, 1, 3, 1, 1, 0, 0 }, // 98
                                                     { 1, 1, 3, 1, 4, 1, 0, 0 }, // 99
                                                     { 1, 1, 4, 1, 3, 1, 0, 0 }, // 100
                                                     { 3, 1, 1, 1, 4, 1, 0, 0 }, // 101
                                                     { 4, 1, 1, 1, 3, 1, 0, 0 }, // 102
                                                     { 2, 1, 1, 4, 1, 2, 0, 0 }, // 103
                                                     { 2, 1, 1, 2, 1, 4, 0, 0 }, // 104
                                                     { 2, 1, 1, 2, 3, 2, 0, 0 }, // 105
                                                     { 2, 3, 3, 1, 1, 1, 2, 0 } // 106
                                                   };

        //public static System.Drawing MakeBarcodeImage(string inputData, int barWeight, bool addQuietZone)
        //{
        //    // get the Code128 codes to represent the message
        //    var content = new Code128Content(inputData);
        //    var codes = content.Codes;

        //    var width = (((codes.Length - 3) * 11) + 35) * barWeight;
        //    var height = Convert.ToInt32(Math.Ceiling(Convert.ToSingle(width) * .15F));

        //    if (addQuietZone)
        //    {
        //        width += 2 * CQuietWidth * barWeight; // on both sides
        //    }

        //    // get surface to draw on
        //    System.Drawing.Image myImage = new Bitmap(width, height);
        //    using (var gr = Graphics.FromImage(myImage))
        //    {
        //        // set to white so we don't have to fill the spaces with white
        //        gr.FillRectangle(Brushes.White, 0, 0, width, height);

        //        // skip quiet zone
        //        var cursor = addQuietZone ? CQuietWidth * barWeight : 0;

        //        for (var codeIdx = 0; codeIdx < codes.Length; codeIdx++)
        //        {
        //            var code = codes[codeIdx];

        //            // take the bars two at a time: a black and a white
        //            for (var bar = 0; bar < 8; bar += 2)
        //            {
        //                var barWidth = CPatterns[code, bar] * barWeight;
        //                var spcWidth = CPatterns[code, bar + 1] * barWeight;

        //                // if width is zero, don't try to draw it
        //                if (barWidth > 0)
        //                {
        //                    gr.FillRectangle(Brushes.Black, cursor, 0, barWidth, height);
        //                }

        //                // note that we never need to draw the space, since we 
        //                // initialized the graphics to all white

        //                // advance cursor beyond this pair
        //                cursor += barWidth + spcWidth;
        //            }
        //        }
        //    }

        //    return myImage;
        //}



        //public static List<countries> loadprefix()
        //{
        //    string file = HttpContext.Current.Server.MapPath("~/Models/phonecode.json");
        //    //deserialize JSON from file  
        //    string Json = System.IO.File.ReadAllText(file);

        //    JavaScriptSerializer ser = new JavaScriptSerializer();
        //    var personlist = ser.Deserialize<List<countries>>(Json);
        //    List<countries> cs = new List<countries>();
        //    foreach (var item in personlist)
        //    {
        //        countries c = new countries();
        //        c.name = item.name;
        //        c.dial_code = item.dial_code;
        //        c.code = item.code;
        //        cs.Add(c);
        //    }
        //    return cs;
        //}
        public class countries
        {
            public string name { get; set; }
            public string dial_code { get; set; }
            public string code { get; set; }
        }
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public static string IntToStringStatusPackage(int status)
        {
            if (status == 0)
                return "<span class=\"bg-bronze\">Mới tạo</span>";
            else if (status == 1)
                return "<span class=\"bg-green\">Đang chuyển về VN</span>";
            else if (status == 2)
                return "<span class=\"bg-blue\">Đã nhận hàng tại VN</span>";
            else
                return "<span class=\"bg-red\">Đã hủy</span>";
        }
        public static string IntToStringStatusSmallPackage(int status)
        {
            if (status == 1)
                return "Đã nhận hàng tại TQ";
            else if (status == 2)
                return "Đang chuyển về VN";
            else if (status == 3)
                return "Đã nhận hàng tại VN";
            else
                return "Đã giao cho khách";
        }
        //public static string RemoveHTMLTags(string content)
        //{
        //    var cleaned = string.Empty;
        //    try
        //    {
        //        StringBuilder textOnly = new StringBuilder();
        //        using (var reader = XmlNodeReader.Create(new System.IO.StringReader("<xml>" + content + "</xml>")))
        //        {
        //            while (reader.Read())
        //            {
        //                if (reader.NodeType == XmlNodeType.Text)
        //                    textOnly.Append(reader.ReadContentAsString());
        //            }
        //        }
        //        cleaned = textOnly.ToString();
        //    }
        //    catch
        //    {
        //        //A tag is probably not closed. fallback to regex string clean.
        //        string textOnly = string.Empty;
        //        Regex tagRemove = new Regex(@"<[^>]*(>|$)");
        //        Regex compressSpaces = new Regex(@"[\s\r\n]+");
        //        textOnly = tagRemove.Replace(content, string.Empty);
        //        textOnly = compressSpaces.Replace(textOnly, " ");
        //        cleaned = textOnly;
        //    }

        //    return cleaned;
        //}
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

        //public static void CreateBarcodeNew(string code)
        //{
        //    var myBitmap = new Bitmap(500, 50);
        //    var g = Graphics.FromImage(myBitmap);
        //    var jgpEncoder = GetEncoder(ImageFormat.Jpeg);

        //    g.Clear(Color.White);

        //    var strFormat = new StringFormat { Alignment = StringAlignment.Center };
        //    g.DrawString(code, new Font("Free 3 of 9", 50), Brushes.Black, new RectangleF(0, 0, 500, 50), strFormat);

        //    var myEncoder = System.Drawing.Imaging.Encoder.Quality;
        //    var myEncoderParameters = new EncoderParameters(1);

        //    var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        //    myEncoderParameters.Param[0] = myEncoderParameter;
        //    myBitmap.Save(HttpContext.Current.Server.MapPath("~/Uploads/Barcode.jpg"), jgpEncoder, myEncoderParameters);
        //}

        //private static ImageCodecInfo GetEncoder(ImageFormat format)
        //{

        //    var codecs = ImageCodecInfo.GetImageDecoders();

        //    foreach (var codec in codecs)
        //    {
        //        if (codec.FormatID == format.Guid)
        //        {
        //            return codec;
        //        }
        //    }
        //    return null;
        //}
        //public static Bitmap CreateBarcode1(string data)
        //{
        //    Bitmap barCode = new Bitmap(1, 1);
        //    Font threeOfNine = new Font("Free 3 of 9", 60, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        //    Graphics graphics = Graphics.FromImage(barCode);
        //    SizeF dataSize = graphics.MeasureString(data, threeOfNine);
        //    barCode = new Bitmap(barCode, dataSize.ToSize());
        //    graphics = Graphics.FromImage(barCode);
        //    graphics.Clear(Color.White);
        //    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
        //    graphics.DrawString(data, threeOfNine, new SolidBrush(Color.Black), 0, 0);
        //    graphics.Flush();
        //    threeOfNine.Dispose();
        //    graphics.Dispose();
        //    return barCode;
        //}
        //public static void CreateBarCode(string barCodedDetail, string fontName, int fontSize, string physicalPath)
        //{

        //    //Find the Width for barcode
        //    int width = barCodedDetail.Length * 35;

        //    //create Bitmap object with Width and Height
        //    Bitmap barCode = new Bitmap(width, 120);

        //    //path where you want to save the barcode Image
        //    string filePath = string.Format("{0}{1}.png", physicalPath, barCodedDetail);

        //    //create the barcoded font object
        //    Font barCodeFont = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Point);

        //    //creating the graphics object for the Bitmap.
        //    Graphics graphics = Graphics.FromImage(barCode);

        //    SizeF sizeF = graphics.MeasureString(barCodedDetail, barCodeFont);

        //    barCode = new Bitmap(barCode, sizeF.ToSize());

        //    graphics = Graphics.FromImage(barCode);

        //    SolidBrush brushBlack = new SolidBrush(Color.Black);

        //    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

        //    //putting * before and after the barCodedDetail,
        //    //this is because scanner only read the data which is started and end with *
        //    graphics.DrawString("*" + barCodedDetail + "*", barCodeFont, brushBlack, 1, 1);

        //    graphics.Dispose();
        //    //Saving the Image file
        //    barCode.Save(filePath, ImageFormat.Png);
        //    barCode.Dispose();
        //    HttpContext.Current.Response.Clear();

        //}
        //public static string TranslateText(string input, string languagePair)
        //{
        //    string returnstr = input;
        //    try
        //    {
        //        string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
        //        var request = (HttpWebRequest)WebRequest.Create(url);
        //        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
        //        request.Method = "GET";
        //        var content = String.Empty;
        //        HttpStatusCode statusCode;
        //        using (var response = request.GetResponse())
        //        using (var stream = response.GetResponseStream())
        //        {
        //            var contentType = response.ContentType;
        //            Encoding encoding = null;
        //            if (contentType != null)
        //            {
        //                var match = Regex.Match(contentType, @"(?<=charset\=).*");
        //                if (match.Success)
        //                    encoding = Encoding.GetEncoding(match.ToString());
        //            }

        //            encoding = encoding ?? Encoding.UTF8;

        //            statusCode = ((HttpWebResponse)response).StatusCode;
        //            using (var reader = new StreamReader(stream, encoding))
        //                content = reader.ReadToEnd();
        //        }
        //        var doc = Dcsoup.Parse(content);
        //        var scoreDiv = doc.Select("html").Select("span[id=result_box]").Html;
        //        returnstr = scoreDiv;
        //    }
        //    catch
        //    {

        //    }

        //    return returnstr;
        //}
        //public static string GenBarCode(string Code, string physicalPath)
        //{
        //    string filePath = string.Format("{0}{1}.jpg", physicalPath, Code);
        //    // Multiply the lenght of the code by 40 (just to have enough width)
        //    int w = Code.Length * 40;

        //    // Create a bitmap object of the width that we calculated and height of 100
        //    Bitmap oBitmap = new Bitmap(w, 100);

        //    // then create a Graphic object for the bitmap we just created.
        //    Graphics oGraphics = Graphics.FromImage(oBitmap);

        //    // Now create a Font object for the Barcode Font
        //    // (in this case the IDAutomationHC39M) of 18 point size
        //    Font oFont = new Font("IDAutomationHC39M", 18);

        //    // Let's create the Point and Brushes for the barcode
        //    PointF oPoint = new PointF(2f, 2f);
        //    SolidBrush oBrushWrite = new SolidBrush(Color.Black);
        //    SolidBrush oBrush = new SolidBrush(Color.White);

        //    // Now lets create the actual barcode image
        //    // with a rectangle filled with white color
        //    oGraphics.FillRectangle(oBrush, 0, 0, w, 100);

        //    // We have to put prefix and sufix of an asterisk (*),
        //    // in order to be a valid barcode
        //    oGraphics.DrawString("*" + Code + "*", oFont, oBrushWrite, oPoint);

        //    // Then we send the Graphics with the actual barcode
        //    HttpContext.Current.Response.ContentType = "image/jpeg";
        //    oBitmap.Save(filePath, ImageFormat.Jpeg);
        //    oBitmap.Dispose();
        //    HttpContext.Current.Response.Clear();
        //    return filePath;
        //}
        public static string fnRemoveSplChars(string strMyString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in strMyString)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') && c == ';')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        public static string removeSpecialCharacter(string strMyString)
        {
            if (strMyString.Contains("<"))
            {
                strMyString = strMyString.Replace("<", "");
            }
            if (strMyString.Contains("["))
            {
                strMyString = strMyString.Replace("[", "");
            }
            if (strMyString.Contains("^"))
            {
                strMyString = strMyString.Replace("^", "");
            }
            if (strMyString.Contains(">"))
            {
                strMyString = strMyString.Replace(">", "");
            }
            if (strMyString.Contains("]"))
            {
                strMyString = strMyString.Replace("]", "");
            }
            if (strMyString.Contains("+"))
            {
                strMyString = strMyString.Replace("+", "");
            }
            if (strMyString.Contains("//"))
            {
                strMyString = strMyString.Replace("/", "");
            }
            if (strMyString.Contains("\\"))
            {
                strMyString = strMyString.Replace("\\", "");
            }
            if (strMyString.Contains("'"))
            {
                strMyString = strMyString.Replace("'", "");
            }
            if (strMyString.Contains("."))
            {
                strMyString = strMyString.Replace(".", "");
            }
            if (strMyString.Contains("{"))
            {
                strMyString = strMyString.Replace("{", "");
            }
            if (strMyString.Contains("}"))
            {
                strMyString = strMyString.Replace("}", "");
            }
            if (strMyString.Contains("("))
            {
                strMyString = strMyString.Replace("(", "");
            }
            if (strMyString.Contains(")"))
            {
                strMyString = strMyString.Replace(")", "");
            }
            if (strMyString.Contains("#"))
            {
                strMyString = strMyString.Replace("#", "");
            }
            if (strMyString.Contains("$"))
            {
                strMyString = strMyString.Replace("$", "");
            }
            if (strMyString.Contains("*"))
            {
                strMyString = strMyString.Replace("*", "");
            }
            if (strMyString.Contains("@"))
            {
                strMyString = strMyString.Replace("@", "");
            }
            if (strMyString.Contains("!"))
            {
                strMyString = strMyString.Replace("!", "");
            }
            if (strMyString.Contains(":"))
            {
                strMyString = strMyString.Replace(":", "");
            }
            //if (strMyString.Contains(";"))
            //{
            //    strMyString = strMyString.Replace(";", "");
            //}
            if (strMyString.Contains("?"))
            {
                strMyString = strMyString.Replace("?", "");
            }
            if (strMyString.Contains(">"))
            {
                strMyString = strMyString.Replace(">", "");
            }
            if (strMyString.Contains("."))
            {
                strMyString = strMyString.Replace(".", "");
            }
            return strMyString.ToString();
        }
        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", ";", RegexOptions.Compiled);
        }
        public static string TranslateTextNew(string input, string sLang, string tLang)
        {
            //string url = String.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            try
            {
                string url = String.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", sLang, tLang, input);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
                request.Method = "GET";
                var content = String.Empty;
                HttpStatusCode statusCode;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var contentType = response.ContentType;
                    Encoding encoding = null;
                    if (contentType != null)
                    {
                        var match = Regex.Match(contentType, @"(?<=charset\=).*");
                        if (match.Success)
                            encoding = Encoding.GetEncoding(match.ToString());
                    }
                    encoding = encoding ?? Encoding.UTF8;
                    statusCode = ((HttpWebResponse)response).StatusCode;
                    using (var reader = new StreamReader(stream, encoding))
                        content = reader.ReadToEnd();
                }
                return content;
                //var doc = Dcsoup.Parse(content);
                //var scoreDiv = doc.Select("html").Select("span[class=tlid-translation translation]").Html;
                //return scoreDiv.ToString();

            }
            catch
            {
                return input;
            }
        }

        #region Get Base64 from IFormfile
        public static string GetBase64IFormFile(IFormFile file)
        {
            string strBase64 = "";
            if (file != null)
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    var byteArr = stream.ToArray();
                    strBase64 = Convert.ToBase64String(byteArr);
                }
            }
            return strBase64;
        }
        #endregion

        #region Resize File Image
        public static byte[] ResizeImage(IFormFile file, int width = 100, int height = 100)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var byteArr = ms.ToArray();
                var imageResize = Image.FromStream(ms);

                var ratioX = (double)width / imageResize.Width;
                var ratioY = (double)height / imageResize.Height;

                var ratio = Math.Min(ratioX, ratioY);

                width = (int)(imageResize.Width * ratio);
                height = (int)(imageResize.Height * ratio);

                var newImage = new Bitmap(width, height);

                Graphics.FromImage(newImage).DrawImage(imageResize, 0, 0, width, height);

                Bitmap bmp = new Bitmap(newImage);

                ImageConverter converter = new ImageConverter();

                byteArr = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
                return byteArr;
            }
        }
        #endregion

        #region Get FileExtension From Base64
        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return ".png";
                case "/9J/4":
                    return ".jpg";
                case "AAAAF":
                    return ".mp4";
                case "JVBER":
                    return ".pdf";
                case "AAABA":
                    return ".ico";
                case "UMFYI":
                    return ".rar";
                case "E1XYD":
                    return ".rtf";
                case "U1PKC":
                    return ".txt";
                case "MQOWM":
                case "77U/M":
                    return ".srt";
                default:
                    return ".png";
            }
        }
        #endregion
    }
}
