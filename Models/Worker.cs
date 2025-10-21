using System.ComponentModel.DataAnnotations;

namespace DCountApiMvcProc.Models
{
    public class Worker
    {
        public int Id { get; set; }

        [Required, StringLength(128)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress, StringLength(128)]
        public string? Email { get; set; }

        public ICollection<DCount> DCounts { get; set; } = [];
    }
}
