namespace InnGoBackend.DTOs.AuthDTOs
{
    public class GetProfileResponseDTO
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
        public int TotalBookings { get; set; }
        
    }
}
