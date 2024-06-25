namespace KGQT.Models
{
    public class tbl_Questions
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        /// <summary>
        ///  0. Chưa active
        ///  1. Đang active
        /// </summary>
        public int Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? MidifiedOn { get; set; }

    }
}
