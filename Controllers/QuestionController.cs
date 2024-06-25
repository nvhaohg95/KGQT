using KGQT.Business;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;

namespace KGQT.Controllers
{
    public class QuestionController : Controller
    {
        public IActionResult Index(int page = 1)
        {
            var oData = QuestionBusiness.GetList(1, page, 5);
            List<tbl_Questions> lstData = oData[0] as List<tbl_Questions>;
            int numberRecord = (int)oData[1];
            int numberPage = (int)oData[2];
            ViewBag.pageCurrent = page;
            ViewBag.numberPage = numberPage;
            ViewBag.numberRecord = numberRecord;
            return View(lstData);
        }
    }
}
