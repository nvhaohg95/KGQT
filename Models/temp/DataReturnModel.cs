namespace KGQT.Models.temp
{
    public class DataReturnModel<T>
    {
        public bool IsError { get; set; }
        public string Key { get; set; }
        public string Message { get; set; }
        public int Type { get; set; } // 1:validate , 2:Alert/noti
        public T Data { get; set; }
    }
}
