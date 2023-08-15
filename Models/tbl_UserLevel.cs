using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_UserLevel
    {
        public int ID { get; set; }
        public string? LevelName { get; set; }
        public double? AmountEarnFrom { get; set; }
        public double? AmountEarnTo { get; set; }
        public double? PerFeeService { get; set; }
        public double? PerFeeCheckCK { get; set; }
        public double? FeeWeightCK { get; set; }
        public double? LessDeposit { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
