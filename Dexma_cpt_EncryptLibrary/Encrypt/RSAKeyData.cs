using System.Numerics;

namespace Dexma_cpt_EncryptLibrary.Encrypt
{
    public class RSAKeyData
    {
        public BigInteger? publicKey { get; set; }
        public BigInteger? privateKey { get; set; }
        public BigInteger P { get; set; }
        public BigInteger Q { get; set; }
    }

}
