using KGQT.Business;
using Newtonsoft.Json;
using Serilog;
using ILogger = Serilog.ILogger;

namespace KGQT.Commons
{
    public static class Converted
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));

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
        
        public static double Double(this string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;

            return Convert.ToDouble(s);
        }
        public static double ToDouble(object s)
        {
            if (s == null) return 0;

            return Convert.ToDouble(s.ToString());
        }

        public static double ToDouble(double? s, int period = 1)
        {
            if (s == null) return 0;
            var num = Convert.ToDouble(s.ToString());
            return Math.Round(num, period, MidpointRounding.AwayFromZero);
        }
        public static decimal ToDecimal(double? s, int period = 1)
        {
            if (s == null) return 0;
            var num = Convert.ToDecimal(s);
            return Math.Round(num, period, MidpointRounding.AwayFromZero);
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
                _log.Error("Parse Datetime", ex.Message, s);
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

        public static string StringCeiling(object value)
        {
            if (value == null) return "0";
            double num = Convert.ToDouble(value);
            if (value.ToString().IndexOf(".") > -1)
                return Math.Ceiling(num).ToString().Replace(",", ".");
            return value.ToString().Replace(",", ".");
        }

        public static double DoubleCeiling(double? value)
        {
            if (value == null) return 0;
            double num = Convert.ToDouble(value);
            if (value.ToString().IndexOf(".") > -1)
                return Math.Ceiling(num);
            return Convert.ToDouble(value);
        }

        public static string Double2String(double? str)
        {
            if (str == null) return "0";

            if (str.ToString().IndexOf(".") > 0)
            {
                var split = str.ToString().Split('.');
                if (split[1].Length > 2)
                    return Math.Round(str.Value, 1, MidpointRounding.AwayFromZero).ToString().Replace(",", ".");
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
                    return Math.Round(num, 1, MidpointRounding.AwayFromZero).ToString().Replace(",", ".");
            }
            return value.Replace(",", ".");
        }

        public static string String2Money(string value)
        {
            if (string.IsNullOrEmpty(value)) return "0";
            double num = Convert.ToDouble(value);
            if (value.IndexOf(".") > 0)
                return string.Format("{0:N0}", Math.Round(num, 1, MidpointRounding.AwayFromZero)).Replace(".", ",");
            return string.Format("{0:N0}", num).Replace(".", ",");
        }

        public static string Double2Money(double? value)
        {
            if (value == null) return "0";
            double num = Convert.ToDouble(value);
            if (value.ToString().IndexOf(".") > 0)
                return string.Format("{0:N0}", Math.Round(num, 1, MidpointRounding.AwayFromZero)).Replace(".", ",");
            return string.Format("{0:N0}", value).Replace(".", ",");
        }
        public static string Decimal2Money(decimal? value)
        {
            if (value == null) return "0";
            decimal num = Convert.ToDecimal(value);
            if (value.ToString().IndexOf(".") > 0)
                return string.Format("{0:N0}", Math.Round(num, 1, MidpointRounding.AwayFromZero)).Replace(".", ",");
            return string.Format("{0:N0}", value).Replace(".", ",");
        }
    }
}
