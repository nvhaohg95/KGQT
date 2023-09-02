using System;
using System.ComponentModel.DataAnnotations;

namespace KGQT.Models.temp
{
    public class UserModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string Password { get; set; }

    }
    public class ForgotPasswordModel : UserModel
    {
        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress]
        public string Email { get; set; }

    }


    public class Jwt
    {
        public string key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }

    public class UserLogin
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobilePhonePrefix { get; set; }
        public string? MobilePhone { get; set; }
        public string? IMG { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public int? RoleID { get; set; }
        public int? UserLevel { get; set; }
        public int? Status { get; set; }
        public double? Wallet { get; set; }
        public bool? IsNotReceiveMail { get; set; }
        public bool? IsOutCityCustomer { get; set; }
        public bool? IsActive { get; set; }
        public int? SaleID { get; set; }
        public string? SaleUsername { get; set; }
        public string Token { get; set; }
        public DateTime? CreateDate { get; internal set; }
    }

    public class AccountInfo
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IMG { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? Gender { get; set; }
        public string BirthDay { get; set; }
        public string? Email { get; set; }
    }

    public class SignUpModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string LastName { get; set; }

        public DateTime? BirthDay { get; set; }

        [Range(1, 2, ErrorMessage = "Vui lòng nhập giới tính")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string? MobilePhonePrefix { get; set; }
        public string? MobilePhone { get; set; }
        public string? IMG { get; set; }
        public string? Latitude { get; internal set; }
        public string? Longitude { get; internal set; }
        public int? RoleID { get; set; }
        public int? UserLevel { get; set; }
        public int? Status { get; set; }
        public DateTime? CraetedDate { get; set; }
        public string? CraetedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public IFormFile? File { get; set; }
        public string? Path { get; set; }

    }
}
