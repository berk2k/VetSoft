namespace TermProjectBackend.Models
{
    public class TokenRefreshResult
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public string Error { get; set; }
    }
}
