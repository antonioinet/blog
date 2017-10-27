using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Blog.Models.ImageViewModels;
using System.IO;

namespace Blog.Controllers
{
    

    [Authorize]
    public class ImageController:Controller
    {
        
        private BlogDbContext _db;
        private static readonly List<string> allowedContentType = new List<string>()
            {"image/jpeg", "image/png"};

        [TempData]
        public string StatusMessage { get; set; }
        public ImageController(
            BlogDbContext db)
        {
            _db = db;
        }
        
        [HttpGet]
        [Route("[controller]/{url}")]
        [Route("[controller]/[action]/{url}")]
        [AllowAnonymous]
        public IActionResult Index(string url)
        {
            Image image = 
                    (from i in _db.Images
                    where i.Url == url
                    select i).FirstOrDefault();
            if (image == null)
                return new NotFoundResult();
            
            return File(image.Content, image.ContentType);
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Upload(ImageUploadViewModel imageUpload)
        {
            IFormFile imgF = Request.Form.Files.FirstOrDefault();
            if (
                imgF.Length == 0 || 
                imgF.Length > 5000000 || 
                !allowedContentType.Contains(imgF.ContentType))
                return BadRequest();

            using(var contentStream = imgF.OpenReadStream())
            {
                byte[] content = new byte[contentStream.Length];
                await contentStream.ReadAsync(content, 0, (int) contentStream.Length);
                Image image = new Image()
                {
                    Url = imageUpload.Url,
                    ContentType = imgF.ContentType,
                    Content = content
                };

                _db.Images.Add(image);
                await _db.SaveChangesAsync();
            }
            
            StatusMessage = "Your image has been uploaded";
            return RedirectToAction(nameof(Upload));
        }
    }
}