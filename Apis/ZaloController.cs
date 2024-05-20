using KGQT.Base;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net.Http.Headers;
using System.Net.Security;

namespace KGQT.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloController : ControllerBase
    {
        [HttpGet]
        [Route("callback")]
        public async Task<bool> CallbackZaloAsync()
        {
            var code = Request.Query["code"].ToString();
            string appID = Config.Zalo.AppID;
            string secret = Config.Zalo.Secret;
            string url = Config.Zalo.GetTokenUrl;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpContent requestContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("app_id", appID),
                    new KeyValuePair<string, string>("code", code)
                    });

                    client.DefaultRequestHeaders.Add("secret_key", secret);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    HttpResponseMessage response = await client.PostAsync(url, requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
            return true;
        }

        [HttpGet]
        public void CreateCode()
        {
            string url = string.Format(Config.Zalo.PerUrl, Config.Zalo.AppID, Config.Zalo.Redirect_Uri);
            Redirect(url);
        }
    }
}
