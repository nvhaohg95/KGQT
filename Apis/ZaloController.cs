using Microsoft.AspNetCore.Mvc;

namespace KGQT.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloController : ControllerBase
    {
        [HttpGet]
        [Route("callback")]
        public bool CallbackZalo()
        {
            return true;
        }
    }
}
