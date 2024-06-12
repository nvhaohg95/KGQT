using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Image
    {
        public string RecID { get; set; } = null!;
        public int? ImageType { get; set; }
        public string? ImageSrc { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
