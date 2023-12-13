using CsvHelper.Configuration;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGQT.Models.temp
{
    public class CsvPackageImport
    {
        public string ShippingCode { get; set; }
        public string CustomerName { get; set; }
        public string CreatedOn { get; set; }
        public int MovingMethod { get; set; }
        public string Note { get; set; }

        [NotMapped]
        public int _m { get; set; }
        public int _method
        {
            get { return _m; }
            set
            {
                if (value == 3)
                    MovingMethod = 1;
                else
                    MovingMethod = 2;
            }
        }
    }

    public class CsvPackageImportMap : ClassMap<CsvPackageImport>
    {
        public CsvPackageImportMap()
        {
            Map(m => m.CreatedOn).Name("");
            Map(m => m.ShippingCode).Name("mvd");
            Map(m => m.CustomerName).Name("khach");
            Map(m => m._method).Name("tuyen");
            Map(m => m.Note).Name("ghichu");
        }
    }
}
