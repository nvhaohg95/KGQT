using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Casso
    {
        public Guid RecID { get; set; }
        public int ID { get; set; }
        public string? tid { get; set; }
        public string? Time { get; set; }
        public int? amount { get; set; }
        public string? description { get; set; }
        public string? subAccId { get; set; }
        public string? OrderID { get; set; }
        public int? cusum_balance { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CassoID { get; set; }
    }
}
