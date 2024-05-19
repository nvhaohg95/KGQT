using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace KGQT.Models.temp
{
    public class Zalo
    {
        public string PerUrl { get; set; }
        public string GetTokenUrl { get; set; }
        public string AppID { get; set; }
        public string Secret { get; set; }
        public string Redirect_Uri { get;set; }
    }
}
