using DocumentFormat.OpenXml.Drawing.Charts;
using KGQT.Business;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using ILogger = Serilog.ILogger;

namespace KGQT.Commons
{
    public static class Converted
    {
        private static readonly ILogger _log = Log.ForContext(typeof(AccountBusiness));

        public static bool ToBool(this bool? value)
        {
            if (!value.HasValue) return false;

            return value.Value;
        }
        public static int ToInt(int? s, int df = 0)
        {
            if (!s.HasValue) return df;

            return s.Value;
        }
        public static int ToInt(object s, int df = 0)
        {
            if (s == null) return df;

            return Convert.ToInt32(s);
        }
        public static int ToInt(this string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;

            return Convert.ToInt32(s);
        }

        public static double ToDouble(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            s = FormatStringDouble(s);
            double num = Convert.ToDouble(s);
            return Math.Round(num, 1, MidpointRounding.AwayFromZero);
        }

        public static double Double(this string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            s = FormatStringDouble(s);
            double num = Convert.ToDouble(s);
            return Math.Round(num, 1, MidpointRounding.AwayFromZero);
        }
        public static double ToDouble(object value)
        {
            if (value == null) return 0;
            string s = FormatStringDouble(value);
            double num = Convert.ToDouble(s);
            return Convert.ToDouble(s.ToString());
        }

        public static double ToDouble(double? value, int period = 1)
        {
            if (value == null) return 0;
            string s = FormatStringDouble(value);
            double num = Convert.ToDouble(s);
            return Math.Round(num, period, MidpointRounding.AwayFromZero);
        }

        public static decimal ToDecimal(this double? s, int period = 1)
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
            string s = FormatStringDouble(value);
            double num = Convert.ToDouble(s);
            if (value.ToString().IndexOf(".") > -1)
                return Math.Ceiling(num).ToString().Replace(",", ".");
            return value.ToString().Replace(",", ".");
        }

        public static double DoubleCeiling(double? value)
        {
            if (value == null) return 0;
            string s = FormatStringDouble(value);
            double num = Convert.ToDouble(s);
            if (value.ToString().IndexOf(".") > -1)
                return Math.Ceiling(num);
            return Convert.ToDouble(value);
        }

        public static string Double2String(double? str)
        {
            if (str == null) return "0";
            string s = FormatStringDouble(str);
            if (s.IndexOf(".") > 0)
            {
                var split = s.Split('.');
                if (split[1].Length > 2)
                    return Math.Round(str.Value, 1, MidpointRounding.AwayFromZero).ToString().Replace(",", ".");
            }
            return s.Replace(",", ".");
        }

        public static string String2Double(string value)
        {
            if (string.IsNullOrEmpty(value)) return "0";
            value = FormatStringDouble(value);
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
            value = FormatStringDouble(value);
            double num = Convert.ToDouble(value);
            if (value.IndexOf(".") > 0)
                return string.Format("{0:N0}", Math.Round(num, 1, MidpointRounding.AwayFromZero)).Replace(".", ",");
            return string.Format("{0:N0}", num).Replace(".", ",");
        }

        public static string Double2Money(double? value)
        {
            if (value == null) return "0";
            string s = value.ToString();

            double num = Convert.ToDouble(s);
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

        public static string ChangeFormat(this object value, string curr = ".", string to = ",")
        {
            if (value == null)
                return "";
            string v = value.ToString();
            return v.Replace(curr, to);
        }

        public static DateTime ConvertDate(string s)
        {
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    return DateTime.ParseExact(s, "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    string[] split = s.Split("/", StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length >= 3)
                    {
                        int year = DateTime.Now.Year;
                        return new DateTime(year, split[1].ToInt(), split[0].ToInt());
                    }

                }
                catch (Exception ex2)
                {
                    return DateTime.Parse(s);
                }
            }
            return DateTime.MinValue;
        }

        public static string ToBase64(object o)
        {
            if (o == null) return "";
            string s = o.ToString();
            byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes(s);
            string encodedString = Convert.ToBase64String(bytesToEncode);
            return encodedString;
        }

        private static string[] sInclude = new string[] {
            "000001", "000002", "000003", "000004", "000005",
            "000006", "000007", "000008", "000009", "0000010",
            "0000011", "0000012","0000013", "0000014","0000015",
            "0000016", "0000017","0000018", "0000019","0000020",
            "0000021", "0000022","0000023", "0000024","0000025",
            "0000026", "0000027","0000028", "0000029","0000030"
        };
        private static string FormatStringDouble(object value)
        {
            var s = value.ToString();
            var exist = sInclude.FirstOrDefault(x => s.Contains(x));
            if (exist != null)
                s = s.Replace(exist, "");
            string[] spt = s.Split(".", StringSplitOptions.RemoveEmptyEntries);
            if (spt.Length > 1 && spt[1].Length > 3)
            {
                s = spt[0] + "." + spt[1].Substring(0, 2);
            }
            return s;
        }
    }
}
