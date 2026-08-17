using InnGoBackend.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Mvc;

namespace InnGoBackend.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        public Task<string?> Login(LoginRequestDTO request);
        public Task<bool?> Register(RegisterRequestDTO request);
        public Task<GetProfileResponseDTO> GetProfile(int userId);
        public Task<bool?> EditProfile(EditProfileRequestDTO request, int userId);
        public Task<bool?> ChangePassword(ChangePasswordRequestDTO request, int userId);
    }
}
