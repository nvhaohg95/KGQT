using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Zalo
    {
        public Guid RecID { get; set; }
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public string? expires_in { get; set; }
        public DateTime? freshtoken_expire { get; set; }
        public DateTime? accesstoken_expire { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
