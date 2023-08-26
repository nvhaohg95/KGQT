namespace KGQT.Models.temp
{
    public class CommentDetail
    {
        public int ID { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string IMG { get; set; }
        public int? RoleID { get; set; }
    }

    public class CommentModel
    {
        public int ComplainID { get; set; }
        public int UID { get; set; }
        public string Comment { get; set; }

        public string OrderShopID { get; set; }
    }


}
