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
        [Route("/register")]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [Route("/register")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Register([FromBody] RegisterViewModel model)
        {
            HttpResponseMessage res = new HttpResponseMessage();

            if (ModelState.IsValid)
            {
                var user = new AuthenticatedUser { UserName = model.Email, Email = model.Email };
                user.Company = "BU";
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.RegistrationCode = model.RegistrationCode;

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    res.StatusCode = HttpStatusCode.OK;
                    res.ReasonPhrase = "Account registered successfully";

                    await _signInManager.SignInAsync(user, isPersistent: false);
                }
                else
                {
                    res.StatusCode = HttpStatusCode.BadRequest;
                    res.ReasonPhrase = result.ToString();
                }
            }

            return res;
        }
    }
}
