namespace Dexma_cpt_CommonModels
{
    public class ChatModel
    {
        public string FirstUsernameChar { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string? Phone { get; set; }
        public string StatusFrom { get; set;}
        public string StatusTo { get; set;}
        public bool AccountStatus { get; set;}
        public ChatModel() { }
    }
}
