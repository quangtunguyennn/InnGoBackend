using InnGoBackend.DTOs.BookingsDTO;
using InnGoHotel.Models;

namespace InnGoBackend.Repositories.BookingRepository
{
    public interface IBookingRepository
    {
        Task<string?> createNewBooking(CreateNewBookingRequestDTO request, User user);
        Task<List<ViewMyBookingsReponseDTO>> viewMyBookings(int userId);
        Task<GetDetailBookingResponseDTO> getById(int bookingId, int userId);
        Task<string?> editBooking(int bookindId,int userId, EditBookingRequestDTO request);
        Task<bool?> CancelBooking(int bookingId);
        Task<bool?> Checkout(int bookingId);
        Task<bool?> isReviewed( int bookingId);
       
    }
}
