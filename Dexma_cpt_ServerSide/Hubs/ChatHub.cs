using Dexma_cpt_CommonModels;
using Dexma_cpt_DBLibrary;
using Dexma_cpt_EncryptLibrary;
using Dexma_cpt_EncryptLibrary.Encrypt;
using Dexma_cpt_ServerSide.Encryption;
using Dexma_cpt_ServerSide.Services;
using Dexma_cpt_ServerSide.Services.Auth;
using Dexma_cpt_ServerSide.Services.Auth.AuthHelp;
using Dexma_cpt_ServerSide.Services.Chatiing.Chats;
using Dexma_cpt_ServerSide.Services.Chatiing.Chats.ChatsHelp;
using Dexma_cpt_ServerSide.Services.Chatiing.Messages;
using Dexma_cpt_ServerSide.Services.Chatiing.Messages.MessagesHelp;
using Dexma_cpt_ServerSide.Services.Profile;
using Dexma_cpt_ServerSide.Services.Profile.ProfileHelp;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Dexma_cpt_ServerSide.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        private static ConcurrentDictionary<string, ChatClient> ChatClients = new();

        #region context

        private readonly DexmaDbContext _dbContext;

        #endregion

        #region auth

        private readonly AuthHelpModel _authHelp;
        private readonly JwtManager _jwtManager;
        private readonly AuthService _authService;

        #endregion

        #region chats

        private readonly ChatsHelpModel _chatsHelp;
        private readonly ChatService _chatService;

        #endregion

        #region idk

        private readonly UserManager _userManager;

        #endregion

        #region messages

        private readonly MessageService _messageService;
        private readonly MessagesHelpModel _messagesHelpModel;

        #endregion

        #region profile

        ProfileService _profileService;
        ProfileHelpModel _profileHelp;

        #endregion

        #region constructor

        public ChatHub(DexmaDbContext dbContext)
        {
            _dbContext = dbContext;
            _jwtManager = new();
            _userManager = new();
            

            RSAEncryption rSAEncryption = new();
            AESEncryption aESEncryption = new();

            _authHelp = new(_dbContext, _jwtManager, aESEncryption);
            _chatsHelp = new(_dbContext);
            _messagesHelpModel = new(_dbContext);
            _profileHelp = new(_dbContext);

            _chatService = new(_chatsHelp, _userManager, _messagesHelpModel);
            _authService = new(_dbContext, _authHelp);
            _messageService = new(_messagesHelpModel, _userManager, rSAEncryption, aESEncryption);

            _profileService = new(_dbContext, _profileHelp, _userManager);
        }

        #endregion


        #region login + reg
        public async Task<AuthorizationOrRegistrationResult> Login(LoginModel loginModel)
        {
            if (!ChatClients.ContainsKey(loginModel.Username))
            {
                ChatClient newClient = new() { Id = Context.ConnectionId, UserName = loginModel.Username };
                ChatClients.TryAdd(loginModel.Username, newClient);
            }

            var result = await _authService.Login(loginModel);
            return result;
        }

        public async Task<AuthorizationOrRegistrationResult> Register(RegisterModel registerModel)
        {
            if (!ChatClients.ContainsKey(registerModel.Username))
            {
                ChatClient newClient = new() { Id = Context.ConnectionId, UserName = registerModel.Username };
                ChatClients.TryAdd(registerModel.Username, newClient);
            }
            return await _authService.Register(registerModel);
        }

        #endregion


        #region get chats + get search result

        public async Task<ChatsResult> GetChats(InitialModel initial)
        {

            return _chatService.GetChats(initial);
        }

        public async Task<ChatsResult> GetUsers(SearchModel searchModel)
        {
            return await _chatService.GetUsers(searchModel);
        }

        #endregion

        #region message operations

        public async Task<MessageResult> GetHistory(SearchModel searchModel)
        {
            return await _messageService.GetHistory(searchModel);
        }

        public async Task<int> UnicastTextMessage(SendMessageModel messageModel)
        {

            var result = await _messageService.SendNewMessage(messageModel);


            if ((ChatClients.TryGetValue(messageModel.UsernameTo, out ChatClient client)))
            {
                KeysHelper keysHelper = new();

                var publicKey = await keysHelper.GetClientPublicKey(messageModel.UsernameTo);
                RSAEncryption rSAEncryption = new();

                var encryptMessage = _messagesHelpModel.DecryptMessage(result.Item3, result.Item4,
                            result.Item5);

                var BigIntegerList = rSAEncryption.Encrypt(encryptMessage, publicKey.publicKey, publicKey.P, publicKey.Q);

                List<string> stringList = new();

                foreach (var bi in BigIntegerList)
                {
                    stringList.Add(bi.ToString());
                }

                var baseMessageModel = new MessageModel
                {
                    MessageData = stringList,
                    SendingDateTime = result.Item3.SendingDateTime,
                    MessageFrom = await _messagesHelpModel.GetMessageFromUser(result.Item3, result.Item4.InternalKeyId, result.Item6, messageModel.UsernameTo),
                    MessageModelId = result.Item3.MessageId,
                    IsEdited = result.Item3.IsEdited == true ? "edited" : string.Empty

                };

                await Clients.Client(client.Id).UnicastTextMessage(baseMessageModel, result.Item1);

                if (result.Item2)
                {

                    await Clients.Client(client.Id).NewChat(result.Item7);

                }
            }

            return result.Item3.MessageId;
        }

        public async Task DeleteTextMessage(SendMessageModel messageModel)
        {
            var result = await _messageService.RemoveMessage(messageModel);


            if (ChatClients.TryGetValue(result.Item1, out ChatClient sender))
            {

                await Clients.Client(sender.Id).DeleteTextMessage(result.Item2, result.Item1);

                if (ChatClients.TryGetValue(messageModel.UsernameTo, out ChatClient client))
                {
                    await Clients.Client(client.Id).DeleteTextMessage(result.Item2, result.Item1);

                }
            }
        }

        public async Task<int> EditTextMessage(SendMessageModel messageModel)
        {
            var result = await _messageService.EditMessage(messageModel);

            KeysHelper keysHelper = new();


            RSAEncryption rSAEncryption = new();

            List<string> stringList = new();


            if (ChatClients.TryGetValue(messageModel.UsernameTo, out ChatClient client) && messageModel.UsernameTo != result.Item1)
            {
                var publicKey = await keysHelper.GetClientPublicKey(messageModel.UsernameTo);

                var encryptMessage = _messagesHelpModel.DecryptMessage(result.Item2, result.Item3,
                            result.Item4);

                var BigIntegerList = rSAEncryption.Encrypt(encryptMessage, publicKey.publicKey, publicKey.P, publicKey.Q);

                foreach (var bi in BigIntegerList)
                {
                    stringList.Add(bi.ToString());
                }

                var baseMessageModel = new MessageModel
                {
                    MessageData = stringList,
                    SendingDateTime = result.Item2.SendingDateTime,
                    MessageFrom = await _messagesHelpModel.GetMessageFromUser(result.Item2, result.Item3.InternalKeyId, result.Item5, messageModel.UsernameTo),
                    IsEdited = result.Item2.IsEdited == true ? "edited" : string.Empty,
                    MessageModelId = result.Item2.MessageId
                };


                await Clients.Client(client.Id).EditTextMessage(baseMessageModel, result.Item1);
            }

            return result.Item2.MessageId;
        }

        #endregion


        #region profile operations

        public async Task<AuthorizationOrRegistrationResult> ChangeProfile(RegisterModel registerModel)
        {
            return await _profileService.UpdateUserProfile(registerModel);
        }

        public async Task<string> RemoveProfile(string token)
        {
            return await _profileService.RemoveUserProfile(token);
        }

        #endregion

    }
}
