using InnGoBackend.DTOs.BranchDTOs;
using InnGoBackend.Models;
using InnGoBackend.Repositories.BranchRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnGoBackend.Controllers
{
    [Route("/api[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        public BranchController(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("/api/branch/createNewBranch")]
        public async Task<CreateNewBranchResponseDTO> CreateNewBranch(CreateNewBranchRequestDTO request)
        {
            return await _branchRepository.CreateNewBranch(request);
        }

        [AllowAnonymous]
        [HttpGet("/api/branch/getAll")]
        public async Task<List<Branch>> GetAll()
        {
            return await _branchRepository.GetBranches();
        }

        [AllowAnonymous]
        [HttpGet("/api/branch/getById")]
        public async Task<Branch> GetById(int id)
        {
            return await _branchRepository.GetBranchById(id);
        }
    }
}
