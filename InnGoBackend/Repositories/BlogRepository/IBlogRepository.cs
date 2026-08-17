using InnGoBackend.DTOs.BlogDTOs;
using InnGoBackend.Models.Entities;

namespace InnGoBackend.Repositories.BlogRepository
{
    public interface IBlogRepository
    {
        Task<bool?> createNewBlog(CreateNewBlogRequestDTO request);
        Task<List<getBlogResponseDTO>> getAll();
        Task<getBlogResponseDTO> GetBlog(int id);
    }
}
