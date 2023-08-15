using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_AccountInfo
    {
        public int ID { get; set; }
        public int? UID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobilePhonePrefix { get; set; }
        public string? MobilePhone { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? IMG { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
