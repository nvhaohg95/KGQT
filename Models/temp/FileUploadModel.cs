namespace KGQT.Models.temp
{
    public class FileUploadModel
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int FileType { get; set; }
        public IFormFile File { get; set; }

    }
}
