using InnGoBackend.DTOs.RoomDTOs;
using InnGoHotel.Models;

namespace InnGoBackend.Repositories.RoomRepository
{
    public interface IRoomRepository
    {
        Task<bool> CreateNewRoom(CreateNewRoomRequestDTO request);
        Task<Room> getRoomById(int id);
        Task<List<Room>> getAllByBranchId(int branchId);
        Task<List<Room>> GetAllRooms();
        Task<List<Room>?> searchRooms(SearchRoomRequestDTO request);
        Task<decimal?> getAverageRating(int roomId);
    }
}
