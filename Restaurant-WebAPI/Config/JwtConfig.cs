namespace Restaurant_WebAPI.Config
{
    public static class JwtConfig
    {
        public const string Issuer = "Restaurant Manager";
        public const string Audience = "Customer";
        public const string Secret = "This-is-the-secret-key-created-by-me-for-secure-access-to-my-app";
    }
}
