using KGQT.Models;

namespace KGQT.Business
{
    public static class FeeWeight
    {
        #region Get List FeeWeight
        public static List<tbl_FeeWeight> GetList()
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_FeeWeights.ToList();
            }
        }
        #endregion
        #region Get Detail FeeWeight
        public static tbl_FeeWeight Get(int id)
        {
            using (var db = new nhanshiphangContext())
            {
                return db.tbl_FeeWeights.FirstOrDefault(x => x.ID == id);
            }
        }
        #endregion
    }
}
