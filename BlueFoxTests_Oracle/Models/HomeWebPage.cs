using System.ComponentModel.DataAnnotations;

namespace BlueFoxTests_Oracle.Models
{
    public partial class HomeWebPage
    {
        [Key]
        public int WebPage_Id { get; set; }

        [StringLength(70)]
        public string WebPage_URL { get; set; }
    }
}
