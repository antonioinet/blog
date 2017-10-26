using System.Collections.Generic;

namespace Blog.Models
{
    public class Blog
    {
        public int BlogId { get; set;}
        public string Url { get; set; }
        public string Name { get; set; }
        public string Secondary { get; set; }
        public List<Post> Posts { get; set; }
    }

}