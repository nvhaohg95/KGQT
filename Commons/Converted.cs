using KGQT.Base;
using Newtonsoft.Json;
using System.Globalization;

namespace KGQT.Commons
{
    public static class Converted
    {
        public static int ToInt(int? s)
        {
            if (!s.HasValue) return 0;

            return s.Value;
        }
        public static int ToInt(object s)
        {
            if (s == null) return 0;

            return Convert.ToInt32(s);
        }
        public static int ToInt(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;

            return Convert.ToInt32(s);
        }

        public static double ToDouble(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;

            return Convert.ToDouble(s);
        }
        public static double ToDouble(object s)
        {
            if (s == null) return 0;

            return Convert.ToDouble(s);
        }

        public static double ToDouble(double? s)
        {
            if (!s.HasValue) return 0;

            return s.Value;
        }

        public static DateTime ToDate(string s)
        {
            if (string.IsNullOrEmpty(s)) return DateTime.Now;
            DateTime dt = DateTime.Now;
            try
            {
                dt = Convert.ToDateTime(s);
            }
            catch (Exception ex)
            {
                Log.Error("Parse Datetime", JsonConvert.SerializeObject(ex));
                //var arr = s.Split("/", StringSplitOptions.RemoveEmptyEntries);
                //if (arr.Length == 3)
                //{
                //    int d = Convert.ToInt32(arr[0]);
                //    int m = Convert.ToInt32(arr[1]);
                //    var arrY = arr[3].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                //    int y = Convert.ToInt32(arrY[0]);
                //    if (CultureInfo.CurrentCulture.Name.ToLower() == "en-US")
                //    {

                //    }


                //}
            }
            return dt;
        }

        public static DateTime ToDate(object s)
        {
            if (s == null) return DateTime.Now;
            return Convert.ToDateTime(s);
        }

        public static DateTime ToDate(DateTime? d)
        {
            if (!d.HasValue) return DateTime.Now;
            return d.Value;
        }

        public static string Date2String(DateTime? d, bool withHour)
        {
            if (d == null) return "";
            if (withHour)
                return d.Value.ToString("dd/MM/yyyy HH:mm:ss");
            else return d.Value.ToString("dd/MM/yyyy");
        }
        public static string Double2String(double? str)
        {
            if (str == null) return "0";

            if (str.ToString().IndexOf(".") > 0)
            {
                var split = str.ToString().Split('.');
                if (split[1].Length > 2)
                    return Math.Round(str.Value, 1).ToString().Replace(",", ".");
            }
            return str.ToString().Replace(",", ".");
        }

        public static string String2Double(string value)
        {
            if (string.IsNullOrEmpty(value)) return "0";
            double num = Convert.ToDouble(value);
            if (value.IndexOf(".") > 0)
            {
                var split = value.Split('.');
                if (split[1].Length > 2)
                    return Math.Round(num, 1).ToString().Replace(",", ".");
            }
            return value.Replace(",", ".");
        }

        public static string String2Money(string value)
        {
            if (string.IsNullOrEmpty(value)) return "0";
            double num = Convert.ToDouble(value);
            if (value.IndexOf(".") > 0)
                return string.Format("{0:N0}", Math.Round(num, 1)).Replace(",", ".");
            return string.Format("{0:N0}", num).Replace(",", ".");
        }

        public static string Double2Money(double? value)
        {
            if (value == null) return "0";
            double num = Convert.ToDouble(value);
            if (value.ToString().IndexOf(".") > 0)
                return string.Format("{0:N0}", Math.Round(num, 1)).Replace(",", ".");
            return string.Format("{0:N0}", value).Replace(",", ".");
        }

    }
}
