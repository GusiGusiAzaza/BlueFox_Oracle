using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public class User
    {
        public int UserId { get; set; }
        //public string Usernamee { get; set; }
        public string PasswordHash { get; set; }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Admins = new HashSet<Admin>();
            Test_Progress = new HashSet<Test_Progress>();
        }

        [Key]
        public int User_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Username { get; set; }

        [Required]
        [StringLength(15)]
        public string Password_Hash { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Admin> Admins { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Test_Progress> Test_Progress { get; set; }

        public virtual User_Info User_Info { get; set; }

        public virtual User_Stats User_Stats { get; set; }
    }
}
