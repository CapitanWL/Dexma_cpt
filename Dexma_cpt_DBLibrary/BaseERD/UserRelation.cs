using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dexma_cpt_DBLibrary
{
    public class UserRelation
    { 
        public required int UserRelationId { get; set; }
        public int? InternalToId { get; set; }
        public int? InternalFromId { get; set; }
        public required int RelationTypeId { get; set; }
        public InternalKey? InternalTo { get; set; }
        public InternalKey? InternalFrom { get; set; }
        public required RelationType RelationType { get; set; }
        public ICollection<Message> Messages { get; } = new List<Message>();
        public UserRelation() { }
    }
}
