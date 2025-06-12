namespace TermProjectBackend.Models.Dto
{
    public class LoginResponseVetStaffDTO
    {
        public VetStaff APIUser { get; set; }

        public string Token { get; set; }

        //public string RefreshToken { get; set; }
        //public DateTime RefreshTokenExpiryDate { get; set; }
    }
}
