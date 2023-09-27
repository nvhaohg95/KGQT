using KGQT.Base;
using KGQT.Models;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace KGQT.Business.Base
{
    public static class BusinessBase
    {
        public static int Add<T>(T entity) where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    db.Add(entity);
                    if (db.SaveChanges() > 0)
                    {
                        Log.Info("Thêm mới thành công: " + typeof(T).Name, JsonConvert.SerializeObject(entity));
                        var propertyInfo = entity.GetType().GetProperty("ID");
                        var value = propertyInfo.GetValue(entity, null);
                        if(value != null)
                        return Int32.Parse(value.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Lỗi thêm mới table :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return -1;
                }
            }
            return -1;
        }

        public static bool Exist<T>(Expression<Func<T,bool>> predicate) where T: class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                  return  db.Set<T>().AsQueryable().Any(predicate);

                }
                catch (Exception ex)
                {
                 Log.Error("Lỗi query, table :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return false;
                }
            }
        }


    }
}
