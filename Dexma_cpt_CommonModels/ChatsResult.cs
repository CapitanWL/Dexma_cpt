using System.Collections.ObjectModel;

namespace Dexma_cpt_CommonModels
{
    public class ChatsResult
    {
        public string? StringResult { get; set; }
        public ObservableCollection<ChatModel>? Chats { get; set; }
    }
}
