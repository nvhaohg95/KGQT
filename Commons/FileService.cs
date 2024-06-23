using System.Drawing;
using System.Drawing.Imaging;

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

        public static byte[] ResizeImage(IFormFile image, int width = 300, int height = 300)
        {
            using (var stream = image.OpenReadStream())
            {
                var originalImage = Image.FromStream(stream);

                // Check if the original image dimensions are smaller than the specified dimensions
                if (originalImage.Width <= width && originalImage.Height <= height)
                {
                    // Return the original image as it is
                    using (var ms = new MemoryStream())
                    {
                        originalImage.Save(ms, originalImage.RawFormat);
                        return ms.ToArray();
                    }
                }

                // Calculate new width and height to maintain the aspect ratio
                var ratioX = (double)width / originalImage.Width;
                var ratioY = (double)height / originalImage.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(originalImage.Width * ratio);
                var newHeight = (int)(originalImage.Height * ratio);

                var resizedImage = new Bitmap(newWidth, newHeight);

                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                }

                using (var ms = new MemoryStream())
                {
                    resizedImage.Save(ms, originalImage.RawFormat);
                    return ms.ToArray();
                }
            }
        }


        public static byte[] ResizeAndCompressImage(IFormFile image, int width = 300, int height = 300, long quality = 75L)
        {
            using (var stream = image.OpenReadStream())
            {
                var originalImage = Image.FromStream(stream);

                // Check if the original image dimensions are smaller than the specified dimensions
                if (originalImage.Width <= width && originalImage.Height <= height)
                {
                    // Return the original image as it is
                    using (var ms = new MemoryStream())
                    {
                        SaveImageToStream(originalImage, ms, quality, originalImage.RawFormat);
                        return ms.ToArray();
                    }
                }

                // Calculate new width and height to maintain the aspect ratio
                var ratioX = (double)width / originalImage.Width;
                var ratioY = (double)height / originalImage.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(originalImage.Width * ratio);
                var newHeight = (int)(originalImage.Height * ratio);

                // Create a new bitmap with the new dimensions
                var resizedImage = new Bitmap(newWidth, newHeight);

                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    // Set quality settings
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    // Draw the original image onto the resized bitmap
                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                }

                // Save the resized and compressed image to a memory stream
                using (var ms = new MemoryStream())
                {
                    SaveImageToStream(resizedImage, ms, quality, originalImage.RawFormat);
                    return ms.ToArray();
                }
            }
        }

        private static void SaveImageToStream(Image image, Stream stream, long quality, ImageFormat format)
        {
            // Determine if the image format is supported for compression
            if (format.Equals(ImageFormat.Jpeg) || format.Equals(ImageFormat.Png))
            {
                // Get the codec for the specified image format
                var codec = ImageCodecInfo.GetImageDecoders()
                                          .FirstOrDefault(c => c.FormatID == format.Guid);

                if (codec != null)
                {
                    // Set the quality parameter for the encoder
                    var encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                    // Save the image using the codec and quality parameters
                    image.Save(stream, codec, encoderParams);
                    return;
                }
            }

            // If the format is not JPEG or PNG, save the image without compression
            image.Save(stream, format);
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

        public static List<FileInfoModel> LoadFiles(string FolderPath, List<FileInfoModel> lstFile)
        {
            if(!Directory.Exists(FolderPath)) return lstFile;
            string[] Files = System.IO.Directory.GetFiles(FolderPath, "*.xlsx", SearchOption.AllDirectories);
            string[] Directories = System.IO.Directory.GetDirectories(FolderPath);
            for (int i = 0; i < Files.Length; i++)
            {
                if (Files[i].ToLower().EndsWith(".xlsx".ToLower()))
                {
                    var file = new FileInfo(Files[i]);
                        lstFile.Add(new FileInfoModel
                        {
                            FileName = file.Name,
                            FilePath = file.FullName,
                            CreatedOn = file.CreationTime,
                        });
                }
            }
            return lstFile;
        }

        
    }


    public class FileInfoModel
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    


}
