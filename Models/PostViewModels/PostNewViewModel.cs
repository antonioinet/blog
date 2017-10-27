using System.ComponentModel.DataAnnotations;

namespace Blog.Models.PostViewModels
{
    public class PostNewViewModel
    {
        [Required]
        public int BlogId { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public string Lead { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}