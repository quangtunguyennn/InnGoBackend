using InnGoBackend.Repositories.ServiceRepository;
using InnGoHotel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnGoBackend.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        public ServiceController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [Authorize(Roles ="Admin, Receptionist")]
        [HttpPost("/api/service/createNewService")]
        public async Task<IActionResult> CreateNewService(Service service)
        {
            bool? check = await _serviceRepository.CreateNewService(service);
            if (check == true)
            {
                return Ok(new { message = "create new service successfully" });
            }
            else
            {
                return BadRequest(new { message = "create new service error" });
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/service/getAll")]
        public async Task<List<Service>> getAll()
        {
            return await _serviceRepository.getAll();
        }

        [AllowAnonymous]
        [HttpGet("/api/service/getById")]
        public async Task<Service?> getById(int id)
        {
            return await _serviceRepository.getById(id);
        }
    }
}
