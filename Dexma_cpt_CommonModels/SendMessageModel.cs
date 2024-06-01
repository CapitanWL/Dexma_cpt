namespace Dexma_cpt_CommonModels
{
    public class SendMessageModel
    {
        public int? SendMessageModelId { get; set; }
        public List<string> Message { get; set; }
        public string Token { get; set; }
        public string UsernameTo { get; set; }
        public DateTime DateTime { get; set; }
        public List<string>? OldMessage { get; set; }
    }
}
