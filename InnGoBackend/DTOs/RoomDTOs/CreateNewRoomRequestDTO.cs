namespace InnGoBackend.DTOs.RoomDTOs
{
    public class CreateNewRoomRequestDTO
    {
        public int BranchId { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public string ImageURL { get; set; }
    }
}
