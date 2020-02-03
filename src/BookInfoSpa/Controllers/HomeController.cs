using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LaunchDarkly.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookInfoSpa.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //get base url
            ViewData["BaseUrl"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/";

            //if not logged in, redirect to login page
            if (HttpContext.Session.GetString(SessionConstant.User) == null)
                return RedirectToAction("Login");

            //get flags for user
            var currentUser = HttpContext.Session.GetString(SessionConstant.User).ToString();
            User user = LaunchDarkly.Client.User.WithKey(currentUser);
            var bookInsertNewForm = Startup.FeatureFlag.BoolVariation(Flags.BookInsertNewForm, user, false);
            var bookNewVisualRating = Startup.FeatureFlag.BoolVariation(Flags.BookListVisualRating, user, false);

            //put flags into home to avoid calling from client
            Dictionary<string, bool> flags = new Dictionary<string, bool>();
            flags.Add(Flags.BookInsertNewForm, bookInsertNewForm);
            flags.Add(Flags.BookListVisualRating, bookNewVisualRating);

            return View("Index", flags);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm]string username)
        {
            HttpContext.Session.SetString(SessionConstant.User, username);
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }


    }
}
