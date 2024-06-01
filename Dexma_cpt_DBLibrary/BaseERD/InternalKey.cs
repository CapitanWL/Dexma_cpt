using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dexma_cpt_DBLibrary
{
    public class InternalKey
    {
        [Key]
        public int InternalKeyId { get; set; }
        public byte[] InternalKeyData { get; set; }
        public int UserKeyId { get; set; }
        public UserKey UserKey { get; set; }
        public ICollection<UserRelation> UserRelationsTo { get; } = new List<UserRelation>();
        public ICollection<UserRelation> UserRelationsFrom { get; } = new List<UserRelation>();
        public InternalKey() { }
    }
}
