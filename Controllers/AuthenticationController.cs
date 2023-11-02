using AuthSampleRoleBased.Models.DTOs;
using AuthSampleRoleBased.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSampleRoleBased.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserAuthService _userAuthService;

        public AuthenticationController(IUserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }


        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationVm regisVm)
        {
            if (!ModelState.IsValid)
            {
                return View(regisVm);
            }

            // default user
            regisVm.Role = "user";
            var result = await _userAuthService.RegistrationAsync(regisVm);

            TempData["msg"] = result.Message;
            return RedirectToAction("Registration");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm loginVm)
        {
            // is not valid
            if (!ModelState.IsValid)
            {
                return View(loginVm);
            }

            var result = await _userAuthService.LoginAsync(loginVm);
            if(result.StatusCode == 1)
            {
                return RedirectToAction("Index", "Dashboard");
            } else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction("Login");
            }
        }


        [Authorize]
        public async Task Logout()
        {
            await _userAuthService.LogoutAsync();
        }


        [Route("auth/regadmin")]
        public async Task<IActionResult> RegAdmin()
        {
           
            RegistrationVm admin = new() { 
                Username = "admin",
                Name = "Rizal",
                Email = "admin@mail.com",
                Password = "Admin@12345#",
                Role = "admin"
            };

            var result = await _userAuthService.RegistrationAsync(admin);
            return Ok(result);
        }
    }
}
