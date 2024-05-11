using KGQT.Models.temp;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace KGQT.Commons
{
    public class CookieService
    {
        private HttpContext _context;
        public HttpContext HttpContext { get { return _context; } }
        public CookieService(HttpContext httpContext)
        {
                _context = httpContext;
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return;
            HttpContext.Response.Cookies.Delete(key);
            CookieOptions tkcookie = new CookieOptions();
            tkcookie.Expires = DateTime.Now.AddDays(30);
            string tkck = Helper.Base64Encode(value);
            HttpContext.Response.Cookies.Append(key, tkck, tkcookie);
        }

        public string Get(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var value = HttpContext.Request.Cookies[key];
                if (!string.IsNullOrEmpty(value))
                {
                    return Helper.Base64Decode(value);
                }
            }
            return string.Empty;
        }

        public void Remove(string key)
        {
            HttpContext.Response.Cookies.Delete(key);

        }
    }
}
