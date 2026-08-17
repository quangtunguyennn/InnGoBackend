using InnGoBackend.DTOs.BlogDTOs;
using InnGoBackend.Repositories.BlogRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnGoBackend.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase 
    {
        private readonly IBlogRepository _blogRepository;
        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }


        [Authorize(Roles ="Admin")]
        [HttpPost("/api/blog/add")]
        public async Task<IActionResult> addNewBlog(CreateNewBlogRequestDTO request)
        {
            var check = await _blogRepository.createNewBlog(request);

            if(check == true)
            {
                return Ok(new { message = "add new blog successfully" });
            }
            else
            {
                return BadRequest(new { message = "add new blo failed" });
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/blog/getById/{blogId}")]
        public async Task<IActionResult> getById(int blogId)
        {
            var res =  await _blogRepository.GetBlog(blogId);

            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("/api/blog/all")]
        public async Task<IActionResult> getAll()
        {
            var res = await _blogRepository.getAll();
            return Ok(res);
        }
    }
}
