namespace KGQT.Models
{
    public class tbl_Images
    {
        public string RecID { get; set; }
        /// <summary>
        /// 1. Logo
        /// 2. Slide
        /// 3. Banner
        /// 4. Sticket
        /// </summary>
        public int ImageType { get; set; }
        public string ImageSrc { get; set; }
        /// <summary>
        /// 0. Mới tạo
        /// 1. Đang sử dụng
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Lưu vị trí Slide/Banner/....
        /// </summary>
        public int? ViewIndex { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

    }
}
