using Microsoft.AspNetCore.Identity;

namespace Asp_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
