using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGQT.Models.temp
{
    public class OrderModel : FilterModel
    {
        public string? OrderId { get; set; }
        public int? Status { get; set; }
        public int? SubStatus { get; set; }
    }
}
