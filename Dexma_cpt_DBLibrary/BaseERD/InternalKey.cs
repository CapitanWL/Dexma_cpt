using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dexma_cpt_DBLibrary
{
    public class InternalKey
    {
        public required int InternalKeyId { get; set; }
        public required byte[] InternalKeyData { get; set; }
        public required int UserKeyId { get; set; }
        public required UserKey UserKey { get; set; }
        public ICollection<UserRelation> UserRelationsTo { get; } = new List<UserRelation>();
        public ICollection<UserRelation> UserRelationsFrom { get; } = new List<UserRelation>();
        public InternalKey() { }
    }
}
