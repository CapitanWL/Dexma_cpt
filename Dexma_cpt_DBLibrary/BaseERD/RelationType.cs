using System.ComponentModel.DataAnnotations;

namespace Dexma_cpt_DBLibrary
{
    public class RelationType
    {
        [Key]
        public int RelationTypeId { get; set; }

        [EnumDataType(typeof(RelationTypes))]
        public required string RelationName { get; set; }
        public ICollection<UserRelation> UserRelations { get; } = new List<UserRelation>();
        public RelationType() { }


    }
}
