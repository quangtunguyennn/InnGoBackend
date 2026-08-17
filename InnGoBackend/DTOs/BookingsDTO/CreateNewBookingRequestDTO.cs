namespace InnGoBackend.DTOs.BookingsDTO
{
    public class CreateNewBookingRequestDTO
    {
        public int roomId {  get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string PaymentMethod { get; set; }
        public List<SelectedServiceDTO>? SelectedServices { get; set; }
    }
}
