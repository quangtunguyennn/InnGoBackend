namespace InnGoBackend.DTOs.BookingsDTO
{
    public class GetDetailBookingResponseDTO
    {
        public int branchId { get; set; }
        public int bookingId { get; set; }
        public int roomId { get; set; }
        public string roomName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string roomImageUrl { get; set; }
        public List<SelectedServiceViewBookingsResponseDTO>? selectedServices { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; } // THÊM DÒNG NÀY
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime bookedAt { get; set; }
    }
}
