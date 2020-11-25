using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public partial class Questions_For_Tests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Questions_For_Tests()
        {
            Answers_For_Tests = new HashSet<Answers_For_Tests>();
        }

        [Key]
        public int Question_Id { get; set; }

        public int? Test_Id { get; set; }

        public int? Question_Number { get; set; }

        [StringLength(200)]
        public string Question { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answers_For_Tests> Answers_For_Tests { get; set; }

        public virtual Test Test { get; set; }
    }
}
