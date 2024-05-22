namespace KGQT.WebHook
{
    public class ReceiveWebhook : IReceiveWebhook
    {
        public Task<string> UpdateTransactionStatus(string root)
        {
            string result = "";
            return Task.FromResult(result);
        }
    }
}
