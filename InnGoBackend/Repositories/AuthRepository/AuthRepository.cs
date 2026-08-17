using Azure.Core;
using InnGoBackend.DTOs.AuthDTOs;
using InnGoBackend.Models;
using InnGoBackend.Models.Context;
using InnGoHotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InnGoBackend.Repositories.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly InnGoDBContext _context;
        private readonly IConfiguration _configuration;
        public AuthRepository(InnGoDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string?> Login(LoginRequestDTO request)
        {
            var user = _context.User.FirstOrDefault(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
                return null;
            var roles = await _context.UserRole
            .Where(ur => ur.UserId == user.UserId)
            .Select(ur => ur.Role.RoleName) 
            .ToListAsync();

            var token = GenerateJwtToken(user, roles);
            return token;
        }
        private string GenerateJwtToken(User user, List<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.Email),
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        

        public async Task<bool?> Register(RegisterRequestDTO request)
        {
            if(await _context.User.FirstOrDefaultAsync(x => x.Email == request.Email) != null)
            {
                return false;
            }
            else
            {
                var user = new User()
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = request.Password,
                    Phone = null,
                    MembershipTier = null,
                    CreatedAt = DateTime.Now
                };

                await _context.AddAsync(user);
                await _context.SaveChangesAsync();

                var newUserRole = new UserRole()
                {
                    UserId = user.UserId,
                    RoleId = 6
                };

                await _context.AddAsync(newUserRole);
                await _context.SaveChangesAsync();

                return true;
            }

               


        }

        public async Task<GetProfileResponseDTO> GetProfile(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.UserId == userId);

            var countBookings = await _context.Booking.CountAsync(x => x.UserId == userId);
            var res = new GetProfileResponseDTO()
            {
                Fullname = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                TotalBookings = countBookings
            };

            return res;
        }

        public async Task<bool?> EditProfile(EditProfileRequestDTO request, int userId)
        {
             var user = await _context.User.FirstOrDefaultAsync(x => x.UserId == userId);

            user.FullName = request.NewFullName;
            user.Email = request.NewEmail;
            user.Phone = request.NewPhone;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool?> ChangePassword(ChangePasswordRequestDTO request, int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.UserId == userId);

            var oldPasswordFromDB = user.PasswordHash;

            if(request.oldPassword != oldPasswordFromDB)
            {
                return false;
            }

            if(request.newPassword == request.confirmNewPassword)
            {
                user.PasswordHash = request.newPassword;
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}
