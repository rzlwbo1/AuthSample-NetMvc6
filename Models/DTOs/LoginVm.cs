using System.ComponentModel.DataAnnotations;

namespace AuthSampleRoleBased.Models.DTOs
{
    public class LoginVm
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
