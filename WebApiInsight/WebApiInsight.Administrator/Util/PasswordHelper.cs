using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApiInsight.Administrator
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Generate a hashed password and a salt
        /// </summary>
        /// <param name="password">
        /// Not hashed password
        /// </param>
        public static HashedPassword GetHashedPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            var saltBytes = new byte[0x10];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes);
            }

            var passwordBytes = Encoding.Unicode.GetBytes(password);
            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

            byte[] hashBytes;
            using (var hashAlgorithm = HashAlgorithm.Create("SHA1"))
            {
                hashBytes = hashAlgorithm.ComputeHash(combinedBytes);
            }
            var result = new HashedPassword
            {
                Password = Convert.ToBase64String(hashBytes),
                PasswordSalt = Convert.ToBase64String(saltBytes)
            };
            return result;
        }

        public static bool PasswordsEqual(string hashedPassword, string salt, string password)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var passwordBytes = Encoding.Unicode.GetBytes(password);
            var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();
            byte[] hashBytes;
            using (var hashAlgorithm = HashAlgorithm.Create("SHA1"))
            {
                if (hashAlgorithm == null)
                    return false;
                hashBytes = hashAlgorithm.ComputeHash(combinedBytes);
            }
            var result = hashedPassword == Convert.ToBase64String(hashBytes);
            return result;
        }
    }

    public class HashedPassword
    {
        /// <summary>
        /// The hashed password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The salt
        /// </summary>
        public string PasswordSalt { get; set; }
    }
}