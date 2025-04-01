using Microsoft.AspNetCore.Identity;

namespace Application.Areas.Identity.Data
{
    public class AppUser : IdentityUser
    {
        public virtual Client Client { get; set; }
        public virtual Company Company { get; set; }
        public virtual Employee Employee { get; set; }
    }
}