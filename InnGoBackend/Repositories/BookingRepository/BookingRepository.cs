using InnGoBackend.DTOs.BookingsDTO;
using InnGoBackend.Models.Context;
using InnGoHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.BookingRepository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly InnGoDBContext _context;
        public BookingRepository(InnGoDBContext context)
        {
            _context = context;
        }

        public async Task<bool?> CancelBooking(int bookingId)
        {
            // Tìm booking theo ID
            var booking = await _context.Booking.FirstOrDefaultAsync(x => x.BookingId == bookingId);

            // 1. Kiểm tra null
            if (booking == null)
            {
                return false;
            }

            // 2. Kiểm tra nếu booking đã bị hủy trước đó rồi (Tùy thuộc vào thiết kế DB của bạn)
            if (booking.Status == "Cancelled")
            {
                return false;
            }

            // LƯU Ý: Đảm bảo now và CheckInDate cùng 1 chuẩn Múi giờ (Ví dụ: cùng là UTC)
            DateTime now = DateTime.UtcNow;

            // 3. Gộp điều kiện: Nếu hiện tại đã tới hoặc qua giờ Check-in -> Không cho hủy
            if (now >= booking.CheckInDate)
            {
                return false;
            }

            // 4. Tính toán thời gian chính xác: Phải hủy trước giờ Check-in ít nhất 48 tiếng (2 ngày)
            // Dùng TotalHours để chính xác đến từng phút thay vì dùng .Days
            if ((booking.CheckInDate - now).TotalHours < 48)
            {
                return false;
            }

            // ==========================================
            // 5. THỰC THI HÀNH ĐỘNG HỦY (Đảm bảo toàn vẹn dữ liệu)
            // ==========================================

            try
            {
                booking.Status = "Cancelled";

                // Cập nhật lại thông tin vào Context
                _context.Booking.Update(booking);

                // Lưu thay đổi vào Database
                var saved = await _context.SaveChangesAsync();

                // Trả về true nếu số dòng bị ảnh hưởng lớn hơn 0
                return saved > 0;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi tại đây nếu cần thiết
                Console.WriteLine($"Lỗi khi hủy phòng: {ex.Message}");
                return false;
            }
        }
        public async Task<bool?> Checkout(int bookingId)
        {
            // 1. Tìm booking
            var booking = await _context.Booking.FirstOrDefaultAsync(x => x.BookingId == bookingId);
            var payment = await _context.Payment.FirstOrDefaultAsync(x => x.BookingId == booking.BookingId);

            if (booking == null)
            {
                return false;
            }

            // 2. Chặn các trạng thái không hợp lệ (Chỉ cho phép check-out nếu phòng đang được sử dụng)
            // (Giả sử trạng thái đang ở trong phòng là "Checked in" hoặc "Confirmed")
            if (booking.Status == "Checked out" || booking.Status == "Cancelled")
            {
                return false; // Không thể check-out một phòng đã out hoặc đã hủy
            }

            // 3. Kiểm tra thanh toán (Bảo vệ doanh thu)
            // Nếu khách chưa thanh toán tiền phòng/dịch vụ thì không cho check-out trên hệ thống
            if (payment.PaymentStatus != "Paid")
            {
                // Có thể throw Exception hoặc return false tùy cách bạn handle trên UI
                // throw new InvalidOperationException("Vui lòng thanh toán toàn bộ chi phí trước khi Check-out.");
                return false;
            }

            /* 
             * ĐÃ XÓA logic chặn Check-out sớm. 
             * Khách hoàn toàn có quyền trả phòng sớm hơn CheckOutDate.
             */

            // 4. Thực thi Check-out
            try
            {
                booking.Status = "Checked out";

                // Tùy chọn nâng cao: Lưu lại thời gian trả phòng thực tế để làm báo cáo
                // booking.ActualCheckOutDate = DateTime.UtcNow; 

                // EF Core tự động tracking entity nên chỉ cần SaveChangesAsync
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Nên ghi log chi tiết (Sử dụng ILogger thay vì Console.WriteLine trong dự án thực tế)
                Console.WriteLine($"Lỗi khi check out đơn {bookingId}: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> createNewBooking(CreateNewBookingRequestDTO request, User user)
        {
            // 1. Kiểm tra tính hợp lệ của thời gian (Nên kiểm tra cái này đầu tiên để tránh query DB dư thừa)
            if (request.CheckInDate.Date < DateTime.Today)
            {
                return "Check-in date cannot be in the past.";
            }

            if (request.CheckOutDate <= request.CheckInDate)
            {
                return "Check-out date must be after check-in date.";
            }

            // 2. Kiểm tra phòng có tồn tại và đang sẵn sàng không
            var room = await _context.Room.FirstOrDefaultAsync(x => x.RoomId == request.roomId);

            if (room == null || room.Status != "Available")
            {
                return "Room is not available.";
            }

            // 3. THUẬT TOÁN KIỂM TRA TRÙNG LỊCH CHUẨN
            // Chỉ cần Any() trả về true nghĩa là ĐÃ CÓ người đặt trong khoảng thời gian này
            bool isBooked = await _context.Booking.AnyAsync(b =>
                b.RoomId == request.roomId &&
                b.Status != "Cancelled" && // Bỏ qua các đơn đã hủy
                b.CheckInDate < request.CheckOutDate && // Ngày vào (cũ) phải trước ngày ra (mới)
                request.CheckInDate < b.CheckOutDate    // Ngày vào (mới) phải trước ngày ra (cũ)
            );

            if (isBooked)
            {
                return "Room is already booked for the selected dates.";
            }

            // 4. Tính toán số tiền thực tế
            int totalNights = (request.CheckOutDate - request.CheckInDate).Days;
            // Giả sử bảng Room của bạn có cột Price. Nếu không thì bạn phải sửa đổi tùy theo thiết kế DB
            decimal calculatedTotalAmount = totalNights * room.Price;
            decimal servicesTotal = 0;
            var bookingServices = new List<BookingService>();

            if(request.SelectedServices != null && request.SelectedServices.Any())
            {
                foreach(var service in  request.SelectedServices)
                {
                    var serviceInfo = await _context.Service.FirstOrDefaultAsync(x => x.ServiceId == service.ServiceID);

                    if(serviceInfo == null)
                    {
                        return "service is not exist";
                   
                    }
                    servicesTotal += serviceInfo.Price * service.Quantity;

                    bookingServices.Add(new BookingService()
                    {

                        ServiceId = service.ServiceID,
                        Quantity = service.Quantity,
                        UnitPrice = serviceInfo.Price

                    });

                }
            }
            // 5. Lưu thông tin Booking
            var newBooking = new Booking()
            {
                UserId = user.UserId,
                RoomId = room.RoomId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                TotalAmount = calculatedTotalAmount + servicesTotal, // Lưu tổng tiền thay vì số ngày
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow // Dùng UtcNow chuẩn quốc tế
            };

            await _context.Booking.AddAsync(newBooking);
            await _context.SaveChangesAsync();


            var newPayment = new Payment()
            {
                BookingId = newBooking.BookingId,
                Amount = newBooking.TotalAmount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = "Unpaid",
                TransactionDate = null,
            };

            await _context.Payment.AddAsync(newPayment);
            await _context.SaveChangesAsync();

            foreach(var bs in bookingServices)
            {
                await _context.BookingService.AddAsync(new BookingService()
                {
                    BookingId = newBooking.BookingId,
                    ServiceId = bs.ServiceId,
                    Quantity = bs.Quantity,
                    UnitPrice = bs.UnitPrice
                });
            }

            await _context.SaveChangesAsync();

            return "Booked successfully.";
        }

        public async Task<string?> editBooking(int bookingId,int userId, EditBookingRequestDTO request)
        {
            // LƯU Ý: Đảm bảo sử dụng cùng chuẩn múi giờ với lúc tạo (UtcNow)
            var now = DateTime.UtcNow;

            // 1. Tìm đơn đặt phòng và các thông tin liên quan
            var booking = await _context.Booking.FirstOrDefaultAsync(x => x.BookingId == bookingId && x.UserId == userId);

            if (booking == null)
            {
                return "Booking does not exist.";
            }

            // Không cho phép sửa nếu đã Hủy hoặc đã Check-out
            if (booking.Status == "Cancelled" || booking.Status == "Checked out")
            {
                return "Cannot edit a cancelled or checked-out booking.";
            }

            // 2. Kiểm tra điều kiện thời gian (Phải sửa trước 48h so với lịch Check-in CŨ)
            if ((booking.CheckInDate - now).TotalHours < 48)
            {
                return "You can only edit your booking at least 48 hours before the original check-in date.";
            }

            // 3. Kiểm tra tính hợp lệ của lịch trình MỚI
            if (request.CheckInDate.Date < now.Date)
            {
                return "New check-in date cannot be in the past.";
            }

            if (request.CheckOutDate <= request.CheckInDate)
            {
                return "New check-out date must be after check-in date.";
            }

            // 4. KIỂM TRA TRÙNG LỊCH CHUẨN (Quan trọng: Phải loại trừ chính Booking đang sửa)
            bool isBooked = await _context.Booking.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.BookingId != bookingId &&             // BỎ QUA đơn đang được sửa
                b.Status != "Cancelled" &&              // Bỏ qua đơn đã hủy
                b.CheckInDate < request.CheckOutDate && // Ngày vào (cũ) phải trước ngày ra (mới)
                request.CheckInDate < b.CheckOutDate    // Ngày vào (mới) phải trước ngày ra (cũ)
            );

            if (isBooked)
            {
                return "Room is already booked for the new selected dates.";
            }

            // 5. Tính toán lại số tiền (Tiền phòng + Tiền dịch vụ mới)
            var room = await _context.Room.FirstOrDefaultAsync(x => x.RoomId == booking.RoomId);
            if (room == null) return "Room data is corrupted.";

            int totalNights = (request.CheckOutDate - request.CheckInDate).Days;
            decimal calculatedTotalAmount = totalNights * room.Price;
            decimal servicesTotal = 0;

            var newBookingServices = new List<BookingService>();

            // Xử lý danh sách dịch vụ mới
            if (request.SelectedServices != null && request.SelectedServices.Any())
            {
                foreach (var serviceReq in request.SelectedServices)
                {
                    var serviceInfo = await _context.Service.FirstOrDefaultAsync(x => x.ServiceId == serviceReq.ServiceID);

                    if (serviceInfo == null)
                    {
                        return $"Service with ID {serviceReq.ServiceID} does not exist.";
                    }

                    servicesTotal += serviceInfo.Price * serviceReq.Quantity;

                    newBookingServices.Add(new BookingService()
                    {
                        BookingId = booking.BookingId,
                        ServiceId = serviceReq.ServiceID,
                        Quantity = serviceReq.Quantity,
                        UnitPrice = serviceInfo.Price
                    });
                }
            }

            // ==========================================
            // 6. THỰC THI CẬP NHẬT DATABASE
            // ==========================================
            try
            {
                // A. Cập nhật thông tin Booking
                booking.CheckInDate = request.CheckInDate;
                booking.CheckOutDate = request.CheckOutDate;
                booking.TotalAmount = calculatedTotalAmount + servicesTotal;
                _context.Booking.Update(booking);

                // B. Cập nhật phương thức thanh toán và tổng tiền bên bảng Payment
                var payment = await _context.Payment.FirstOrDefaultAsync(p => p.BookingId == bookingId);
                if (payment != null)
                {
                    payment.PaymentMethod = request.PaymentMethod;
                    payment.Amount = booking.TotalAmount; // Cập nhật lại số tiền phải trả
                    _context.Payment.Update(payment);
                }

                // C. Xóa toàn bộ dịch vụ cũ và Thêm dịch vụ mới
                var oldServices = await _context.BookingService.Where(bs => bs.BookingId == bookingId).ToListAsync();
                if (oldServices.Any())
                {
                    _context.BookingService.RemoveRange(oldServices);
                }

                if (newBookingServices.Any())
                {
                    await _context.BookingService.AddRangeAsync(newBookingServices);
                }

                // D. Lưu toàn bộ thay đổi (Transaction sẽ do EF Core tự quản lý trong 1 lần SaveChanges)
                await _context.SaveChangesAsync();

                return "Booking updated successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi edit booking: {ex.Message}");
                return "An error occurred while updating the booking. Please try again.";
            }
        }

        public async Task<GetDetailBookingResponseDTO> getById(int bookingId, int userId)
        {
            // Giả sử bạn có _context là đối tượng DbContext của Entity Framework
            var bookingDetail = await _context.Booking
                .Where(b => b.BookingId == bookingId && b.UserId == userId) // Hoặc b.BookingId tùy vào tên khóa chính của bạn
                .Select(b => new GetDetailBookingResponseDTO
                {
                    bookingId = b.BookingId,
                    roomId = b.RoomId,
                    branchId = b.Room.BranchId, // Giả sử Room có khóa ngoại trỏ tới Branch
                    roomName = b.Room.RoomType,
                    address = b.Room.Branch.Address,
                    city = b.Room.Branch.City,
                    roomImageUrl = b.Room.ImageUrl, // Hoặc lấy từ b.Room.Images.FirstOrDefault().Url

                    // Thông tin thanh toán
                    PaymentMethod = b.Payment.PaymentMethod, // Hoặc b.Payment.Method nếu tách bảng
                    PaymentStatus = b.Payment.PaymentStatus, // Hoặc b.Payment.Status

                    // Thông tin thời gian & chi phí
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status,
                    bookedAt = b.CreatedAt, // Hoặc b.BookedAt

                    // Lấy danh sách dịch vụ đi kèm
                    selectedServices = b.BookingServices.Select(s => new SelectedServiceViewBookingsResponseDTO
                    {
                        ServiceID = s.ServiceId,
                        ServiceName = s.Service.Name,
                        Quantity = s.Quantity,
                        // Các field khác của SelectedServiceViewBookingsResponseDTO...
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            // Xử lý nếu không tìm thấy đơn đặt phòng
            if (bookingDetail == null)
            {
                throw new Exception($"Không tìm thấy Booking với ID {bookingId}");
                // Hoặc có thể return null tùy thuộc vào logic xử lý lỗi (Exception handling/Result Pattern) của dự án
            }

            return bookingDetail;
        }

        public async Task<bool?> isReviewed( int bookingId)
        {
            if(await _context.Review.AnyAsync(x => x.BookingId == bookingId))
            {
                return true;
            }
            return false;
        }

        public async Task<List<ViewMyBookingsReponseDTO>> viewMyBookings(int userId)
        {
            var res = await _context.Booking
                .Where(b => b.UserId == userId)
                // Sắp xếp ngày tạo từ mới nhất tới cũ nhất
                .OrderByDescending(b => b.CreatedAt)
                .Select(booking => new ViewMyBookingsReponseDTO
                {
                    bookingId = booking.BookingId,
                    CheckInDate = booking.CheckInDate,
                    CheckOutDate = booking.CheckOutDate,
                    TotalAmount = booking.TotalAmount,
                    Status = booking.Status,
                    createdAt = booking.CreatedAt,

                    // Join thủ công lấy thông tin Room
                    roomId = booking.RoomId,
                    roomName = _context.Room.Where(r => r.RoomId == booking.RoomId).Select(r => r.RoomType).FirstOrDefault(),
                    roomImageUrl = _context.Room.Where(r => r.RoomId == booking.RoomId).Select(r => r.ImageUrl).FirstOrDefault(),

                    // THÊM: Map thông tin branchId
                    branchId = _context.Room.Where(r => r.RoomId == booking.RoomId).Select(r => r.BranchId).FirstOrDefault(),

                    // Map thông tin Address và City
                    address = _context.Room.Where(r => r.RoomId == booking.RoomId).Select(r => r.Branch.Address).FirstOrDefault(),
                    city = _context.Room.Where(r => r.RoomId == booking.RoomId).Select(r => r.Branch.City).FirstOrDefault(),


                    // Join thủ công lấy thông tin Payment
                    PaymentMethod = _context.Payment.Where(p => p.BookingId == booking.BookingId).Select(p => p.PaymentMethod).FirstOrDefault(),
                    PaymentStatus = _context.Payment.Where(p => p.BookingId == booking.BookingId).Select(p => p.PaymentStatus).FirstOrDefault(),

                    // LẤY DANH SÁCH SERVICES CÙNG TÊN VÀ SỐ LƯỢNG
                    selectedServices = _context.BookingService
                        .Where(bs => bs.BookingId == booking.BookingId)
                        .Select(bs => new SelectedServiceViewBookingsResponseDTO
                        {
                            ServiceID = bs.ServiceId,

                            // Query thêm vào bảng Service để lấy tên. 
                            ServiceName = _context.Service.Where(s => s.ServiceId == bs.ServiceId).Select(s => s.Name).FirstOrDefault(),

                            Quantity = bs.Quantity
                        }).ToList()
                })
                .ToListAsync();

            return res;
        }


    }
}
