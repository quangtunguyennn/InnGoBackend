using InnGoBackend.DTOs.BranchDTOs;
using InnGoBackend.Models;

namespace InnGoBackend.Repositories.BranchRepository
{
    public interface IBranchRepository
    {
       
        public Task<CreateNewBranchResponseDTO> CreateNewBranch(CreateNewBranchRequestDTO request);
        public Task<List<Branch>> GetBranches();
  
        public Task<Branch> GetBranchById(int id);
    }
}
