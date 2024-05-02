namespace KGQT.Models.temp
{
    public class UserVM
    {
        public List<AccountInfo> ListUser { get; set; }
        public AccountInfo User { get; set; }
        public UserVM()
        {
            ListUser = new List<AccountInfo>();
            User = new AccountInfo();
        }
    }

    public class UserRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
