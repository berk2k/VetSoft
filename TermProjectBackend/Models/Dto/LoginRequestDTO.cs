using System.ComponentModel.DataAnnotations;

namespace TermProjectBackend.Models.Dto
{
    public class LoginRequestDTO
    {
        
        public string UserName { get; set; }

        
        public string Password { get; set; }
    }
}
