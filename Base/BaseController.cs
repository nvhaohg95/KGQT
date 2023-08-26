using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Base
{
    public class BaseController : ControllerBase
    {
        protected UserModel user = new UserModel();
        protected static KGNewContext _db = new KGNewContext();
        public BaseController()
        {}

        [HttpGet]
        [Route("ahihi")]
        public tbl_Account GetLogin()
        {
            string username = User.FindFirstValue("UserName");
            if (string.IsNullOrEmpty(username)) return null;
            var u = _db.tbl_Accounts.FirstOrDefault(x => x.Username == username);
            return u;
        }

    }
}
