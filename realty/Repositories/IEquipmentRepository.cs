using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Repositories
{
    public interface IEquipmentRepository
    {
        Task<List<Equipment>> GetAllAsync();
        Task<Equipment?> GetByIdAsync(int id);

        Task<Equipment> CreateAsync(Equipment equipment);
        Task<bool> UpdateAsync(int id, Equipment equipment);
        Task<bool> DeleteAsync(int id);
    }
}
