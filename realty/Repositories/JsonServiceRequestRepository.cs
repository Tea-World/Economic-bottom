using ServiceDesk.Api.Models;
using ServiceDesk.Api.Storage;

namespace ServiceDesk.Api.Repositories
{
    public class JsonServiceRequestRepository : IServiceRequestRepository
    {
        private readonly JsonFileStore<ServiceRequest> _store;

        public JsonServiceRequestRepository(IConfiguration configuration)
        {
            var filePath = configuration["Storage:ServiceRequestsFile"]
                ?? "Data/serviceRequests.json";

            _store = new JsonFileStore<ServiceRequest>(filePath);
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
            => await _store.ReadAllAsync();

        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            var items = await _store.ReadAllAsync();
            return items.FirstOrDefault(r => r.Id == id);
        }

        public async Task<ServiceRequest> CreateAsync(ServiceRequest request)
        {
            return await _store.UpdateAsync(items =>
            {
                request.Id = items.Any() ? items.Max(r => r.Id) + 1 : 1;
                request.CreatedAt = DateTime.UtcNow;
                items.Add(request);
                return request;
            });
        }

        public async Task<bool> UpdateAsync(int id, ServiceRequest request)
        {
            return await _store.UpdateAsync(items =>
            {
                var index = items.FindIndex(r => r.Id == id);
                if (index == -1) return false;

                request.Id = id;
                items[index] = request;
                return true;
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _store.UpdateAsync(items =>
            {
                var removed = items.RemoveAll(r => r.Id == id);
                return removed > 0;
            });
        }
    }
}
