using System.Security.Cryptography;

namespace Dexma_cpt_EncryptLibrary
{
    public static class ConstForHashing
    {
        public const int PASSWORD_KEY_LENGTH = 128;

        public static readonly HashAlgorithmName HASH_ALGORITHM_NAME = HashAlgorithmName.SHA512;

        public const int ITERATIONS = 10000;
    }
}
