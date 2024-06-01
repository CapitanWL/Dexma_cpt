using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dexma_cpt_DBLibrary
{
    public class User
    {
        [Key]
        
        public int UserId { get; set; }

        [StringLength(35)]
        public required string Username { get; set; }

        [StringLength(50)]
        public required string Nickname { get; set; }

        [StringLength(11)]
        public required string Phone { get; set; }
        public required bool AccountStatus { get; set; }
        public UserKey UserKey { get; set; }
        public User() { }
    }
}
