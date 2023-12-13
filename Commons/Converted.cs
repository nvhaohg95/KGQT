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
            return Convert.ToDateTime(s);
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
    }
}
