using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Blog.Models.PostViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Blog.Data;
using Blog.Models;
using System;
using Ganss.XSS;
using System.Collections.Generic;

namespace Blog.Controllers
{

    [Authorize]
    public class PostController:Controller
    {
        private BlogDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private HtmlSanitizer _sanitizer = new HtmlSanitizer(
            new List<string>(){"p","h4","blockquote","footer"},
            new List<string>(),
            new List<string>{"class"},
            new List<string>(),
            new List<string>()            
            );
        public PostController(
            UserManager<ApplicationUser> userManager,
            BlogDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        private string sanitize(string content)
        {
            
            return _sanitizer.Sanitize(content);
        }

        [HttpGet]
        [Route("[controller]/[action]/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id)
        {
            Post p = (from post in _db.Posts
                        where post.PostId == id
                        select post).FirstOrDefault();
            if (p == null)
                return new NotFoundResult();
            
            var user = await _userManager.FindByNameAsync(p.AuthorId);
            return View(new PostViewModel()
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

        [HttpGet]
        [Route("[controller]/{url}")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string url)
        {
            Post p = (from post in _db.Posts
                        where post.Url == url
                        select post).FirstOrDefault();
            if (p == null)
                return new NotFoundResult();
            
            var user = await _userManager.FindByNameAsync(p.AuthorId);
            return View(new PostViewModel()
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

        [Route("[controller]/[action]/{id}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            PostNewViewModel post = (from p in _db.Posts
                        where p.PostId == id
                        select new PostNewViewModel()
                        {
                            Url = p.Url,
                            ImageUrl = p.ImageUrl,
                            Lead = p.Lead,
                            Title = p.Title,
                            Content = p.Content
                        }).FirstOrDefault();
            if (post == null)
                return new NotFoundResult();
            return View(post);        
        }

        [Route("[controller]/[action]/{id}")]
        [HttpPost]
        public async Task<IActionResult> Edit(PostNewViewModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Post post = (from p in _db.Posts
                        where p.PostId == id
                        select p).FirstOrDefault();
            if (post == null)
                return new NotFoundResult();
            post.AuthorId = this.User.FindFirstValue(ClaimTypes.Name);
            post.Url = model.Url;
            post.ImageUrl = model.ImageUrl;
            post.Created = DateTime.UtcNow;
            post.Lead = model.Lead;
            post.Title = model.Title;            
            post.Content = sanitize(model.Content);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {id = post.PostId});
        }
        
        [Route("[controller]/[action]")]
        [HttpGet]
        public IActionResult New()
        {
            ViewData["HostUrl"] = $"{this.Request.Scheme}://{this.Request.Host}";
            ViewData["BlogList"] = _db.Blogs.ToList();
            return View();
        }

        [Route("[controller]/[action]")]
        [HttpPost]
        public async Task<IActionResult> New(PostNewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Blog.Models.Blog blog=
                (from b in _db.Blogs
                where b.BlogId == model.BlogId
                select b).FirstOrDefault();
            if (blog == null)
                return new NotFoundResult();

            Post post = new Post()
            {
                Blog = blog,
                AuthorId = this.User.FindFirstValue(ClaimTypes.Name),
                Url = model.Url,
                ImageUrl = model.ImageUrl,
                Created = DateTime.UtcNow,
                Lead = model.Lead,
                Title = model.Title,
                Content = sanitize(model.Content)
            };
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new {id = post.PostId});
        }

    }
}