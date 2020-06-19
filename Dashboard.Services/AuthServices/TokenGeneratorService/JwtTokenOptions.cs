namespace Dashboard.Services.AuthServices
{
    /// <summary>
    /// JWT Token options
    /// </summary>
    public class JwtTokenOptions
    {
        /// <summary>
        /// Issuer
        /// </summary>
        public string issuer { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string audience { get; set; }

        /// <summary>
        /// AccessTokenExpiration in minutes
        /// </summary>
        public int AccessTokenExpirationInMinutes { get; set; }
    }
}
