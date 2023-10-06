using KGQT.Base;
using KGQT.Commons;
using KGQT.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace KGQT.Business.Base
{
    public static class BusinessBase
    {
        public static int Add<T>(T entity, string key = "") where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {

                    if (!string.IsNullOrEmpty(key))
                    {
                        var type = typeof(T);
                        var v = type?.GetProperty(key)?.GetValue(entity, null);
                        var lamdaExp = GenExpressionByString<T>(key, v);
                        var exist = db.Set<T>().Any(lamdaExp);
                        if (exist)
                            return -1;
                    }

                    db.Add(entity);
                    if (db.SaveChanges() > 0)
                    {
                        Log.Info("Thêm mới thành công: " + typeof(T).Name, JsonConvert.SerializeObject(entity));
                        var propertyInfo = entity.GetType().GetProperty("ID");
                        var value = propertyInfo?.GetValue(entity, null);
                        if (value != null)
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

        public static bool Update<T>(T entity) where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    db.Set<T>().Update(entity);
                    if (db.SaveChanges() > 0)
                    {
                        Log.Info("Cập nhật thành công: " + typeof(T).Name, JsonConvert.SerializeObject(entity));
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Cập nhật không thành công :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return false;
                }
            }
            return true;
        }
        public static bool Remove<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    var entity = GetOne(predicate);
                    db.Set<T>().Remove(entity);
                    if (db.SaveChanges() > 0)
                    {
                        Log.Info("Xóa nhật thành công: " + typeof(T).Name, JsonConvert.SerializeObject(entity));
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Xóa không thành công :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return false;
                }
            }
            return true;
        }

        public static bool Remove<T>(T entity) where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    db.Set<T>().Remove(entity);
                    if (db.SaveChanges() > 0)
                    {
                        Log.Info("Xóa nhật thành công: " + typeof(T).Name, JsonConvert.SerializeObject(entity));
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Xóa không thành công :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return false;
                }
            }
            return true;
        }


        public static T GetOne<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    return db.Set<T>().AsQueryable().FirstOrDefault(predicate);

                }
                catch (Exception ex)
                {
                    Log.Error("Lỗi query, table :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return null;
                }
            }
        }

        public static T GetFirst<T>() where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    return db.Set<T>().FirstOrDefault();

                }
                catch (Exception ex)
                {
                    Log.Error("Lỗi query, table :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return null;
                }
            }
        }

        public static IList<T> GetList<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    return db.Set<T>().AsQueryable().Where(predicate).ToList();

                }
                catch (Exception ex)
                {
                    Log.Error("Lỗi query, table :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return null;
                }
            }
        }

        public static bool Exist<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            using (var db = new nhanshiphangContext())
            {
                try
                {
                    return db.Set<T>().AsQueryable().Any(predicate);

                }
                catch (Exception ex)
                {
                    Log.Error("Lỗi query, table :" + typeof(T).Name, JsonConvert.SerializeObject(ex));
                    return false;
                }
            }
        }

        #region Genarate
        public static string GenQueryStringByPrimaryKey<T>(T entity) where T : class
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
            return String.Format(predicate, datavalues.ToArray());
        }

        public static Func<T, bool> GenExpressionByString<T>(string sKey, object dataValue) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "o");
            Type type = typeof(T);
            var comparison = Expression.Equal(Expression.Property(parameter, type.GetProperty(sKey)), Expression.Constant(dataValue));

            Func<T, bool> expression = Expression.Lambda<Func<T, bool>>(comparison, parameter).Compile();
            return expression;
        }
        #endregion

        #region tracklogship
        public static void TrackLogShippingOrder(int userId, int shipId, string content, int type, string userName)
        {
            using (var db = new nhanshiphangContext())
            {
                var t = new tbl_TrackShippingOrder();
                t.UID = userId;
                t.ShipID = shipId;
                t.Context = content;
                t.Type = type;
                t.CreatedBy = userName;
                t.CreatedOn = DateTime.Now;
                db.tbl_TrackShippingOrders.Add(t);
                db.SaveChanges();
            }

        }
        #endregion
    }
}
