using InnGoBackend.Models.Context;
using InnGoBackend.Repositories.WishlistRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly InnGoDBContext _context;
        public WishlistController(IWishlistRepository wishlistRepository, InnGoDBContext context)
        {
            _wishlistRepository = wishlistRepository;
            _context = context;
        }

        [Authorize(Roles ="User")]
        [HttpPatch("/api/wishlist/toggle")]
        public async Task<IActionResult> toggleWishlist(int branchId, int roomId)
        {
            var email = User?.Identity?.Name;
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);  
            if (user == null)
            {
                return Unauthorized();
            }

            bool? check = await _wishlistRepository.toogleWishlist(branchId, roomId, user.UserId);

            if(check == true)
            {
                return Ok(new { message = "toggle successfully" });
            }
            else
            {
                return BadRequest(new {message = "toggle failed"});
            }
        }

        [Authorize(Roles ="User")]
        [HttpGet("/api/wishlist/getAll")]
        public async Task<IActionResult> getAll()
        {
            var email = User?.Identity?.Name;
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return Unauthorized();
            }

            var res = await _wishlistRepository.viewWishlist(user.UserId);

            return Ok(res);

        }
    }
}
