using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using Blog.Data;
using Blog.Models.BlogViewModels;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private BlogDbContext _db;
        public HomeController(BlogDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Post> posts = _db.Posts.ToList();
            return View(posts);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
