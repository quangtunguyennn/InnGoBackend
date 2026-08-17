using InnGoBackend.DTOs.AuthDTOs;
using InnGoBackend.Models.Context;
using InnGoBackend.Repositories.AuthRepository;
using InnGoHotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly InnGoDBContext _context;
        public AuthController(IAuthRepository authRepository, InnGoDBContext context)
        {
            _authRepository = authRepository;
            _context = context;
        }

        [HttpPost("/api/auth/login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
           var token = await _authRepository.Login(request);
            return Ok(new { token = token });
        }

        [HttpPost("/api/auth/register")]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
        {
            bool? check = await _authRepository.Register(request);

            if (check == true)
            {
                return (Ok(new { message = "register successfully" }));
            }
            else
            {
                return BadRequest(new { message = "email is currently used" });
            }
        }

        [Authorize(Roles ="User")]
        [HttpGet("/api/auth/getProfile")]
        public async Task<IActionResult> getProfile()
        {
            var email = User?.Identity?.Name;

            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);

            if(user == null)
            {
                return Unauthorized();
            }

            var res = await _authRepository.GetProfile(user.UserId);
            return Ok(res);
        }


        [Authorize(Roles ="User")]
        [HttpPut("/api/auth/Profile")]
        public async Task<IActionResult> editProfile(EditProfileRequestDTO request)
        {
            var email = User?.Identity?.Name;
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);

            bool? check = await _authRepository.EditProfile(request, user.UserId);

            if(check == true)
            {
                return Ok(new { message = "edited successfully" });
            }
            else
            {
                return BadRequest(new { message = "edited failed" });
            }
        }

        [Authorize(Roles ="User")]
        [HttpPut("/api/auth/changePassword")]
        public async Task<IActionResult> changePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var email = User?.Identity?.Name;
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);

            bool? check = await _authRepository.ChangePassword(request, user.UserId);

            if (check == true)
            {
                return Ok(new { message = "changed password successfully" });
            }
            else
            {
                return BadRequest(new { message = "changed password failed" });
            }
        }
    }
}
