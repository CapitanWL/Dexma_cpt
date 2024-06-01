using System.Collections.ObjectModel;

namespace Dexma_cpt_CommonModels
{
    public class MessageResult
    {
        public string? StringResult { get; set; }
        public ObservableCollection<MessageModel>? History { get; set; }
    }
}

