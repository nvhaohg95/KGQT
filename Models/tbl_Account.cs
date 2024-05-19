using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Account
    {
        public int ID { get; set; }
        public string? UserID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDay { get; set; }
        public bool? Gender { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? IMG { get; set; }
        /// <summary>
        /// 1. Admin
        /// 4. User
        /// </summary>
        public int? RoleID { get; set; }
        /// <summary>
        /// 1. thành viên
        /// 2. dịch vụ order
        /// 3. shop đồng
        /// 4. shop bạc
        /// 5. shop vàng
        /// 6. shop kim cương
        /// 7. shop vip 1
        /// 8. shop vip 2
        /// 9. shop vip 3
        /// 10. shop vip 4
        /// </summary>
        public int? UserLevel { get; set; }
        /// <summary>
        /// 1. Not Active
        /// 2. Active
        /// 3. Banned
        /// </summary>
        public int? Status { get; set; }
        public int AvailableSearch { get; set; }
        public string? Wallet { get; set; }
        public bool? IsNotReceiveMail { get; set; }
        public bool? IsOutCityCustomer { get; set; }
        public bool? IsActive { get; set; }
        public int? SaleID { get; set; }
        public string? SaleUsername { get; set; }
        public string? Token { get; set; }
        public string? TokenDevice { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
