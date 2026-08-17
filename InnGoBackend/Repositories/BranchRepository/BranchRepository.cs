using InnGoBackend.DTOs.BranchDTOs;
using InnGoBackend.Models;
using InnGoBackend.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.BranchRepository
{
    public class BranchRepository : IBranchRepository
    {
        private readonly InnGoDBContext _context;
        public BranchRepository(InnGoDBContext context)
        {
            _context = context;
        }
        public async Task<CreateNewBranchResponseDTO> CreateNewBranch(CreateNewBranchRequestDTO request)
        {
            var newBranch = new Branch()
            {
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                Phone = request.Phone,
                Description = request.Description,
                ImageUrl = request.ImageURL
            };

            await _context.AddAsync(newBranch);
            await _context.SaveChangesAsync();

            var res = new CreateNewBranchResponseDTO()
            {
                Name = newBranch.Name,
                Address = newBranch.Address,
                City = newBranch.City,
                Phone = newBranch.Phone,
                Description = newBranch.Description,
                ImageURL = newBranch.ImageUrl
            };
            return res;
        }

        public async Task<Branch> GetBranchById(int id)
        {
            return await _context.Branch.FirstOrDefaultAsync(x => x.BranchId == id);
        }

        public async Task<List<Branch>> GetBranches()
        {
            return await _context.Branch.ToListAsync();
        }
    }
}
