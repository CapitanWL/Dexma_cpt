using System.Security.Cryptography;

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
    }
}
