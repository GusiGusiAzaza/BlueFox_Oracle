using System;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public partial class Test_Result
    {
        [Key]
        public int Progress_Id { get; set; }

        public int? User_Id { get; set; }

        public int? Test_Id { get; set; }

        public DateTime? Test_Date { get; set; }

        public bool? Is_Passed { get; set; }

        public int? Right_Answers_Count { get; set; }

        public int? Questions_Count { get; set; }

        public virtual Test Test { get; set; }

        public virtual User User { get; set; }
    }
}
