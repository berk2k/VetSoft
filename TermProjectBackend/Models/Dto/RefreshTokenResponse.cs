namespace TermProjectBackend.Models.Dto
{
    public class RefreshTokenResponse
    {
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
