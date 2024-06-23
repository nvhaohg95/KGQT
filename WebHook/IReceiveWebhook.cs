using System.Net;

namespace KGQT.WebHook
{
    public interface IReceiveWebhook
    {
        public Task<HttpStatusCode> ReceiveData(string requestBody);
    }
}
