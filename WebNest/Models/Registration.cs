using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebNest.Models
{
    [Table("Registration")]
    public class Registration
    {

        [Key]
        public int RegId { get; set; }
        public string? ClientName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyType { get; set; }
        public int Employees { get; set; }


        public string? UserId { get; set; }
        public string? Password { get; set; }

        [NotMapped]
        public string? ConfirmPassword { get; set; }

        public int LoggedInUserId { get; set; }



        public string? ProfilePicture { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }



    }
}
