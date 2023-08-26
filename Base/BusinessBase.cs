using KGQT.Models;

namespace KGQT.Business.Base
{
    public class BusinessBase
    {
        protected static KGNewContext _db = new KGNewContext();
        IConfiguration _config;
    }
}
