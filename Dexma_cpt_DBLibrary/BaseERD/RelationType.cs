using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
