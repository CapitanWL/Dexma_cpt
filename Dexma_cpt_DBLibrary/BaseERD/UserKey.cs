using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dexma_cpt_DBLibrary;

namespace Dexma_cpt_DBLibrary
{
    public class UserKey
    {
        [Key]
        public int UserKeyId { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public InternalKey InternalKey { get; set; }
        public UserKey() { }

        public UserKey(byte[] password, byte[] salt, int userId)
        {
            Password = password;
            PasswordSalt = salt;
            UserId = userId;
        }
    }
}
