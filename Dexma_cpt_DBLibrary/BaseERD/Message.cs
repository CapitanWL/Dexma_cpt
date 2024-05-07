using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dexma_cpt_DBLibrary
{
    public class Message
    {
        public required int MessageId { get; set; }
        public int? UserRelationId { get; set; }
        public required byte[] MessageData { get; set; }
        public required DateTime SendingDateTime { get; set; }
        public required bool IsDeleted { get; set; }
        public UserRelation? UserRelation { get; set; }
        public Message() { }
    }
}
