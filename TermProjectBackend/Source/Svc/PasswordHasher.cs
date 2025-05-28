using System.Security.Cryptography;
using TermProjectBackend.Source.Svc.Interfaces;

namespace TermProjectBackend.Source.Svc
{
  
    public class PasswordHasher : IPasswordHasher
    {
        // algotihm : PBKDF2 (SHA256)
        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            var hash = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hashBytes = hash.GetBytes(32);

            byte[] result = new byte[48];
            Buffer.BlockCopy(salt, 0, result, 0, 16);
            Buffer.BlockCopy(hashBytes, 0, result, 16, 32);

            return Convert.ToBase64String(result);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

            var hash = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hashToCompare = hash.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hashToCompare[i])
                    return false;
            }

            return true;
        }
    }

}
