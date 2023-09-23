using KGQT.Models;

namespace KGQT.Business.Base
{
    public class BusinessBase
    {
        protected static nhanshiphangContext _db = new nhanshiphangContext();
        IConfiguration _config;
    }
}
