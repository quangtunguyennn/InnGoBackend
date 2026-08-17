namespace InnGoBackend.DTOs.AuthDTOs
{
    public class ChangePasswordRequestDTO
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmNewPassword { get; set; }
    }
}
