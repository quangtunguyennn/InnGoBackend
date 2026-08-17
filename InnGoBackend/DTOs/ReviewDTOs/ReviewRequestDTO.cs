namespace InnGoBackend.DTOs.ReviewDTOs
{
    public class ReviewRequestDTO
    {
        public int bookingId { get; set; }
        public int roomId { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }

    }
}
