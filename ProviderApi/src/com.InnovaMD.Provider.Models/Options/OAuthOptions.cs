namespace com.InnovaMD.Provider.Models.Common
{
    public class OAuthOptions
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
        public string[] AllowOrigins { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public int ClockSkew { get; set; }
    }
}
