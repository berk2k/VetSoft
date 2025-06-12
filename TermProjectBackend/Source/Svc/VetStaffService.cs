using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using TermProjectBackend.Source.Svc.Interfaces;

namespace TermProjectBackend.Source.Svc
{
    public class VetStaffService : IVetStaffService
    {
        private readonly VetDbContext _vetDb;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;
        public VetStaffService(VetDbContext vetDb, ITokenService tokenService, IPasswordHasher passwordHasher, ILogger<UserService> logger)
        {
            _vetDb = vetDb;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _logger = logger;
        }

        public int getStaffId(VetStaff vetStaff)
        {
            return vetStaff.Id;
        }


        public async Task<LoginResponseVetStaffDTO> Login(LoginRequestVetStaffDTO loginRequestDTO)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginRequestDTO.Email);

            var user = await _vetDb.VetStaff.SingleOrDefaultAsync(u => u.Email == loginRequestDTO.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: Invalid credentials for email {Email}", loginRequestDTO.Email);
                return FailedLoginResponse();
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(loginRequestDTO.Password, user.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Incorrect password for email {Email}", loginRequestDTO.Email);
                return FailedLoginResponse();
            }

            _logger.LogInformation("Login successful for user {Email} (ID: {Id})", user.Email, user.Id);

            var tokenString = _tokenService.GenerateToken(user.Id, user.Email);
            //var refreshTokenResponse = await _tokenService.CreateRefreshTokenAsync(user.Id);

            return new LoginResponseVetStaffDTO
            {
                Token = tokenString,
                //RefreshToken = refreshTokenResponse.RefreshToken,
                //RefreshTokenExpiryDate = refreshTokenResponse.ExpiryDate,
                APIUser = SanitizeUser(user)
            };
        }

        private LoginResponseVetStaffDTO FailedLoginResponse()
        {
            return new LoginResponseVetStaffDTO
            {
                Token = "",
                //RefreshToken = "",
                APIUser = null,
            };
        }

        private VetStaff SanitizeUser(VetStaff user)
        {
            user.Password = "";
            return user;
        }
        public VetStaff CreateVetStaff(CreateNewStaffDTO vetStaffDTO)
        {
            var hashPassword = _passwordHasher.HashPassword(vetStaffDTO.Password);
            VetStaff vetStaff = new VetStaff()
            {
                Email = vetStaffDTO.Email,
                Name = vetStaffDTO.Name,
                Password = hashPassword,
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

        public List<VetStaff> GetAllStaff(int page, int pageSize)
        {
            return _vetDb.VetStaff
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToList();
        }
    }
}
