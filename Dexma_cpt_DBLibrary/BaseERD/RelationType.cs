using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dexma_cpt_DBLibrary
{
    public class RelationType
    {
        public required int RelationTypeId { get; set; }
        public required string RelationName { get; set; }
        public ICollection<UserRelation> UserRelations { get; } = new List<UserRelation>();
        public RelationType() { }


    }
}
