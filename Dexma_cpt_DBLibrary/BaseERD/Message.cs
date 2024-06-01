using System.ComponentModel.DataAnnotations;

namespace Dexma_cpt_DBLibrary
{
    public class Message
    {
        [Key]
        
        public int MessageId { get; set; }
        public int UserRelationId { get; set; }
        public byte[] MessageData { get; set; }
        public DateTime SendingDateTime { get; set; }
        public bool IsEdited { get; set; }
        public byte[] MessageIV {  get; set; }
        public UserRelation UserRelation { get; set; }
        public Message() { }
    }
}
