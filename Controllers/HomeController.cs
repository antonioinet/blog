using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Models;
using Blog.Data;
using Blog.Models.PostViewModels;
using Microsoft.AspNetCore.Identity;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private BlogDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        private static readonly int postOnPage = 5;
        public HomeController(
            UserManager<ApplicationUser> userManager,
            BlogDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id)
        {
            id = id <= 0 ? 1 : id;
            id--;
            
            List<Post> postsInDb = 
                (from p in _db.Posts
                orderby p.Created descending
                select p
                )
                .Skip(id * postOnPage)
                .Take(postOnPage+1)
                .ToList();
            List<PostViewModel> posts = new List<PostViewModel>();
            foreach(var p in postsInDb.Take(postOnPage))
            {
                var user = await _userManager.FindByNameAsync(p.AuthorId);

                posts.Add(new PostViewModel()
                {
                    PostId=p.PostId,
                    AuthorId=p.AuthorId,
                    AuthorName=user?.AuthorName,
                    Created=p.Created,
                    Content=p.Content,
                    ImageUrl=p.ImageUrl,
                    Lead=p.Lead,
                    Title=p.Title,
                    Url=p.Url
                });
            }

            id ++;
            PostListViewModel model = new PostListViewModel()
            {
                Posts = posts,
                OlderIndex = id<2 ? null : (int?) id-1,
                NewerIndex = postsInDb.Count > postOnPage ? (int?) id+1 : null
            };
            return View(model);
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
