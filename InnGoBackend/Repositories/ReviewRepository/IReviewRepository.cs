using InnGoBackend.DTOs.ReviewDTOs;
using InnGoHotel.Models;

namespace InnGoBackend.Repositories.ReviewRepository
{
    public interface IReviewRepository
    {
        Task<bool?> review(int userId, ReviewRequestDTO request);
        Task<List<GetAllReviewsResponseDTO>> getAllReviewsOfRoom(int roomId);
        Task<List<GetMyReviewsResponse>> getMyReviews(int userId);
    }
}
