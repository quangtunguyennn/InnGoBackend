using InnGoBackend.DTOs.RoomDTOs;
using InnGoBackend.Repositories.RoomRepository;
using InnGoHotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnGoBackend.Controllers
{
    [Route("/api[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [Authorize(Roles ="Admin, Receptionist")]
        [HttpPost("/api/room/createNewRoom")]
        public async Task<IActionResult> CreateNewRoom(CreateNewRoomRequestDTO request)
        {
            if(await _roomRepository.CreateNewRoom(request))
            {
                return Ok(new { message = "create new room successfully" });
            }
            else
            {
                return BadRequest(new { message = "create failed" });
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/room/getAll")]
        public async Task<List<Room>> GetAll()
        {
            return await _roomRepository.GetAllRooms();
        }

        [AllowAnonymous]
        [HttpGet("/api/room/getById")]
        public async Task<Room> getById(int id)
        {
            return await _roomRepository.getRoomById(id);
        }

        [AllowAnonymous]
        [HttpGet("/api/room/getRoomsByBranchId")]
        public async Task<List<Room>> getRoomsByBranchId(int branchId)
        {
            return await _roomRepository.getAllByBranchId(branchId);
        }

        [AllowAnonymous]
        [HttpGet("/api/room/search")]
        public async Task<List<Room>?> searchRooms([FromQuery]SearchRoomRequestDTO request)
        {
            return await _roomRepository.searchRooms(request);
        }

        [AllowAnonymous]
        [HttpGet("/api/room/avgRating")]
        public async Task<decimal?> getAvgRating(int roomId)
        {
            var res=  await _roomRepository.getAverageRating(roomId);
            return res;
        }
    }
}
