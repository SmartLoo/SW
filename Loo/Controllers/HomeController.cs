using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Loo.Models;

namespace Loo.Controllers
{
    public class HomeController : Controller
    {
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

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
