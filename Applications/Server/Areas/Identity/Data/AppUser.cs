using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace Application.Areas.Identity.Data;


public class AppUser : IdentityUser
{
    public virtual Client Client { get; set; }  
    public virtual Company Company { get; set; }
    public virtual Employee Employee { get; set; }
}

