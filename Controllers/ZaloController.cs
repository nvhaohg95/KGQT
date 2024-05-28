using KGQT.Base;
using KGQT.Business.Base;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace KGQT.Controllers
{
    public class ZaloController : Controller
    {
        public IActionResult Authorization()
        {
            string uri = HttpUtility.UrlEncodeUnicode(Config.Zalo.Redirect_Uri);
            string url = string.Format(Config.Zalo.PerUrl, Config.Zalo.AppID, uri);
            return Redirect(url);
        }

        public async Task<IActionResult> Callback()
        {
            var code = Request.Query["code"].ToString();
            var result = await ZaloCommon.GetTokenAsync(code);
            return Redirect("/admin/package/index");
        }
    }
}
