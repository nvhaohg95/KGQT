using System.Drawing;

namespace KGQT.Commons
{
    public class FileService
    {
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
