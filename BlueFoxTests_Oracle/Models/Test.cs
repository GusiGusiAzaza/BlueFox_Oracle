using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public partial class Test
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Test()
        {
            Questions_For_Tests = new HashSet<Questions_For_Tests>();
            Test_Progress = new HashSet<Test_Progress>();
        }

        [Key]
        public int Test_Id { get; set; }

        public int? Admin_Id { get; set; }

        [StringLength(50)]
        public string Test_Name { get; set; }

        public int? Theme_Id { get; set; }

        public virtual Admin Admin { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Questions_For_Tests> Questions_For_Tests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Test_Progress> Test_Progress { get; set; }

        public virtual Themes_For_Tests Themes_For_Tests { get; set; }
    }
}
