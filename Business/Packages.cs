using KGQT.Models;

namespace KGQT.Business
{
    public static class Packages
    {
        public static bool CheckExist(string package)
        {
            using (var db = new nhanshiphangContext())
                return db.tbl_Packages.Any(x=>x.PackageOrderHangCode == package);
        }
    }
}
