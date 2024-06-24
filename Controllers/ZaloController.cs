using KGQT.Base;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class ZaloController : Controller
    {
        public async Task<IActionResult> Callback()
        {
            var code = Request.Query["code"].ToString();
            var result = await ZaloCommon.GetTokenAsync(code);
            return Redirect("/admin/package/index");
        }
    }
}
