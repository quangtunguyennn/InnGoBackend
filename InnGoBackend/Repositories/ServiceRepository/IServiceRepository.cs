using InnGoHotel.Models;

namespace InnGoBackend.Repositories.ServiceRepository
{
    public interface IServiceRepository
    {
        Task<bool?> CreateNewService(Service service);
        Task<List<Service>> getAll();
        Task<Service?> getById(int id);
    }
}
