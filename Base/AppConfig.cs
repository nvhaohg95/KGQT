using Org.BouncyCastle.Pqc.Crypto.Lms;
using static System.Net.WebRequestMethods;

namespace KGQT.Base
{
    public static class Config
    {
        static IConfiguration _configuration = null;
        private static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .Build();

                return _configuration;
            }
        }

        static Settings _settings = null;
        public static Settings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = Configuration.GetSection("AppSettings").Get<Settings>();

                if (_settings == null)
                    _settings = new Settings();

                return _settings;
            }
        }

        private static IConfigurationSection _connections = null;
        public static IConfigurationSection Connections
        {
            get
            {
                if (_connections == null)
                    _connections = Configuration.GetSection("ConnectionStrings");

                return _connections;
            }
        }

    }

    public class Settings
    {
        public string LogPath { get; set; }
        public string ApiKey { get; set; }

        public string ApiUrl { get; set; }
    }
}
