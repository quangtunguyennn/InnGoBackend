namespace InnGoBackend.DTOs.BlogDTOs
{
    public class CreateNewBlogRequestDTO
    {
        public string title { get; set; }
        public string slug { get; set; }
        public string summary { get; set; }
        public string content { get; set; }
        public string thumbnailUrl { get; set; }
        
    }
}
