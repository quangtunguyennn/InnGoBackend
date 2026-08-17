using InnGoBackend.Models.Context;
using InnGoHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace InnGoBackend.Repositories.ServiceRepository
{
   
    public class ServiceRepository : IServiceRepository
    {
        private readonly InnGoDBContext _context;
        public ServiceRepository(InnGoDBContext context)
        {
            _context = context;
        }

            public async Task<bool?> CreateNewService(Service service)
        {
            var newService = new Service()
            {
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                ImageUrl = service.ImageUrl,
            };

            await _context.AddAsync(newService);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Service>> getAll()
        {
            return await _context.Service.ToListAsync();
        }

        public async Task<Service?> getById(int id)
        {
            return await _context.Service.FirstOrDefaultAsync(x => x.ServiceId == id);
        }
    }
}
