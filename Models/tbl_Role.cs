using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Role
    {
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
