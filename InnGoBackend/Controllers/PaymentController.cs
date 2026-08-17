using InnGoBackend.Repositories.PaymentRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnGoBackend.Controllers
{
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [Authorize(Roles ="User, Receptionist")]
        [HttpPatch("/api/payment/complete")]
        public async Task<IActionResult> completePayment(int bookingId)
        {
            bool? check = await _paymentRepository.completePayment(bookingId);


            if (check == true)
            {
                return Ok(new { message = "complete payment successfully" });
            }
            else
            {
                return BadRequest(new { message = "complete payment failed" });
            }

        }

        [Authorize(Roles ="User, Receptionist")]
        [HttpGet("/api/payment/isCompleted")]
        public async Task<bool> isCompletedPayment(int bookingId)
        {
            var check = await _paymentRepository.isCompletedPayment(bookingId);

            if (check == true)
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
