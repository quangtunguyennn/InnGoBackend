namespace InnGoBackend.Repositories.PaymentRepository
{
    public interface IPaymentRepository
    {
        Task<bool?> completePayment(int bookingId);
        Task<bool?> isCompletedPayment(int bookingId);
    }
}
