namespace KGQT.Models.temp
{
    public class OrderDetails
    {
        public tbl_ShippingOrder Order { get; set; }
        public IEnumerable<tbl_Package> Packs { get; set; }
        public IEnumerable<tbl_ShippingOrderDeclaration> Declarations { get; set; }
    }
}
