using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class Client
    {
        [Key] public string UserId { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string Phone { get; set; }

        public virtual AppUser User { get; set; }
    }
}