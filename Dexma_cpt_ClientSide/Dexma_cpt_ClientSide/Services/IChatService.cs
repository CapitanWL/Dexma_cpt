using Dexma_cpt_CommonModels;
using System;
using System.Threading.Tasks;

namespace Dexma_cpt_ClientSide.Services
{
    public interface IChatService
    {
        event Action<MessageModel, string> NewTextMessage;

        event Action<MessageModel, string> EditTextMessage;

        event Action<int, string> DeleteTextMessage;

        event Action<ChatModel> NewChat;

        event Action<int, string> EditOnSenderSide;



        Task ConnectAsync();
        Task<AuthorizationOrRegistrationResult?> LoginAsync(LoginModel loginModel);
        Task<AuthorizationOrRegistrationResult?> RegisterAsync(RegisterModel registerModel);
        Task<ChatsResult?> GetChatsAsync();
        Task<ChatsResult?> SearchUserAsync(SearchModel searchModel);
        Task<MessageResult> GetChatHistory(SearchModel searchModel);
        Task<int> SendUnicastMessageAsync(SendMessageModel messageModel);
        Task DeleteMessageInChatAsync(SendMessageModel messageModel);
        Task<int> EditMessageInChatAsync(SendMessageModel messageModel);
        Task<AuthorizationOrRegistrationResult> ChangeProfile(RegisterModel registerModel);
        Task<string> RemoveProfile(string token);
        void OnMessageEdited(int? messageId, string MessageData);
    }
}
