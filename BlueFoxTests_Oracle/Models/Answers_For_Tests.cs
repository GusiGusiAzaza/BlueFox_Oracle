using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public partial class Answers_For_Tests
    {
        [Key]
        public int Answer_Id { get; set; }

        [StringLength(200)]
        public string Answer { get; set; }

        public bool Is_Right { get; set; }

        public int? Question_Id { get; set; }

        public virtual Questions_For_Tests Questions_For_Tests { get; set; }
    }
}
