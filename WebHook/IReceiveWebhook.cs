namespace KGQT.WebHook
{
    public interface IReceiveWebhook
    {
        public Task<string> UpdateTransactionStatus(string requestBody);
    }
}
