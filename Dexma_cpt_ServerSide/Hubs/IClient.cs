using Dexma_cpt_CommonModels;

namespace Dexma_cpt_ServerSide.Hubs
{
    public interface IChatClient
    {
        Task UnicastTextMessage(MessageModel messageModel, string sender);

        Task DeleteTextMessage(int id, string sender);

        Task<int> EditTextMessage(MessageModel messageModel, string sender);

        Task NewChat(ChatModel chatModel);
    }
}
