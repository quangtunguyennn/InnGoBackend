using InnGoBackend.DTOs.ReviewDTOs;
using InnGoBackend.Models.Context;
using InnGoBackend.Repositories.ReviewRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Controllers
{
    [Route("/api[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly InnGoDBContext _context;
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(InnGoDBContext context, IReviewRepository reviewRepository)
        {
            _context = context;
            _reviewRepository = reviewRepository;
        }

        [AllowAnonymous]
        [HttpGet("/api/review/getAll")]
        public async Task<IActionResult> getAll(int roomId)
        {
            var res =  await _reviewRepository.getAllReviewsOfRoom(roomId);
            return Ok(res);
        }

        [Authorize(Roles = "User")]
        [HttpPost("/api/review/post")]
        public async Task<IActionResult> review(ReviewRequestDTO request)
        {
            var email = User?.Identity?.Name;
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email ==  email);

            if (user == null)
            {
                return Unauthorized();
            }

            bool? check = await _reviewRepository.review(user.UserId, request);

            if(check == true)
            {
                return Ok(new {message = "review posted successfully"});
            }
            else
            {
                return BadRequest(new { message = "review posted failed" });
            }
        }

        [Authorize(Roles ="User")]
        [HttpGet("/api/review/my")]
        public async Task<IActionResult> getMyReview()
        {
            var email = User?.Identity?.Name;
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return Unauthorized();
            }

            var res = await _reviewRepository.getMyReviews(user.UserId);
            return Ok(res);

          
        }
    }
}
