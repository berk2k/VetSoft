namespace TermProjectBackend.Models.Dto
{
    public class LoginResponseDTO
    {
        public User APIUser { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; } 
        public DateTime RefreshTokenExpiryDate { get; set; } 
    }
}
