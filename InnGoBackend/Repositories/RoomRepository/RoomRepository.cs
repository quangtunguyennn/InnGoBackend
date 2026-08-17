using InnGoBackend.DTOs.RoomDTOs;
using InnGoBackend.Models.Context;
using InnGoHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.RoomRepository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly InnGoDBContext _context;
        public RoomRepository(InnGoDBContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateNewRoom(CreateNewRoomRequestDTO request)
        {
            var newRoom = new Room()
            {
                BranchId = request.BranchId,
                RoomType = request.RoomType,    
                Price = request.Price,
                Capacity = request.Capacity,
                Status = request.Status,
                ImageUrl = request.ImageURL
            };

         
            await _context.AddAsync(newRoom);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Room>> getAllByBranchId(int branchId)
        {
            return await _context.Room.Where(x => x.BranchId == branchId).ToListAsync();
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await _context.Room.ToListAsync();
        }

        public async Task<decimal?> getAverageRating(int roomId)
        {
            // Lấy danh sách Rating của phòng đó
            var ratings = _context.Review.Where(x => x.RoomId == roomId);

            // Nếu chưa có review nào thì trả về null (hoặc 0)
            if (!await ratings.AnyAsync())
            {
                return null; // Hoặc return 0;
            }

            // SQL sẽ tự tính AVG và ép kiểu về decimal
            var avg = await ratings.AverageAsync(x => (decimal)x.Rating);

            // Tròn đến 1 chữ số thập phân (Ví dụ: 4.3)
            return Math.Round(avg, 1);
        }

        public async Task<Room> getRoomById(int id)
        {
            return await _context.Room.FirstOrDefaultAsync(x => x.RoomId == id);
        }

        public async Task<List<Room>?> searchRooms(SearchRoomRequestDTO request)
        {
            // Giả định bạn đã inject DbContext vào service với tên _context

            // 1. Khởi tạo câu truy vấn gốc
            IQueryable<Room> query = _context.Room;

            query = query.Where(x => x.BranchId == request.BranchId);
            // 2. XỬ LÝ ĐIỀU KIỆN BẮT BUỘC: Lọc phòng trống theo ngày
            // Logic: Tìm các phòng MÀ KHÔNG CÓ ( !Any ) bất kỳ Booking nào bị trùng lịch.
            // Hai khoảng thời gian giao nhau khi: (CheckIn Cũ < CheckOut Mới) VÀ (CheckOut Cũ > CheckIn Mới)
            query = query.Where(room => !room.Bookings.Any(booking =>
                booking.CheckInDate < request.CheckOutDate &&
                booking.CheckOutDate > request.CheckInDate &&
                (booking.Status != "Cancelled") // Đảm bảo bỏ qua các đơn đã hủy (tùy vào logic Enum/String của bạn)
            ));

            // 3. XỬ LÝ ĐIỀU KIỆN TÙY CHỌN (Chỉ áp dụng nếu có giá trị)

            // Lọc theo giá tối thiểu
            if (request.minPrice.HasValue)
            {
                query = query.Where(r => r.Price >= request.minPrice.Value);
            }

            // Lọc theo giá tối đa
            if (request.maxPrice.HasValue)
            {
                query = query.Where(r => r.Price <= request.maxPrice.Value);
            }

            // Lọc theo loại phòng
            query = query.Where(x => x.Capacity >= request.Capacity);

            // 4. Thực thi truy vấn xuống DB và lấy kết quả
            var availableRooms = await query.ToListAsync();

            return availableRooms;
        }
    }
}
