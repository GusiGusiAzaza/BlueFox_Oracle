using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public class User
    {
        public int User_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Username { get; set; }

        [Required]
        [StringLength(15)]
        public string Password_Hash { get; set; }

        public virtual User_Info User_Info { get; set; }

        public virtual User_Stats User_Stats { get; set; }
    }
}
