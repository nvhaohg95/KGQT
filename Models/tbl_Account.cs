using System;
using System.Collections.Generic;

namespace KGQT.Models
{
    public partial class tbl_Account
    {
        public int ID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        /// <summary>
        /// 0. Admin
        /// 1. User
        /// 2. Quản lý văn phòng
        /// 3. Quản lý kho
        /// 4. Nhân viên
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
        public double? Wallet { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public bool? IsNotReceiveMail { get; set; }
        public bool? IsOutCityCustomer { get; set; }
        public bool? IsActive { get; set; }
        public int? SaleID { get; set; }
        public string? SaleUsername { get; set; }
    }
}
