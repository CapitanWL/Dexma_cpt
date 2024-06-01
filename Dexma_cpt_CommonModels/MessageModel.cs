namespace Dexma_cpt_CommonModels
{
    public class MessageModel
    {
        public int? MessageModelId { get; set; }
        public List<string> MessageData { get; set; }
        public DateTime SendingDateTime { get; set; }
        public string MessageFrom { get; set; }
        public string? IsEdited { get; set; }
    }
}
