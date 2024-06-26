using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace KGQT.Models.temp
{
    public class Zalo
    {
        public string PerUrl { get; set; }
        public string GetTokenUrl { get; set; }
        public string AppID { get; set; }
        public string Secret { get; set; }
        public string Redirect_Uri { get; set; }
    }


    #region Follower
    public class DataFollow
    {
        public int total { get; set; }
        public List<Follower> users { get; set; }
    }

    public class Follower
    {
        public string user_id { get; set; }
    }

    public class Followers
    {
        public DataFollow data { get; set; }
        public int error { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region User info
    public class RootUserInfo
    {
        public UserInfo data { get; set; }
        public int error { get; set; }
        public string message { get; set; }
    }
    public class UserInfo
    {
        public string avatar { get; set; }
        public int user_gender { get; set; }
        public string user_id { get; set; }
        public string user_id_by_app { get; set; }
        public bool is_sensitive { get; set; }
        public string display_name { get; set; }
        public int birth_date { get; set; }
    }


    #endregion

    #region Message
    public class MessageInfo
    {
        public string message_id { get; set; }
        public string user_id { get; set; }
    }

    public class SendMessageResponse
    {
        public MessageInfo data { get; set; }
        public int error { get; set; }
        public string message { get; set; }
    }
    #endregion

    #region WebHook
    public class Info
    {
        public string address { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string name { get; set; }
        public string ward { get; set; }
    }

    public class Recipient
    {
        public string id { get; set; }
    }

    public class WebHookReceive
    {
        public string app_id { get; set; }
        public string user_id_by_app { get; set; }
        public string event_name { get; set; }
        public string timestamp { get; set; }
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public WebHook_Follower follower { get; set; }
        public Info info { get; set; }
    }

    public class Sender
    {
        public string id { get; set; }
    }
    
    public class WebHook_Follower
    {
        public string id { get; set; }
    }

    #endregion
}
