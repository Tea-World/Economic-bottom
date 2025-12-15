using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Repositories
{
    public interface IServiceRequestRepository
    {
        Task<List<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);

        Task<ServiceRequest> CreateAsync(ServiceRequest request);
        Task<bool> UpdateAsync(int id, ServiceRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
