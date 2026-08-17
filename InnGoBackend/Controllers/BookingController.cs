using InnGoBackend.DTOs.BookingsDTO;
using InnGoBackend.Models.Context;
using InnGoBackend.Repositories.BookingRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly InnGoDBContext _context;
        public BookingController(IBookingRepository bookingRepository, InnGoDBContext context)
        {
            _bookingRepository = bookingRepository;
            _context = context;
        }

        [Authorize(Roles ="User")]
        [HttpPost("/api/booking/createNewBooking")]
        public async Task<IActionResult> createNewBooking(CreateNewBookingRequestDTO request)
        {
            var email = User?.Identity?.Name;

            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);
            if(user == null)
            {
                return Unauthorized();
            }

            var message = await _bookingRepository.createNewBooking(request, user);

            return Ok(message);
        }

        [Authorize(Roles ="User,Admin, Receptionist")]
        [HttpGet("/api/booking/viewMyBookings")]
        public async Task<IActionResult> viewMyBookings()
        {
            var email = User?.Identity?.Name;

            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return Unauthorized();
            }

            var res = await _bookingRepository.viewMyBookings(user.UserId);
            return Ok(res);
        }

        [Authorize(Roles ="User, Receptionist")]
        [HttpPatch("/api/booking/cancel/{bookingId}")]
        public async Task<IActionResult> cancelBooking(int bookingId)
        {
            var email = User?.Identity?.Name;

            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return Unauthorized();
            }

            bool? check = await _bookingRepository.CancelBooking(bookingId);

            if(check == true)
            {
                return Ok(new { message = "cancel booking successfully" });
            }
            else
            {
                return BadRequest(new { message = "cancel booking failed" });
            }
        }

        [Authorize(Roles ="Receptionist")]
        [HttpPatch("/api/booking/checkout/{bookingId}")]
        public async Task<IActionResult> checkout(int bookingId)
        {
            bool? check = await _bookingRepository.Checkout(bookingId);

            if(check == true)
            {
                return Ok(new { message = "check out successfully" });
            }
            else
            {
                return BadRequest(new { message = "check out failed" });
            }
        }

        [Authorize(Roles ="User")]
        [HttpPut("/api/booking/edit")]
        public async Task<IActionResult> editBooking(int bookingId, EditBookingRequestDTO request)
        {
            var email = User?.Identity?.Name;

            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return Unauthorized();
            }

            var res = await _bookingRepository.editBooking(bookingId,user.UserId, request);

            return Ok(res);

        }

        [Authorize(Roles ="User")]
        [HttpGet("/api/booking/get/{bookingId}")]
        public async Task<IActionResult> getById(int bookingId)
        {

            var email = User?.Identity?.Name;

            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return Unauthorized();
            }

            var  res =  await _bookingRepository.getById(bookingId, user.UserId);

            return Ok(res);
        }

        [HttpGet("/api/booking/isReviewed")]
        public async Task<bool> isReviewed(int bookingId)
        {
            var check = await _bookingRepository.isReviewed(bookingId);

            if(check == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
