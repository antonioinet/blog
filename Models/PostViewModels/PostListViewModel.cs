using System.Collections.Generic;
namespace Blog.Models.PostViewModels
{
    public class PostListViewModel
    {
        public List<PostViewModel> Posts { get; set; }
        public int? OlderIndex { get; set; }
        public int? NewerIndex { get; set; }
    }
}