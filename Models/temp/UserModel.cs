using System;
using System.ComponentModel.DataAnnotations;

namespace KGQT.Models.temp
{
    public class UserModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string PassWord { get; set; }
    }

    public class ForgotPassWord : UserModel
    {
        [Required(ErrorMessage = "Địa chỉ email không được bỏ trống")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string ConfirmPassword { get; set; }

    }

    public class ChangePassword
    {
        public string UserName { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
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
        public string? Token { get; set; }
        public string? UserID { get; set; }
        public DateTime? CreateDate { get; internal set; }
    }

    public class AccountInfo
    {
        public int? ID { get; set; }
        public int? RoleID { get; set; }
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IMG { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public double? Wallet { get; set; }


    }

    public class SignUpModel
    {
        [Required(ErrorMessage = "Tài khoản không được bỏ trống")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Họ không được bỏ trống")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống")]
        public string LastName { get; set; }

        public DateTime? BirthDay { get; set; }

        [Range(1, 2, ErrorMessage = "Vui lòng chọn giới tính")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Địa chỉ Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
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
