using KGQT.Models;

namespace KGQT.Business
{
    public static class ZaloBusiness
    {
        public static object[] GetPage(string userName, int page = 1, int pageSize = 10)
        {
            using (var db = new nhanshiphangContext())
            {
                List<tbl_ZaloFollewer> datas = new();
                int total = 0;
                int totalPage = 0;
                IQueryable<tbl_ZaloFollewer> query = db.tbl_ZaloFollewers;
                if (!string.IsNullOrEmpty(userName))
                    query = query.Where(x => x.user_id == userName || x.Username == userName || x.phone == userName || x.display_name == userName || x.address == userName);

                total = query.Count();
                if (total > 0)
                {
                    totalPage = Convert.ToInt32(Math.Ceiling((decimal)total / pageSize));
                    datas = query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                }
                return new object[] { datas, total, page };
            }
        }

    }
}
