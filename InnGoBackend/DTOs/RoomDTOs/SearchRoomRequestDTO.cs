using System.ComponentModel.DataAnnotations;

namespace InnGoBackend.DTOs.RoomDTOs
{
    public class SearchRoomRequestDTO
    {
        [Required]
        public int BranchId { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        public int? Capacity { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
  
    }
}
