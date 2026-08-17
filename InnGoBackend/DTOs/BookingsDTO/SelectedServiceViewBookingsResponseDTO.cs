namespace InnGoBackend.DTOs.BookingsDTO
{
    public class SelectedServiceViewBookingsResponseDTO
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; } // THÊM DÒNG NÀY
        public int Quantity { get; set; }
    }
}
