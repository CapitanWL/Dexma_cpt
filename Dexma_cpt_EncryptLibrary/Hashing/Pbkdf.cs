using System.Security.Cryptography;
using System.Text;

namespace Dexma_cpt_EncryptLibrary
{
    public static class Pbkdf
    {
        public static byte[] PbkdfCreate(string password, byte[] salt)
        {
            using (var keyDerivationFunction = new Rfc2898DeriveBytes(password, salt,
                       ConstForHashing.ITERATIONS, ConstForHashing.HASH_ALGORITHM_NAME))
            {
                byte[] key = keyDerivationFunction.GetBytes(ConstForHashing.PASSWORD_KEY_LENGTH);
                return key;
            }
        }

        public static byte[] PbkdfCreate(byte[] password, byte[] salt)
        {
            using (var keyDerivationFunction = new Rfc2898DeriveBytes(password, salt,
                       ConstForHashing.ITERATIONS, ConstForHashing.HASH_ALGORITHM_NAME))
            {
                byte[] key = keyDerivationFunction.GetBytes(ConstForHashing.PASSWORD_KEY_LENGTH);
                return key;
            }
        }
    }
}