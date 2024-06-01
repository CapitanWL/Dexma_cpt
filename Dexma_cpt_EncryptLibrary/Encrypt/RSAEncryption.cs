using System.Numerics;
using System.Text;

namespace Dexma_cpt_EncryptLibrary.Encrypt
{
    public class RSAEncryption
    {
        private BigInteger p; // для генерации открытого и закрытого ключей
        private BigInteger q; // для генерации открытого и закрытого ключей
        private BigInteger n; // для вычисления открытого и закрытого ключей
        private BigInteger phi; // для вычисления закрытого ключа
        private BigInteger e; // открытая экспонента
        private BigInteger d; // для расшифровки сообщений
        private Random random = new Random();

        public RSAEncryption()
        {
            p = GenerateRandomPrime(64, random);
            q = GenerateRandomPrime(64, random);

            e = GenerateRandomPrime(32, random);

            n = p * q;

            phi = (p - 1) * (q - 1);

            d = CalculatePrivateKey(e, phi);

        }

        public BigInteger GetP()
        {
            return p;
        }

        public BigInteger GetQ() { return q; }

        public BigInteger GetPublicKey()
        {
            return e;
        }

        public BigInteger GetPrivateKey()
        {
            return d;
        }

        public static BigInteger CalculatePrivateKey(BigInteger e, BigInteger phi)
        {

            BigInteger d = ModInverse(e, phi);
            return d;
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;

                m = a % m;
                a = t;
                t = y;

                y = x - q * y;
                x = t;
            }

            if (x < 0)
                x += m0;

            return x;
        }

        public List<BigInteger> Encrypt(string message, BigInteger? e, BigInteger p, BigInteger q)
        {
            n = p * q;
            int bitLength = (int)Math.Ceiling(BigInteger.Log(n, 2));
            int blockSize = (bitLength - 1) / 8;


            if (e == null) return null;

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            List<BigInteger> encryptedBlocks = new List<BigInteger>();

            for (int i = 0; i < bytes.Length; i += blockSize)
            {
                byte[] blockBytes = bytes.Skip(i).Take(blockSize).ToArray();
                BigInteger numericBlock = new BigInteger(blockBytes);
                BigInteger encryptedBlock = BigInteger.ModPow(numericBlock, (BigInteger)e, n);
                encryptedBlocks.Add(encryptedBlock);
            }

            return encryptedBlocks;
        }

        public string Decrypt(List<BigInteger> encryptedBlocks, BigInteger? d, BigInteger p, BigInteger q)
        {
            n = p * q;
            int bitLength = (int)Math.Ceiling(BigInteger.Log(n, 2));
            int blockSize = (bitLength - 1) / 8;

            if (d == null) return null;

            List<byte> decryptedBytes = new List<byte>();

            foreach (BigInteger encryptedBlock in encryptedBlocks)
            {
                BigInteger decryptedBlock = BigInteger.ModPow(encryptedBlock, (BigInteger)d, n);
                byte[] decryptedBlockBytes = decryptedBlock.ToByteArray();
                decryptedBytes.AddRange(decryptedBlockBytes);
            }

            return Encoding.UTF8.GetString(decryptedBytes.ToArray());
        }


        private static BigInteger GenerateRandomPrime(int bitLength, Random random)
        {
            BigInteger primeCandidate;
            do
            {
                byte[] bytes = new byte[bitLength / 8];
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] |= 0x01;
                bytes[0] |= 0x80;

                primeCandidate = new BigInteger(bytes);
            }
            while (!IsPrime(primeCandidate, 10));

            return primeCandidate;
        }

        private static bool IsPrime(BigInteger n, int iterations)
        {
            if (n <= 1) return false;
            if (n == 2 || n == 3) return true;
            if (n.IsEven) return false;

            BigInteger d = n - 1;
            int s = 0;

            while (d.IsEven)
            {
                d >>= 1;
                s++;
            }

            for (int i = 0; i < iterations; i++)
            {
                BigInteger a = RandomBigInteger(2, n - 2);
                BigInteger x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1) continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);

                    if (x == 1) return false;
                    if (x == n - 1) break;
                }

                if (x != n - 1) return false;
            }

            return true;
        }

        private static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
        {
            byte[] maxBytes = max.ToByteArray();
            byte[] value = new byte[maxBytes.Length];

            Random random = new Random();
            random.NextBytes(value);

            BigInteger result = new BigInteger(value);

            if (result < min)
            {
                result += min;
            }

            return result % max;
        }
    }
}
