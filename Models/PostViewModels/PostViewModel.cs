using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.PostViewModels
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Created { get; set; }
        public string Lead { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}