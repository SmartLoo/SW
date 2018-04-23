using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Loo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Loo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AuthenticatedUser> _userManager;
        private readonly SignInManager<AuthenticatedUser> _signInManager;

        public AccountController(
            UserManager<AuthenticatedUser> userManager,
            SignInManager<AuthenticatedUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return new JsonResult("Logout Successful");
        }

        [HttpPost]
        [Route("/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username,
                model.Password, false,false); 

            if (result.Succeeded) 
            {
                return new JsonResult("202");
            } 
            else 
            {
                return new JsonResult(result.ToString());
            }                       
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/register")]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [Route("/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var user = new AuthenticatedUser { UserName = model.Email, Email = model.Email };
            user.Company = "BU";
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Phone = model.Phone;
            user.NotificationOptIn = model.Notifications;
            user.IsAdmin = true;

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {                   
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new JsonResult("202");
            }
            else
            {
                return new JsonResult(result.ToString());
            }
        }

        [HttpGet]
        [Route("/setup_master")]
        [Authorize]
        public IActionResult SetupMaster()
        {
            return View();
        }

        [HttpPost]
        [Route("/setup_master")]
        [Authorize]
        public async Task<IActionResult> SetupMaster([FromBody] SetupMasterViewModel model)
        {
            return new JsonResult(model);
        }
    }
}
