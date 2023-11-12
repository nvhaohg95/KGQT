using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_FeeWeight
    {
        public int ID { get; set; }
        public double? WeightFrom { get; set; }
        public double? WeightTo { get; set; }
        public double? Amount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        /// <summary>
        /// 1 bay nhanh
        /// 2 bay thường
        /// 3 bộ
        /// 4 lô 
        /// 5 biển
        /// </summary>
        public int? Type { get; set; }

        public double? MinWeight { get; set; }
    }
}
