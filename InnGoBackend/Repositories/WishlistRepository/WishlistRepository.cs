using InnGoBackend.DTOs.BookingsDTO;
using InnGoBackend.DTOs.WishlistDTOs;
using InnGoBackend.Models.Context;
using InnGoHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.WishlistRepository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly InnGoDBContext _context;
        public WishlistRepository(InnGoDBContext context)
        {
            _context = context;
        }
        public async Task<bool?> toogleWishlist(int branchId, int roomId, int userId)
        {
            try
            {
                // 1. Tìm bản ghi cụ thể của chính user này
                var existingItem = await _context.Wishlist
                    .FirstOrDefaultAsync(x => x.BranchId == branchId
                                           && x.RoomId == roomId
                                           && x.UserId == userId); // Phải có UserId

                if (existingItem != null)
                {
                    // 2. NẾU ĐÃ TỒN TẠI -> Xóa (Unlike)
                    _context.Wishlist.Remove(existingItem);
                }
                else
                {
                    // 3. NẾU CHƯA TỒN TẠI -> Thêm mới (Like)
                    var newFavorite = new Wishlist()
                    {
                        UserId = userId,
                        BranchId = branchId,
                        RoomId = roomId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Wishlist.AddAsync(newFavorite);
                }

                // 4. Lưu thay đổi vào Database
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                Console.WriteLine($"Toggle wishlist error: {ex.Message}");
                return false;
            }
        }



        public async Task<List<ViewWishlistResponseDTO>>? viewWishlist(int userId)
        {
            // THÊM OrderByDescending Ở ĐÂY: Sắp xếp theo CreatedAt từ mới nhất -> cũ nhất
            var wishlist = await _context.Wishlist
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var res = new List<ViewWishlistResponseDTO>();

            foreach (var wish in wishlist)
            {
                var branch = await _context.Branch.FirstOrDefaultAsync(x => x.BranchId == wish.BranchId);
                var room = await _context.Room.FirstOrDefaultAsync(x => x.RoomId == wish.RoomId);

                res.Add(new ViewWishlistResponseDTO()
                {
                    BranchId = wish.BranchId,
                    BranchName = branch?.Name,
                    BranchImageUrl = branch?.ImageUrl,
                    address = branch?.Address,
                    city = branch?.City,
                    RoomId = wish.RoomId,
                    RoomName = room?.RoomType,
                    RoomImageUrl = room?.ImageUrl,
                    roomPrice = room?.Price ?? 0, // Đề phòng room null
                    addedAt = wish.CreatedAt
                });
            }
            return res;
        }
    }
}
