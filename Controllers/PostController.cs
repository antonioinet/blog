using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private HtmlSanitizer _sanitizer = new HtmlSanitizer(
            new List<string>(){"p","h4","blockquote","footer"},
            new List<string>(),
            new List<string>{"class"},
            new List<string>(),
            new List<string>()            
            );
        public PostController(BlogDbContext db)
        {
            _db = db;
        }

        private string sanitize(string content)
        {
            
            return _sanitizer.Sanitize(content);
        }

        [Route("[controller]/{id}")]
        [Route("[controller]/[action]/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(int id)
        {
            Post post = (from p in _db.Posts
                        where p.PostId == id
                        select p).FirstOrDefault();
            if (post == null)
                return new NotFoundResult();

            return View(post);
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
            return RedirectToAction(nameof(Index),post.PostId);
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
        public async Task<IActionResult> New(PostNewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Post post = new Post()
            {
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
            return RedirectToAction(nameof(Index), post.PostId);
        }

    }
}