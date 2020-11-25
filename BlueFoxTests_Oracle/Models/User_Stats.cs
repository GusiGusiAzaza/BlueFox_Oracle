using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueFoxTests_Oracle.Models
{
    public partial class User_Stats
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int User_Id { get; set; }

        public int? Right_Answered { get; set; }

        public int? Total_Answered { get; set; }

        public int? Finished_Tests_Count { get; set; }

        public int? Right_Tests_Count { get; set; }

        public virtual User User { get; set; }
    }
}
