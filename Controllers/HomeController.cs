using System;
using Microsoft.AspNetCore.Mvc;

namespace MediaPlus.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomeController1
        public ActionResult Index()
        {
            return View();
        }

    }
}
