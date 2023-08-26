using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGQT.Models.temp
{
    public class FilterModel
    {
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 0;
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
    public class PagingModel
    {
        public int PageSize { get; set; } = 5;
        public int PageIndex { get; set; } = 0;
    }
}
