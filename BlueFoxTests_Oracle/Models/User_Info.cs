using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueFoxTests_Oracle.Models
{
    public partial class User_Info
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int User_Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(15)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(100)]
        public string Summary { get; set; }

        [StringLength(60)]
        public string Education { get; set; }

        [StringLength(60)]
        public string Work { get; set; }

        public virtual User User { get; set; }
    }
}
