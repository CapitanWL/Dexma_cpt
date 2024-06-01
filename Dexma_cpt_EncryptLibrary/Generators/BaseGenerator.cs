using System.Security.Cryptography;
using System.Text;

namespace Dexma_cpt_EncryptLibrary
{
    public static class BaseGenerator
    {
        public static byte[] SaltGenerator()
        {
            byte[] salt = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string GenerateRandomString()
        {
            Random Random = new();
            string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            int length = Random.Next(40, 80 + 1);
            StringBuilder stringBuilder = new(length);

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(Chars[Random.Next(Chars.Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}
