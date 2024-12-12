using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace IsBankMvc.Business.Helpers
{
    public static class CryptographyHelper
    {
        private const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
        private const int DerivationIterations = 1000;
        public static string ComputeSHA256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashedBytes = sha256.ComputeHash(inputBytes);

                var builder = new StringBuilder();
                for (var i = 0; i < hashedBytes.Length; i++) builder.Append(hashedBytes[i].ToString("x2"));

                return builder.ToString();
            }
        }
        public static string GenerateSalt(int length = 32)
        {
            var salt = new byte[length];

            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public static string Hash(string input, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var bytes = KeyDerivation.Pbkdf2(
                input, saltBytes, KeyDerivationPrf.HMACSHA512, 10000, 16);

            return Convert.ToBase64String(bytes);
        }

        public static bool Verify(string input, string hash, string salt)
        {
            try
            {
                var saltBytes = Convert.FromBase64String(salt);
                var bytes = KeyDerivation.Pbkdf2(input, saltBytes, KeyDerivationPrf.HMACSHA512, 10000, 16);
                var encoded = Convert.ToBase64String(bytes);
                return hash.Equals(encoded);
            }
            catch
            {
                return false;
            }
        }
        public static string EncryptSHA512(string text, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key must have valid value.", nameof(key));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("The text must have valid value.", nameof(text));

            byte[] buffer = Encoding.UTF8.GetBytes(text);
            SHA512CryptoServiceProvider hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new ArgumentException("Parameter must not be null.", nameof(aes));

                aes.Key = aesKey;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(buffer))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    var result = resultStream.ToArray();
                    var combined = new byte[aes.IV.Length + result.Length];
                    Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                    Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

                    return Convert.ToBase64String(combined);
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy(int blockSize = 256)
        {
            var randomBytes = new byte[blockSize / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }
    }
}
