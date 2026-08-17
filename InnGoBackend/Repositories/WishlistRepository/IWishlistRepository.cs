using InnGoBackend.DTOs.BookingsDTO;
using InnGoBackend.DTOs.WishlistDTOs;
using InnGoHotel.Models;

namespace InnGoBackend.Repositories.WishlistRepository
{
    public interface IWishlistRepository
    {
        Task<bool?> toogleWishlist(int bookingId,int roomId, int userId);
        Task<List<ViewWishlistResponseDTO>>? viewWishlist(int userId); 
    }
}
