using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBookReader.Models
{
    public class User:IdentityUser
    {
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
