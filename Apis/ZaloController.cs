using KGQT.Base;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Policy;
using System.Web;

namespace KGQT.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloController : ControllerBase
    {
        [HttpGet]
        [Route("callback")]
        public async Task<IActionResult> CallbackAsync()
        {
            var code = Request.Query["code"].ToString();
            var result = await ZaloCommon.GetTokenAsync(code);
            return Redirect("/admin/package/index");
        }


        [HttpGet]
        [Route("checktoken")]
        public IActionResult CheckToken()
        {
            string uri = HttpUtility.UrlEncodeUnicode(Config.Zalo.Redirect_Uri);
            string url = string.Format(Config.Zalo.PerUrl, Config.Zalo.AppID, uri);
            return Redirect(url);
        }
    }
}
