using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public class Test
    {

        [Key]
        public int Test_Id { get; set; }

        public int? Admin_Id { get; set; }

        [StringLength(50)]
        public string Test_Name { get; set; }

        public int Theme_Id { get; set; }

        public int Time_Limit_In_Minutes { get; set; }
        
        public int Passing_Score { get; set; }

        public bool Is_Enabled { get; set; }

        public List<Questions_For_Tests> Questions_For_Tests { get; set; }
    }
}
