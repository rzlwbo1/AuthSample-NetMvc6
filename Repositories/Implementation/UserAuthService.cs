using AuthSampleRoleBased.Models.Domain;
using AuthSampleRoleBased.Models.DTOs;
using AuthSampleRoleBased.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuthSampleRoleBased.Repositories.Implementation
{
    public class UserAuthService : IUserAuthService
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthService(SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }


        public async Task<Status> LoginAsync(LoginVm loginVm)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(loginVm.Username);

            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "nvalid username/password";
                return status;
            }

            // match the password
            // is not match
            if(!await userManager.CheckPasswordAsync(user, loginVm.Password))
            {
                status.StatusCode = 0;
                status.Message = "Invalid username/password";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, loginVm.Password, false, true);
            if (signInResult.Succeeded)
            {
                var userRole = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                foreach (var role in userRole)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }

                status.StatusCode = 1;
                status.Message = "Login Succesfully";
                return status;
            } 
            else if(signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User Locked out";
                return status;

            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error on logged in";
                return status;
            }
        }


        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }


        public async Task<Status> RegistrationAsync(RegistrationVm registrationVm)
        {
            var status = new Status();
            var userExist = await userManager.FindByNameAsync(registrationVm.Username);

            if (userExist != null)
            {
                status.StatusCode = 0;
                status.Message = "User already Exist";
                return status;
            }

            // create a new user
            ApplicationUser user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = registrationVm.Name,
                Email = registrationVm.Email,
                UserName = registrationVm.Username,
                EmailConfirmed = true
            };

            // assign to userManager
            var result = await userManager.CreateAsync(user, registrationVm.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User Creation Failed";
                return status;
            }

            // role management
            // jika gak ada role, maka buatin
            if (!await roleManager.RoleExistsAsync(registrationVm.Role))
            {
                await roleManager.CreateAsync(new IdentityRole(registrationVm.Role));
            }

            // nambah role
            if (await roleManager.RoleExistsAsync(registrationVm.Role))
            {
                await userManager.AddToRoleAsync(user, registrationVm.Role);
            }

            status.StatusCode = 1;
            status.Message = "User has Registerd Succesfully";
            return status;
        }
    }
}
