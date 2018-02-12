using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kalkul.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "About Calculator.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Mailis Toompuu.";

            return View();
        }


        public IActionResult Abstract()
        {
            ViewData["Message"] = "IGeneralOperator & IOperator.";

            return View();
        }

        public IActionResult Example()
        {
            ViewData["Message"] = "Examples of an operator class.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
