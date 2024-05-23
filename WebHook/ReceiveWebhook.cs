using Serilog;
using System.Net;

namespace KGQT.WebHook
{
    public class ReceiveWebhook : IReceiveWebhook
    {
        public Task<HttpStatusCode> UpdateTransactionStatus(string json)
        {
            Log.Information("Webhook:" + json);
            return Task.FromResult(HttpStatusCode.OK);
        }
    }
}
