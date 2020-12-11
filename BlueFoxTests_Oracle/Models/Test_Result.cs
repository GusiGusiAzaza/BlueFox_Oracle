using System;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public class Test_Result
    {
        [Key]
        public int Result_Id { get; set; }

        public int User_Id { get; set; }

        public int Test_Id { get; set; }

        public int Try_Count { get; set; }

        public float Score { get; set; }

        public bool Is_Passed { get; set; }

        public int? Right_Answers_Count { get; set; }

        public int? Questions_Count { get; set; }

        public DateTime Start_Date { get; set; }

        public DateTime? End_Date { get; set; }

        public Test Test { get; set; }
    }
}
