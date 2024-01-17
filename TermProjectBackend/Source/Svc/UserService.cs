using Microsoft.IdentityModel.Tokens;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TermProjectBackend.Source.Svc
{
    public class UserService : IUserService
    {
        private readonly VetDbContext _vetDb;
        private string secretKey;

        public UserService(VetDbContext vetDb, IConfiguration configuration)
        {
            _vetDb = vetDb;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            
        }

        public int getUserId(User user)
        {
            return user.Id;
        }

        public User GetUserInformationById(int id)
        {
            // Assuming User has an Id property
            User retrievedUser = _vetDb.Users.Find(id);

            // You can modify the logic based on your actual data model
            if (retrievedUser == null)
            {
                // Handle the case where the user is not found
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            // Optionally, you can exclude sensitive information before returning the user
            // retrievedUser.SensitiveProperty = null;

            return retrievedUser;
        }

        public bool IsUserUnique(string userName)
        {
            var user  = _vetDb.Users.FirstOrDefault(u => u.UserName == userName);

            if(user == null)
            {
                return true;
            }

            return false;
        }

        public LoginResponseDTO Login(LoginRequestDTO loginReguestDTO)
        {
            var user = _vetDb.Users.FirstOrDefault(u => u.UserName == loginReguestDTO.UserName && u.Password == loginReguestDTO.Password);

            if(user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    APIUser = null
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Id.ToString()),
                    new Claim(ClaimTypes.Role,user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                APIUser = user,
                UserId = user.Id
            };

            return loginResponseDTO;

        }

        public User Register(RegisterationRequestDTO reqisterationRequestDTO)
        {
            User user = new User()
            {
                UserName = reqisterationRequestDTO.UserName,
                Name = reqisterationRequestDTO.Name,
                Password = reqisterationRequestDTO.Password,
                Role = reqisterationRequestDTO.Role,
            };

            _vetDb.Users.Add(user);
            _vetDb.SaveChanges();

            user.Password = "";

            return user;

        }
    }
}
