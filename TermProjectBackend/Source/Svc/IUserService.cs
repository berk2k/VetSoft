using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public interface IUserService
    {
        bool IsUserUnique(string userName);

        public LoginResponseDTO Login(LoginRequestDTO loginReguestDTO);

        public User Register(RegisterationRequestDTO reqisterationRequestDTO);

    }
}
