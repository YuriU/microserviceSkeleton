namespace WebApi.Settings
{
    public class AuthSettings
    {
        public string TokenUrl { get; set; }
        
        public string AuthorizationUrl { get; set; }
        
        public string Audience { get; set; }
        
        public string Authority { get; set; }
    }
}