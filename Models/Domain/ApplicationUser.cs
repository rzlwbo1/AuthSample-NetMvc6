using Microsoft.AspNetCore.Identity;

namespace AuthSampleRoleBased.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? ProfilePict { get; set; }
    }
}
