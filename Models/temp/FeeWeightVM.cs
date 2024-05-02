namespace KGQT.Models.temp
{
    public class FeeWeightVM
    {
        public List<tbl_FeeWeight> ListFeeWeight { get; set; }
        public tbl_FeeWeight FeeWeight { get; set; }
        public FeeWeightVM()
        {
            this.ListFeeWeight = new List<tbl_FeeWeight>();
            this.FeeWeight = new tbl_FeeWeight();
        }
    }
}
