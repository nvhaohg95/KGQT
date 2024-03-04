using DocumentFormat.OpenXml.Spreadsheet;
using KGQT.Business;
using KGQT.Business.Base;
using KGQT.Commons;
using KGQT.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace KGQT.Areas.Admin.Controllers
{
    [Area("admin")]
    public class BigPackageController : Controller
    {
        public IActionResult Index(string code, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            var oData = BigPackage.GetPage(code, fromDate, toDate, page, pageSize);
            var lstData = oData[0] as List<tbl_BigPackage>;
            int totalRecord = (int)oData[1];
            int totalPage = (int)oData[2];
            @ViewData["code"] = code;
            @ViewData["fromDate"] = fromDate;
            @ViewData["toDate"] = toDate;
            @ViewData["page"] = page;
            @ViewData["totalRecord"] = totalRecord;
            @ViewData["totalPage"] = totalPage;
            return View(lstData);
        }

        public IActionResult Detail(int id)
        {
            if (id == null || id < 0) return BadRequest();
            var data = Packages.GetByBigPackage(id);
            return View(data);
        }

        [HttpGet]
        public FileResult GenerateExcel(int id)
        {
            var big = BusinessBase.GetOne<tbl_BigPackage>(x => x.ID == id);
            var data = Packages.GetByBigPackage(id);
            if (data == null) return null;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage())
            {
                ExcelWorksheet ws = excel.Workbook.Worksheets.Add("Sheet 1");
                //Add Header
                ws.Cells[1, 1].Value = "MVĐ";
                ws.Cells[1, 2].Value = "Username";
                ws.Cells[1, 3].Value = "Tuyến";
                ws.Cells[1, 4].Value = "Cân nặng";
                ws.Cells[1, 5].Value = "Ngày xuất kho";
                ws.Cells[1, 6].Value = "Đo lên";
                ws.Cells[1, 7].Value = "Kích thước";
                ws.Column(1).Width = 25;
                ws.Column(5).Width = 25;
                ws.Column(7).Width = 25;

                var modelRows = data.Count() + 1;
                string modelRange = "A1:E" + modelRows.ToString();
                var modelTable = ws.Cells[modelRange];

                // Assign borders
                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                List<int> rows = new List<int>();
                int i = 2;
                foreach (var row in data)
                {
                    ws.Cells["A" + i].Value = row.PackageCode;
                    ws.Cells["B" + i].Value = row.Username;
                    ws.Cells["C" + i].Value = PJUtils.MovingMethod2Type(row.MovingMethod);
                    ws.Cells["D" + i].Value = Converted.Double2String(row.Weight);
                    ws.Cells["E" + i].Value = Converted.Date2String(row.OrderDate, true);
                    if (row.WeightExchange > 0)
                    {
                        ws.Cells["F" + i].Value = Converted.Double2String(row.WeightExchange);
                        ws.Cells["G" + i].Value = row.Length + "x" + row.Width + "x" + row.Height;

                    }
                    if (row.ImportedSGWH == null || row.ImportedSGWH == DateTime.MinValue)
                    {
                        rows.Add(i);
                    }
                    i++;
                }
                ws.Cells["A1:G1"].Style.Fill.SetBackground(System.Drawing.Color.Blue);
                ws.Cells["A1:G1"].Style.Font.Bold = true;
                ws.Cells["A1:G1"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                if (rows.Count > 0)
                {
                    foreach (var item in rows)
                    {
                        ws.Cells["A" + item + ":" + "E" + item].Style.Fill.SetBackground(System.Drawing.Color.Gray);
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    excel.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Export" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");
                }
            }
        }
    }
}
