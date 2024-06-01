using Dexma_cpt_ClientSide.Encryption;
using Dexma_cpt_ClientSide.Storages;
using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary.Encrypt;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;

namespace Dexma_cpt_ClientSide.Services
{
    public class ChatService : IChatService
    {
        public event Action<MessageModel, string>? NewTextMessage;

        public event Action<MessageModel, string> EditTextMessage;

        public event Action<int, string> DeleteTextMessage;

        public event Action<ChatModel> NewChat;

        public event Action<int, string> EditOnSenderSide;


        private HubConnection? connection;
        private readonly string url = @"http://localhost:5290/chatHub";
        private readonly StorageManager _storage;
        private readonly RSAEncryption _rSAEncryption;
        private readonly ClientKeyHelper _clientKeyHelper;

        public ChatService(StorageManager storage, RSAEncryption rSAEncryption, ClientKeyHelper clientKeyHelper)
        {
            _storage = storage;
            _rSAEncryption = rSAEncryption;
            _clientKeyHelper = clientKeyHelper;
        }

        public async Task ConnectAsync()
        {
            var hubConnectionBuilder = new HubConnectionBuilder();
            connection = hubConnectionBuilder.WithUrl(url).Build();

            connection.ServerTimeout = TimeSpan.FromSeconds(60);

            connection.On<MessageModel, string>("UnicastTextMessage", (m, s) => NewTextMessage?.Invoke(m, s));
            connection.On<int, string>("DeleteTextMessage", (m, s) => DeleteTextMessage?.Invoke(m, s));
            connection.On<MessageModel, string>("EditTextMessage", (m, s) => EditTextMessage?.Invoke(m, s));
            connection.On<ChatModel>("NewChat", (c) => NewChat?.Invoke(c));

            ServicePointManager.DefaultConnectionLimit = 10;
            await connection.StartAsync();
        }

        public async Task<AuthorizationOrRegistrationResult?> RegisterOrLoginAsync(string methodName, object model)
        {
            if (connection != null)
            {
                var result = await connection.InvokeAsync<AuthorizationOrRegistrationResult>(methodName, model);

                if (result != null)
                {
                    var message = result.StringResult;
                    var resultmodel = result.ProfileModel;

                    if (resultmodel != null && result.StringResult == null)
                    {
                        _storage.WriteToFile(resultmodel.Username, resultmodel.Token);

                        return new AuthorizationOrRegistrationResult
                        {
                            ProfileModel = resultmodel,
                            StringResult = null,
                            publicKey = result.publicKey,
                            P = result.P,
                            Q = result.Q,
                        };
                    }
                    else
                    {

                        return new AuthorizationOrRegistrationResult
                        {
                            ProfileModel = null,
                            StringResult = result.StringResult,
                            publicKey = null,
                            P = null,
                            Q = null,
                        };

                    }
                }
            }

            return null;
        }

        public async Task<AuthorizationOrRegistrationResult?> RegisterAsync(RegisterModel registerModel)
        {
            return await RegisterOrLoginAsync("Register", registerModel);
        }

        public async Task<AuthorizationOrRegistrationResult?> LoginAsync(LoginModel loginModel)
        {
            var privateKey = _rSAEncryption.GetPrivateKey();
            var publicKey = _rSAEncryption.GetPublicKey();
            var p = _rSAEncryption.GetP();
            var q = _rSAEncryption.GetQ();

            RSAKeyData publicRsa = new()
            {
                publicKey = publicKey,
                privateKey = null,
                P = p,
                Q = q,
            };

            RSAKeyData privateRsa = new()
            {
                publicKey = null,
                privateKey = privateKey,
                P = p,
                Q = q,
            };

            await ClientKeyHelper.SaveClientPublicKey(loginModel.Username, publicRsa);
            await ClientKeyHelper.SaveClientPrivateKey(loginModel.Username, privateRsa);

            loginModel.publicKey = publicRsa.publicKey.ToString();
            loginModel.Q = publicRsa.Q.ToString();
            loginModel.P = publicRsa.P.ToString();

            var result = await RegisterOrLoginAsync("Login", loginModel);

            if (result.StringResult == null)
            {

                RSAKeyData rSAKey = new()
                {
                    publicKey = BigInteger.Parse(result.publicKey),
                    P = BigInteger.Parse(result.P),
                    Q = BigInteger.Parse(result.Q),
                };

                await ClientKeyHelper.SaveHubPublicKey(rSAKey);

                return result;
            }
            else return result;
        }

        public async Task<ChatsResult?> GetChatsAsync()
        {
            if (connection != null)
            {
                string token = _storage.ReadFromFile(App.User.Username);

                InitialModel initial = new()
                {
                    Token = token,
                };

                var result = await connection.InvokeAsync<ChatsResult>("GetChats", initial);

                return result;
            }
            else return null;
        }

        public async Task<ChatsResult?> SearchUserAsync(SearchModel searchModel)
        {
            if (connection != null)
            {
                string token = _storage.ReadFromFile(App.User.Username);
                searchModel.token = token;
                var result = await connection.InvokeAsync<ChatsResult>("GetUsers", searchModel);

                return result;
            }
            else return null;
        }

        public async Task<MessageResult> GetChatHistory(SearchModel searchModel)
        {
            if (connection != null)
            {
                string token = _storage.ReadFromFile(App.User.Username);
                searchModel.token = token;

                var result = await connection.InvokeAsync<MessageResult>("GetHistory", searchModel);

                return result;
            }
            else return null;
        }

        public async Task<int> SendUnicastMessageAsync(SendMessageModel messageModel)
        {
            if (connection != null)
            {
                string token = _storage.ReadFromFile(App.User.Username);
                messageModel.Token = token;
                return await connection.InvokeAsync<int>("UnicastTextMessage", messageModel);
            }
            return 0;
        }

        public async Task DeleteMessageInChatAsync(SendMessageModel messageModel)
        {
            if (connection != null)
            {
                string token = _storage.ReadFromFile(App.User.Username);
                messageModel.Token = token;

                await connection.InvokeAsync<OperationOnMessageResult>("DeleteTextMessage", messageModel);
            }
        }

        public void OnMessageEdited(int? messageId, string MessageData)
        {
            EditOnSenderSide?.Invoke((int)messageId, MessageData);
        }

        public async Task<int> EditMessageInChatAsync(SendMessageModel messageModel)
        {
            if (connection != null)
            {
                string token = _storage.ReadFromFile(App.User.Username);
                messageModel.Token = token;
                var result = await connection.InvokeAsync<int>("EditTextMessage", messageModel);
                return result;
            }
            return 0;
        }

        public async Task<AuthorizationOrRegistrationResult> ChangeProfile(RegisterModel registerModel)
        {
            if (connection != null)
            {
                string token = _storage.ReadFromFile(App.User.Username);
                registerModel.Token = token;
                var result = await connection.InvokeAsync<AuthorizationOrRegistrationResult>("ChangeProfile", registerModel);
                return result;
            }
            return null;
        }

        public async Task<string> RemoveProfile(string token)
        {
            if (connection != null)
            {
                string Usertoken = _storage.ReadFromFile(App.User.Username);
                token = Usertoken;
                return await connection.InvokeAsync<string>("RemoveProfile", token);
            }
            return null;
        }
    }
}
