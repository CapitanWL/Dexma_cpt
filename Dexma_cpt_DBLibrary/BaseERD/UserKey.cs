using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dexma_cpt_DBLibrary;

namespace Dexma_cpt_DBLibrary
{
    public class UserKey
    {
        public required int UserKeyId { get; set; }
        public required byte[] Password { get; set; }
        public required byte[] PasswordSalt { get; set; }
        public required int UserId { get; set; }
        public required User User { get; set; }
        public required InternalKey InternalKey { get; set; }
        public UserKey() { }
    }
}
