using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Blog.Models.ImageViewModels
{
    public class ImageUploadViewModel
    {
        public string Url { get; set; }
    }
}