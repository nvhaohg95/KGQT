using KGQT.Base;
using KGQT.Commons;
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
                    Type type = typeof(T);
					var keys = Helper.GetKeys(type);

					string predicate = "";
					var datavalues = new List<object>();
					for (var i = 0; i < keys.Length; i++)
					{
						var key = keys[i];
						var propertyKey = type.GetProperty(key);
						var proType = propertyKey.PropertyType;
						if (proType != null && proType.IsGenericType)
							proType = proType.GetGenericArguments()[0];
						var val = Helper.GetValue(key, entity);

						var opt = "=";
						if (proType == typeof(Guid))
							opt = ".Equals";

						predicate = predicate + (predicate == "" ? "" : "&&")
									+ $"{key}{opt}(@{i})";

						datavalues.Add(val);
					}
                   // "userName = '1234124' && userName == '2'";
                    string criteria = String.Format(predicate, datavalues.ToArray());

                    var exist = db.Set<T>().AsQueryable().Any(x => x.GetType().GetProperty(keys[0].ToString()) == entity.GetType().GetProperty(keys[0].ToString()));
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
