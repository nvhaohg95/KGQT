using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Question
    {
        public int ID { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? MidifiedOn { get; set; }
    }
}
