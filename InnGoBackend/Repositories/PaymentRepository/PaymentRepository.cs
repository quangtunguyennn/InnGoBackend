
using InnGoBackend.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly InnGoDBContext _context;
        public PaymentRepository(InnGoDBContext context)
        {
            _context = context;
        }
        public async Task<bool?> completePayment(int bookingId)
        {
            var booking = await _context.Booking.FirstOrDefaultAsync(x => x.BookingId == bookingId);    
            if (booking == null)
            {
                return false;
            }

            var payment = await _context.Payment.FirstOrDefaultAsync(x => x.BookingId == booking.BookingId);

            if(payment == null)
            {
                return false;
            }

            payment.PaymentStatus = "Paid";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool?> isCompletedPayment(int bookingId)
        {
              
            var payment = await _context.Payment.FirstOrDefaultAsync(x => x.BookingId == bookingId);

            if (payment == null)
            {
                return false;
            };

            if (payment.PaymentStatus != "Paid")
            {
                return false;
            }
            return true;
        }
    }
}
