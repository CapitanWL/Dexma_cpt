using Dexma_cpt_EncryptLibrary.Encrypt;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Dexma_cpt_ServerSide.Encryption
{
    public class KeysHelper
    {
        public async Task SaveClientPublicKey(string username, RSAKeyData publicKeyData)
        {
            string publicKeyFileName = AppContext.BaseDirectory + @"../../../Encryption/ClientsPublicKeys/" + username + "_publicKey.json";

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string publicKeyJson = JsonSerializer.Serialize(publicKeyData, options);

            await File.WriteAllTextAsync(publicKeyFileName, publicKeyJson);
        }

        public async Task<RSAKeyData?> GetClientPublicKey(string username)
        {
            string publicKeyFileName = AppContext.BaseDirectory + @"../../../Encryption/ClientsPublicKeys/" + username + "_publicKey.json";

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string KeyJson = await File.ReadAllTextAsync(publicKeyFileName);

            return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
        }

        public async Task SaveHubPublicKey(RSAKeyData publicKeyData)
        {
            string serverKeyFileName = AppContext.BaseDirectory + @"../../../Encryption/HubKeys/hub_publicKey.json";

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string KeyJson = JsonSerializer.Serialize(publicKeyData, options);

            await File.WriteAllTextAsync(serverKeyFileName, KeyJson);
        }

        public async Task SaveHubPrivateKey(RSAKeyData publicKeyData)
        {
            string serverKeyFileName = AppContext.BaseDirectory + @"../../../Encryption/HubKeys/hub_privateKey.json";

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            options.Converters.Add(new BigIntegerConverter());

            string KeyJson = JsonSerializer.Serialize(publicKeyData, options);

            await File.WriteAllTextAsync(serverKeyFileName, KeyJson);
        }

        public async Task<RSAKeyData> GetHubPublicKey()
        {
            string serverKeyFileName = AppContext.BaseDirectory + @"../../../Encryption/HubKeys/hub_publicKey.json";

            if (File.Exists(serverKeyFileName))
            {
                string KeyJson = await File.ReadAllTextAsync(serverKeyFileName);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                options.Converters.Add(new BigIntegerConverter());

                return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
            }

            return null;
        }

        public async Task<RSAKeyData> GetHubPrivateKey()
        {
            string serverKeyFileName = AppContext.BaseDirectory + @"../../../Encryption/HubKeys/hub_privateKey.json";

            if (File.Exists(serverKeyFileName))
            {
                string KeyJson = await File.ReadAllTextAsync(serverKeyFileName);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                options.Converters.Add(new BigIntegerConverter());

                return JsonSerializer.Deserialize<RSAKeyData>(KeyJson, options);
            }

            return null;
        }

    }
}
