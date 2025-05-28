using TermProjectBackend.Context;
using TermProjectBackend.Models;
using TermProjectBackend.Models.Dto;
using Microsoft.Extensions.Logging;
using TermProjectBackend.Source.Svc.Interfaces;

namespace TermProjectBackend.Source.Svc
{
    public class UserService : IUserService
    {
        private readonly VetDbContext _vetDb;
        private readonly ILogger<UserService> _logger;
        private readonly ITokenService _tokenService;
        public UserService(VetDbContext vetDb, IConfiguration configuration, ILogger<UserService> logger, ITokenService tokenService)
        {
            _vetDb = vetDb;
            _logger = logger;
            _tokenService = tokenService;
            _logger.LogInformation("UserService initialized");
            
        }

        public int getUserId(User user)
        {
            _logger.LogDebug("getUserId called for user: {UserName}", user.UserName);
            return user.Id;
        }

        public int getUserIdByName(string userName)
        {
            _logger.LogInformation("getUserIdByName called for username: {UserName}", userName);

            var user = _vetDb.Users.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                _logger.LogDebug("getUserIdByName: Found user ID {UserId} for username {UserName}", user.Id, userName);
                return user.Id;
            }
            else
            {
                _logger.LogWarning("getUserIdByName: User not found for username {UserName}", userName);
                throw new ArgumentException("User not found", nameof(userName));
            }
        }

        public User GetUserInformationById(int id)
        {
            _logger.LogInformation("GetUserInformationById called for ID: {UserId}", id);

            User retrievedUser = _vetDb.Users.Find(id);

            if (retrievedUser == null)
            {
                _logger.LogWarning("GetUserInformationById: User with ID {UserId} not found", id);
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            _logger.LogDebug("GetUserInformationById: Successfully retrieved user {UserName} (ID: {UserId})",
                retrievedUser.UserName, id);

            
            retrievedUser.Password = "";

            return retrievedUser;
        }

        public bool IsUserUnique(string userName)
        {
            _logger.LogInformation("IsUserUnique called for username: {UserName}", userName);

            var user = _vetDb.Users.FirstOrDefault(u => u.UserName == userName);

            bool isUnique = (user == null);
            _logger.LogDebug("IsUserUnique result for {UserName}: {IsUnique}", userName, isUnique);

            return isUnique;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Login attempt for username: {UserName}", loginRequestDTO.UserName);

            var user = _vetDb.Users.FirstOrDefault(u =>
                u.UserName == loginRequestDTO.UserName &&
                u.Password == loginRequestDTO.Password);

            if (user == null)
            {
                _logger.LogWarning("Login failed: Invalid credentials for username {UserName}", loginRequestDTO.UserName);
                return new LoginResponseDTO()
                {
                    Token = "",
                    RefreshToken = "",
                    APIUser = null,
                    UserId = 0
                };
            }

            _logger.LogInformation("Login successful for user {UserName} (ID: {UserId})", user.UserName, user.Id);


            var tokenString = _tokenService.GenerateToken(user.Id, user.UserName);
            var refreshTokenResponse = await _tokenService.CreateRefreshTokenAsync(user.Id);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenString,
                RefreshToken = refreshTokenResponse.RefreshToken,
                RefreshTokenExpiryDate = refreshTokenResponse.ExpiryDate,
                APIUser = user,
                UserId = user.Id
            };


            if (loginResponseDTO.APIUser != null)
            {
                loginResponseDTO.APIUser.Password = "";
            }

            return loginResponseDTO;
        }

        public User Register(RegisterationRequestDTO registerationRequestDTO)
        {
            _logger.LogInformation("Register called for username: {UserName}", registerationRequestDTO.UserName);

            User user = new User()
            {
                UserName = registerationRequestDTO.UserName,
                Name = registerationRequestDTO.Name,
                Password = registerationRequestDTO.Password,
                Role = registerationRequestDTO.Role,
            };

            _logger.LogDebug("Register: Adding new user {UserName} with role {Role}", user.UserName, user.Role);

            _vetDb.Users.Add(user);
            _vetDb.SaveChanges();

            _logger.LogInformation("Register successful: Created user ID {UserId} for {UserName}", user.Id, user.UserName);

            // Güvenlik için password temizle
            user.Password = "";

            return user;
        }

        public void DeleteAccount(int id)
        {
            _logger.LogInformation("DeleteAccount called for user ID: {UserId}", id);

            User userToDelete = _vetDb.Users.Find(id);

            if (userToDelete == null)
            {
                _logger.LogWarning("DeleteAccount failed: User with ID {UserId} not found", id);
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            _logger.LogInformation("DeleteAccount: Deleting user {UserName} (ID: {UserId})", userToDelete.UserName, id);

            // Delete associated pets
            var userPets = _vetDb.Pets.Where(p => p.OwnerID == id).ToList();
            _logger.LogDebug("DeleteAccount: Removing {PetCount} pets for user ID {UserId}", userPets.Count, id);
            _vetDb.Pets.RemoveRange(userPets);

            // Delete associated appointments
            var userAppointments = _vetDb.Appointments.Where(a => a.ClientID == id).ToList();
            _logger.LogDebug("DeleteAccount: Removing {AppointmentCount} appointments for user ID {UserId}", userAppointments.Count, id);
            _vetDb.Appointments.RemoveRange(userAppointments);

            // Delete the user
            _vetDb.Users.Remove(userToDelete);

            // Save changes to the database
            _vetDb.SaveChanges();

            _logger.LogInformation("DeleteAccount successful: User {UserName} (ID: {UserId}) and all associated data deleted",
                userToDelete.UserName, id);
        }

        public List<string> GetAllUserNames()
        {
            _logger.LogInformation("GetAllUserNames called");

            var userNames = _vetDb.Users.Select(u => u.UserName).ToList();

            _logger.LogDebug("GetAllUserNames: Retrieved {Count} usernames", userNames.Count);

            return userNames;
        }
    }
}