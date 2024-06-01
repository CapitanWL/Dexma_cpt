using System.ComponentModel.DataAnnotations;

namespace Dexma_cpt_DBLibrary
{
    public class UserRelation
    {
        [Key]

        public int UserRelationId { get; set; }
        public int? InternalToId { get; set; }
        public int? InternalFromId { get; set; }
        public int RelationTypeId { get; set; }
        public InternalKey? InternalTo { get; set; }
        public InternalKey? InternalFrom { get; set; }
        public RelationType RelationType { get; set; }
        public ICollection<Message> Messages { get; } = new List<Message>();
        public UserRelation() { }
    }
}
