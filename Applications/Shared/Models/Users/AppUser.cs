using Microsoft.AspNetCore.Identity;

namespace Application.Models
{
    public class AppUser : IdentityUser
    {
        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
    }
}