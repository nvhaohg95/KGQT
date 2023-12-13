using KGQT.Business;
using KGQT.Models;
using KGQT.Models.temp;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Mobility
{
    [Route("api/[controller]")]
    public class AuthencationController : ControllerBase
    {
        [HttpGet]
        [Route("login")]
        public DataReturnModel<UserLogin> Login(string username, string password)
        {
            var result = AccountBusiness.Login(username, password);
            return result;
        }    

        [HttpPost]
        [Route("register")]
        public DataReturnModel RegisterAccount([FromBody] SignUpModel model)
        {
            if (model.File != null)
            {
                model.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads", "avatars");
            }
            var data = AccountBusiness.RegisterAccount(model);
            return data;
        }
    }
}
