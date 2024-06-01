using Dexma_cpt_CommonModels;
using Dexma_cpt_EncryptLibrary.Encrypt;
using Dexma_cpt_ServerSide.Encryption;
using Dexma_cpt_ServerSide.Services.Chatiing.Chats.ChatsHelp;
using Dexma_cpt_ServerSide.Services.Chatiing.Messages.MessagesHelp;
using System.Collections.Concurrent;
using System.Numerics;

namespace Dexma_cpt_ServerSide.Services.Chatiing.Chats
{
    public class ChatService
    {
        private readonly ChatsHelpModel _chatsHelp;
        private readonly UserManager _userManager;
        private readonly MessagesHelpModel _messagesHelpModel;


        public ChatService(ChatsHelpModel chatsHelp, UserManager userManager, MessagesHelpModel messagesHelpModel)
        {
            _chatsHelp = chatsHelp;
            _userManager = userManager;
            _messagesHelpModel = messagesHelpModel;
        }

        public ChatsResult GetChats(InitialModel initial)
        {
            if (_userManager.ValidateToken(initial.Token))
            {
                int uid = _userManager.GetUserId(initial.Token);
                var collection = _chatsHelp.GetChatsAsync(uid);
                
                return new ChatsResult()
                {
                    Chats = collection == null ? null : collection,
                    StringResult = collection == null ? string.Empty : null
                };
            };

            return new ChatsResult()
            {
                Chats = null,
                StringResult = "Error"
            };
        }

        public async Task<ChatsResult> GetUsers(SearchModel searchModel)
        {
            if (_userManager.ValidateToken(searchModel.token))
            {
                int uid = _userManager.GetUserId(searchModel.token);

                var collection = await _chatsHelp.GetUsersAsync(searchModel.searchUsername, uid);

                return new ChatsResult()
                {
                    Chats = collection,
                    StringResult = collection == null ? "Not found!" : string.Empty
                };
            }
            return new ChatsResult()
            {
                Chats = null,
                StringResult = "Error"
            };
        }
    }
}
