namespace InnGoBackend.DTOs.WishlistDTOs
{
    public class ViewWishlistResponseDTO
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchImageUrl { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomImageUrl { get; set; }
        public decimal roomPrice { get; set; }
        public DateTime addedAt { get; set; }
    }
}
