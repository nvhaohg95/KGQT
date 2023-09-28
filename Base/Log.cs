using Microsoft.AspNetCore.Routing;
using System.Configuration;

namespace KGQT.Base
{
    public static class Log
    {
        private static void Instance(string type, string title, string message)
        {
            string path = Config.Settings.LogPath;
            string fileName = DateTime.Now.ToString("ddMMyyyy");
            path = Path.Combine(path, fileName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = path + "/log.txt";
            using (StreamWriter fs = new StreamWriter(path,true))
            {
                fs.WriteLine($"[{type}]-[{DateTime.Now}] -{title} : {message}");
            }
        }

        public static void Error(string title, string message)
        {
            Instance("ERROR", title, message);
        }

        public static void Info(string title, string message)
        {
            Instance("INFO", title, message);
        }
    }
}
