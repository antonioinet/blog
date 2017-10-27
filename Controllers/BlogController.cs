using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Blog.Models.BlogViewModels;
using Blog.Models.PostViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    public class BlogController: Controller
    {
        private BlogDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        private static readonly int postOnPage = 5;

        public BlogController( 
            UserManager<ApplicationUser> userManager,
            BlogDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }

        private async Task<PostListViewModel> getModel(Blog.Models.Blog blog, int page)
        {
            page = page <= 0 ? 1 : page;
            page--;

            List<Post> postsInDb = 
                (from p in _db.Posts
                where p.Blog.BlogId == blog.BlogId
                orderby p.Created descending
                select p
                )
                .Skip(page * postOnPage)
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

            page ++;
            PostListViewModel model = new PostListViewModel()
            {
                PageHead = blog.Name,
                SecondaryText = blog.Secondary,            
                Posts = posts,
                OlderIndex = page<2 ? null : (int?) page-1,
                NewerIndex = postsInDb.Count > postOnPage ? (int?) page+1 : null
            };
            return model;
        }
        
        [HttpGet]
        [Route("[controller]/[action]/{id}/{page?}")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, int page)
        {

            Blog.Models.Blog blog = 
                (from b in _db.Blogs
                 where b.BlogId == id
                 select b
                ).FirstOrDefault();
            
            if (blog == null)
                return new NotFoundResult();

            var model = await getModel(blog, page);
            return View(model);
        }

        [HttpGet]
        [Route("[controller]/{url}/{page?}")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string url, int page)
        {

            Blog.Models.Blog blog = 
                (from b in _db.Blogs
                 where b.Url == url
                 select b
                ).FirstOrDefault();
            
            if (blog == null)
                return new NotFoundResult();

            var model = await getModel(blog, page);
            return View(model);
        }        

        [Route("[controller]/[action]")]
        [HttpGet]
        public IActionResult New()
        {
            ViewData["HostUrl"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }

        [Route("[controller]/[action]")]
        [HttpPost]
        public async Task<IActionResult> New(BlogNewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Blog.Models.Blog blog = new Blog.Models.Blog()
            {
                Url = model.Url,
                OwnerId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                Name = model.Name,
                Secondary = model.Secondary
            };
            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {id = blog.BlogId});
        }

    }
}