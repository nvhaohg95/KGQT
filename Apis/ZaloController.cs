using KGQT.Base;
using KGQT.Business;
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
        [Route("Authorization")]
        public IActionResult Authorization()
        {
            string uri = HttpUtility.UrlEncodeUnicode(Config.Zalo.Redirect_Uri);
            string url = string.Format(Config.Zalo.PerUrl, Config.Zalo.AppID, uri);
            return Redirect(url);
        }

        [HttpGet]
        [Route("checktoken")]
        public IActionResult CheckToken()
        {
            string uri = HttpUtility.UrlEncodeUnicode(Config.Zalo.Redirect_Uri);
            string url = string.Format(Config.Zalo.PerUrl, Config.Zalo.AppID, uri);
            return Redirect(url);
        }

        [HttpGet]
        [Route("request")]
        public async Task<IActionResult> Test2(string uid)
        {
            var data = await ZaloCommon.RequestMoreInfoAsync(uid);
            return Ok(data);
        }

        [HttpGet]
        [Route("dailytask")]
        public async Task<IActionResult> Test3()
        {
            PackagesBusiness.DailyTaskAsync();
            return Ok(200);
        }

        [HttpGet]
        [Route("sendall")]
        public async Task<IActionResult> SendAll(string message)
        {
            ZaloCommon.SendMessageToAllV2(message);
            return Ok(200);
        }

        [HttpGet]
        [Route("getlist")]
        public async Task<IActionResult> GetList()
        {
            await ZaloCommon.GetListFollowerV2();
            return Ok(200);
        }
        
        [HttpGet]
        [Route("getinfo")]
        public async Task<IActionResult> GetInfo(string uid)
        {
            await ZaloCommon.GetInfoFlowerAsync(uid);
            return Ok(200);
        }
    }
}
