namespace KGQT.Models.temp
{
    public class DataReturnModel<T>
    {
        public bool IsError { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string TypeRequest { get; set; }
        public T Data { get; set; }

    }
}
