namespace InnGoBackend.DTOs.ReviewDTOs
{
    public class GetMyReviewsResponse
    {
        public int roomId { get; set; }
        public int bookingId { get; set; }
        public string roomName { get; set; }
        public string roomImageUrl { get; set; }
        public DateTime checkInDate { get; set; }
        public DateTime checkOutDate { get; set; }
        public DateTime createdAt { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
    }
}
