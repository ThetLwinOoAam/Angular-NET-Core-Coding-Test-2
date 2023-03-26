namespace WebApi.Models.Tokens
{
    public class AccessTokenConfig
    {
        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string SecretKey { get; set; }

        public string AccessTokenExpiration { get; set; }

        public string RefreshTokenExpiration { get; set; }

    }
}
