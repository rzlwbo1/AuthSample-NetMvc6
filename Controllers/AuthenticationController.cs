using Microsoft.AspNetCore.Mvc;

namespace AuthSampleRoleBased.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
