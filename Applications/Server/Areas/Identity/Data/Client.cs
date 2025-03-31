using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Areas.Identity.Data
{
    public class Client
    {
        [Key]
        public string UserId { get; set; }


        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }

        public virtual AppUser User { get; set; }
    }
}
