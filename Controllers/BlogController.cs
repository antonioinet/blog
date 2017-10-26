using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Blog.Models.BlogViewModels;
using Blog.Models.PostViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        
        [HttpGet()]
        [Route("[controller]/{authorId}/{id?}")]
        public async Task<IActionResult> Index(string authorId, int id)
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

            var user = await _userManager.FindByNameAsync(authorId);

            foreach(var p in postsInDb.Take(postOnPage))
            {
                

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
                PageHead = user?.AuthorName,                
                Posts = posts,
                OlderIndex = id<2 ? null : (int?) id-1,
                NewerIndex = postsInDb.Count > postOnPage ? (int?) id+1 : null
            };
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
                Name = model.Name,
                Secondary = model.Secondary
            };
            _db.Blogs.Add(blog);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {id = blog.BlogId});
        }

    }
}