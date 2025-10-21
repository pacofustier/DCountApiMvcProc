using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCountApiMvcProc.Models
{
    public class DCount
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, 24)]
        [Column(TypeName = "decimal(4,2)")]
        public decimal Hours { get; set; }

        [Required]
        public int WorkerId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Worker? Worker { get; set; }
        public Project? Project { get; set; }
    }
}
