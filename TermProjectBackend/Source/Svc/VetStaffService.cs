using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;

namespace TermProjectBackend.Source.Svc
{
    public class VetStaffService : IVetStaffService
    {
        private readonly VetDbContext _vetDb;
        private string secretKey;
        public VetStaffService(VetDbContext vetDb, IConfiguration configuration)
        {
            _vetDb = vetDb;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public int getStaffId(VetStaff vetStaff)
        {
            return vetStaff.Id;
        }


        public LoginResponseVetStaffDTO Login(LoginRequestVetStaffDTO loginReguestDTO)
        {
            var staff = _vetDb.VetStaff.FirstOrDefault(u => u.Email == loginReguestDTO.Email && u.Password == loginReguestDTO.Password);

            if (staff == null)
            {
                return new LoginResponseVetStaffDTO()
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
                    new Claim("UserId", staff.Id.ToString()),
                    new Claim(ClaimTypes.Name,staff.Id.ToString()),
                    new Claim(ClaimTypes.Role,staff.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseVetStaffDTO loginResponseDTO = new LoginResponseVetStaffDTO()
            {
                Token = tokenHandler.WriteToken(token),
                APIUser= staff,
                UserId = staff.Id
            };

            return loginResponseDTO;

        }
        public VetStaff CreateVetStaff(CreateNewStaffDTO vetStaffDTO)
        {
            VetStaff vetStaff = new VetStaff()
            {
                Email = vetStaffDTO.Email,
                Name = vetStaffDTO.Name,
                Password = vetStaffDTO.Password,
                Role = vetStaffDTO.Role,
            };

            _vetDb.Add(vetStaff);
            _vetDb.SaveChanges();
            return vetStaff;
        }

        public void DeleteVetStaff(int id)
        {
            VetStaff vetStaff = _vetDb.VetStaff.Find(id);

            if (vetStaff == null)
            {
               
                throw new InvalidOperationException($"Staff with ID {id} not found.");
            }

            _vetDb.VetStaff.Remove(vetStaff);

            _vetDb.SaveChanges();
        }

        public void UpdateVetStaff(UpdateVetStaffDTO vetStaffDTO)
        {
            var staffToUpdate = _vetDb.VetStaff.FirstOrDefault(i => i.Id == vetStaffDTO.Id);

            if (staffToUpdate == null)
            {

                throw new InvalidOperationException($"Staff with ID {vetStaffDTO.Id} not found.");
            }

            if (staffToUpdate != null)
            {

                staffToUpdate.Name = vetStaffDTO.Name;
                staffToUpdate.Password = vetStaffDTO.Password;

                _vetDb.SaveChanges();
            }
        }
    }
}
