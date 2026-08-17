namespace InnGoBackend.DTOs.ReviewDTOs
{
    public class GetAllReviewsResponseDTO
    {
        public int reviewId {  get; set; }
        public int roomId { get; set; }
        public int userId { get; set; }
        public string userName { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
    }
}
