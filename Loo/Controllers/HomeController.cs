using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Loo.Models;
using Microsoft.AspNetCore.Authorization;

namespace Loo.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("restrooms")]
        public IActionResult Restrooms()
        {
            return View();
        }

        [Route("schedule")]
        public IActionResult Schedule()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
