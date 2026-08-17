using InnGoBackend.DTOs.BlogDTOs;
using InnGoBackend.Models.Context;
using InnGoBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.BlogRepository
{
   
    public class BlogRepository : IBlogRepository
    {
        private readonly InnGoDBContext _context;
        public BlogRepository(InnGoDBContext context)
        {
            _context = context;
        }

            public async Task<bool?> createNewBlog(CreateNewBlogRequestDTO request)
        {
            var newBlog = new Blog()
            {
                Title = request.title,
                Slug = request.slug,
                Summary = request.summary,
                Content = request.content,
                ThumbnailUrl = request.thumbnailUrl,
                Status = "Published",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _context.AddAsync(newBlog);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<getBlogResponseDTO>> getAll()
        {
            var blogs = await _context.Blog.ToListAsync();

            var res = new List<getBlogResponseDTO>();
            foreach(var blog in blogs)
            {
                res.Add(new getBlogResponseDTO()
                {
                    blogId = blog.BlogId,
                    title = blog.Title,
                    slug = blog.Slug,
                    summary = blog.Summary,
                    content = blog.Content,
                    thumbnailUrl = blog.ThumbnailUrl

                });
            }
            return res;
        }

        public async Task<getBlogResponseDTO> GetBlog(int id)
        {
            var blog = await _context.Blog.FirstOrDefaultAsync(x => x.BlogId == id);

            var res = new getBlogResponseDTO();
            if(blog == null)
            {
                return null;
            }
            if(blog != null)
            {
                res.blogId = blog.BlogId;
                res.title = blog.Title;
                res.slug = blog.Slug;
                res.summary = blog.Summary;
                res.content = blog.Content;
                res.thumbnailUrl = blog.ThumbnailUrl;

            }

            return res;
        }
    }
}
