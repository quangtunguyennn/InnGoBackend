namespace InnGoBackend.DTOs.BookingsDTO
{
    public class EditBookingRequestDTO
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string PaymentMethod { get; set; }
        public List<SelectedServiceDTO>? SelectedServices { get; set; }
    }
}
