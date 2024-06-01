using Dexma_cpt_EncryptLibrary.Encrypt;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dexma_cpt_ClientSide.Encryption
{
    public class ClientKeyHelper
    {

        #region hub
        public static async Task SaveHubPublicKey(RSAKeyData publicKeyData)
        {
            string publicKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/Hub/hub_publicKey.json";

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string publicKeyJson = JsonSerializer.Serialize(publicKeyData, options);

            await File.WriteAllTextAsync(publicKeyFileName, publicKeyJson);
        }

        public static async Task<RSAKeyData> GetHubPublicKey()
        {
            string serverKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/Hub/hub_publicKey.json";

            if (File.Exists(serverKeyFileName))
            {
                string KeyJson = await File.ReadAllTextAsync(serverKeyFileName);

                JsonSerializerOptions options = new()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                options.Converters.Add(new BigIntegerConverter());

                return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
            }

            return null;
        }

        public static RSAKeyData GetHubPublicKeyWithoutAsync()
        {
            string serverKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/Hub/hub_publicKey.json";

            if (File.Exists(serverKeyFileName))
            {
                string KeyJson = File.ReadAllText(serverKeyFileName);

                JsonSerializerOptions options = new()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                options.Converters.Add(new BigIntegerConverter());

                return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
            }

            return null;
        }

        #endregion

        #region client public key

        public static async Task SaveClientPublicKey(string username, RSAKeyData publicKeyData)
        {
            string publicKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/ClientsKeys/" + username + "_publicKey.json";

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string publicKeyJson = JsonSerializer.Serialize(publicKeyData, options);

            await File.WriteAllTextAsync(publicKeyFileName, publicKeyJson);
        }

        public static async Task<RSAKeyData?> GetClientPublicKey(string username)
        {
            string publicKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/ClientsKeys/" + username + "_publicKey.json";

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string KeyJson = await File.ReadAllTextAsync(publicKeyFileName);

            return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
        }

        #endregion

        #region client private key

        public static async Task SaveClientPrivateKey(string username, RSAKeyData publicKeyData)
        {
            string publicKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/ClientsKeys/" + username + "_privateKey.json";

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string publicKeyJson = JsonSerializer.Serialize(publicKeyData, options);

            await File.WriteAllTextAsync(publicKeyFileName, publicKeyJson);
        }

        public async Task<RSAKeyData?> GetClientPrivateKey(string username)
        {
            string publicKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/ClientsKeys/" + username + "_privateKey.json";

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string KeyJson = await File.ReadAllTextAsync(publicKeyFileName);

            return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
        }

        public RSAKeyData GetClientPrivateKeyWihoutAsync(string username)
        {
            string publicKeyFileName = @"../../../../Dexma_cpt_ClientSide/Encryption/ClientsKeys/" + username + "_privateKey.json";

            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string KeyJson = File.ReadAllText(publicKeyFileName);

            return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
        }

        #endregion
    }
}
