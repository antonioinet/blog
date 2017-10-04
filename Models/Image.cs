namespace Blog.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}