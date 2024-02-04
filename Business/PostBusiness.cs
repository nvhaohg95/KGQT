using KGQT.Models;
using KGQT.Models.temp;

namespace KGQT.Business
{
    public static class PostBusiness
    {
        public static DataReturnModel<bool> Insert(tbl_Posts data)
        {
            using (var db = new nhanshiphangContext())
            {
                db.Add(data);
                int save = db.SaveChanges();
                if(save > 0)
                    return new DataReturnModel<bool>() { IsError = false, Data = true, Message = "Tạo bài viết thành công!" };
                else
                    return new DataReturnModel<bool>() { IsError = true, Data = false, Message = "Tạo bài viết không thành công!" };
            }
        }
    }
}
