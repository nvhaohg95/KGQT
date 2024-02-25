namespace KGQT.Models
{
    public partial class tbl_Posts
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Contents { get; set; }
        public string CraetedBy { get; set; }
        public DateTime CraetedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
