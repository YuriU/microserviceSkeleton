namespace WebApi.Settings
{
    public class AuthSettings
    {
        public string ClientId { get; set; }
        
        public string ClientSecretId { get; set; }
        
        public string ExternalIdentityUrl { get; set; }
        
        public string Audience { get; set; }
        
        public string Authority { get; set; }
    }
}