using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public class Questions_For_Tests
    {
        [Key]
        public int Question_Id { get; set; }

        public int? Test_Id { get; set; }

        public int? Question_Number { get; set; }

        [StringLength(200)]
        public string Question { get; set; }

        public List<Answers_For_Tests> Answers_For_Tests { get; set; }

        public virtual Test Test { get; set; }
    }
}
