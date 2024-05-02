namespace KGQT.Models.temp
{
    public class ExcelModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

    }

    public class tempExport
    {
        public string Value { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

    }

    public class tmpExportv2
    {
        public string Code { get; set; }
        public string Username { get; set; }
        public string Method { get; set; }
        public string Weight { get; set; }
        public string Date { get; set; }
    }
}
