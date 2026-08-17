using InnGoBackend.DTOs.ReviewDTOs;
using InnGoBackend.Models.Context;
using InnGoHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.ReviewRepository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly InnGoDBContext _context;
        public ReviewRepository(InnGoDBContext context)
        {
            _context = context;
        }
        public async Task<List<GetAllReviewsResponseDTO>> getAllReviewsOfRoom(int roomId)
        {
            var res = await (from review in _context.Review
                             where review.RoomId == roomId

                             // Left Join để lấy tên User (tránh lỗi nếu User bị xóa)
                             join user in _context.User on review.UserId equals user.UserId into userGroup
                             from user in userGroup.DefaultIfEmpty()

                             select new GetAllReviewsResponseDTO
                             {
                                 reviewId = review.ReviewId,
                                 roomId = review.RoomId,
                                 userId = review.UserId,
                                 userName = user != null ? user.FullName : "Người dùng ẩn danh",
                                 rating = review.Rating,
                                 comment = review.Comment,
                             }).ToListAsync();

            return res;
        }

        public async Task<List<GetMyReviewsResponse>> getMyReviews(int userId)
        {
            // Sử dụng LINQ Query để Join các bảng Review, Room và lấy Booking tương ứng
            var query = from review in _context.Review
                        where review.UserId == userId

                        // 1. Kết nối với bảng Room để lấy Tên phòng và Hình ảnh
                        join room in _context.Room on review.RoomId equals room.RoomId

                        // 2. Tìm Booking (chuyến đi) của User tại Phòng này để lấy Ngày CheckIn / CheckOut
                        // Lấy chuyến đi gần nhất (OrderByDescending) phòng trường hợp user đặt phòng này nhiều lần
                        let booking = _context.Booking
                            .Where(b => b.UserId == userId && b.RoomId == review.RoomId)
                            .OrderByDescending(b => b.CheckOutDate)
                            .FirstOrDefault()

                        // 3. Map thẳng dữ liệu vào DTO
                        select new GetMyReviewsResponse
                        {
                            bookingId = booking.BookingId,
                            roomId = review.RoomId,
                            rating = review.Rating,
                            comment = review.Comment,

                            // --- MAP CÁC TRƯỜNG CÒN THIẾU ---
                            // Lưu ý: Bạn hãy đổi room.Name, room.ImageUrl... cho khớp với tên property trong Model (Entity) của bạn
                            roomName = room.RoomType,      // Có thể model của bạn là room.Name hoặc room.Title
                            roomImageUrl = room.ImageUrl,  // Có thể model của bạn là room.Image hoặc room.Thumbnail
                            createdAt = booking.CreatedAt,  // Đảm bảo bảng Review của bạn có cột ngày tạo

                            checkInDate = booking != null ? booking.CheckInDate : default,
                            checkOutDate = booking != null ? booking.CheckOutDate : default
                        };

            // Thực thi câu lệnh SQL và trả về List
            var res = await query.ToListAsync();

            return res;
        }

        public async Task<bool?> review(int userId, ReviewRequestDTO request)
        {
            var newReview = new Review()
            {
                BookingId = request.bookingId,
                RoomId = request.roomId,
                UserId = userId,
                Rating = request.rating,
                Comment = request.comment,
                CreatedAt = DateTime.Now
            };

            await _context.AddAsync(newReview);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
