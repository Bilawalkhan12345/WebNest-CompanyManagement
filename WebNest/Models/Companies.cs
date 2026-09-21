using System.ComponentModel.DataAnnotations;

namespace WebNest.Models
{
    public class Companies
    {
        [Key]
        public int CompId { get; set; }
        public string Name { get; set; }
        public int Employees { get; set; }
        public string Type { get; set; }

    }
}
