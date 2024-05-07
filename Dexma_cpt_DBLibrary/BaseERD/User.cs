using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dexma_cpt_DBLibrary
{
    public class User
    {
        public required int UserId { get; set; }
        public required string Username { get; set; }
        public string? Nickname { get; set; }
        public byte[]? AccountImage { get; set; }
        public required string Phone { get; set; }
        public required bool AccountStatus { get; set; }
        public required UserKey UserKey { get; set; }
        public User() { }
    }
}
