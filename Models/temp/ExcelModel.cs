namespace KGQT.Models.temp
{
    public class ExcelModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

    }

    public class tempExportChina
    {
        public int MovingMethod { get; set; }
        public string Value { get; set; }
        public int Column { get; set; }
        public int Date { get; set; }
        public int Row { get; set; }
        public bool Mapped { get; set; }

    }
}
