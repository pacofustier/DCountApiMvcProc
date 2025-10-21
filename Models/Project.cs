using System.ComponentModel.DataAnnotations;

namespace DCountApiMvcProc.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required, StringLength(128)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1024)]
        public string? Description { get; set; }

        public ICollection<DCount> DCounts { get; set; } = [];
    }
}
