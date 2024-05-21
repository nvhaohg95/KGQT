using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_TrackShippingOrder
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public int? ShipID { get; set; }
        public string? Context { get; set; }
        public int? Type { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifieldBy { get; set; }
        public DateTime? ModifieldOn { get; set; }
    }
}
