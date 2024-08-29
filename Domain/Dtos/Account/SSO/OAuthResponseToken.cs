namespace Domain.Dtos.Account.SSO
{
    public class OAuthResponseToken
    {
        public string access_token { get; set; }
        public string id_token { get; set; }
        public string scope { get; set; }
        public int expires_in { get; set; }
        public object refresh_token { get; set; }
        public string token_type { get; set; }
    }
}
