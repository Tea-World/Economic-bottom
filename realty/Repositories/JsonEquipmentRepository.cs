using ServiceDesk.Api.Models;
using ServiceDesk.Api.Storage;

namespace ServiceDesk.Api.Repositories
{
    public class JsonEquipmentRepository : IEquipmentRepository
    {
        private readonly JsonFileStore<Equipment> _store;

        public JsonEquipmentRepository(IConfiguration configuration)
        {
            var filePath = configuration["Storage:EquipmentFile"]
                ?? "Data/equipment.json";

            _store = new JsonFileStore<Equipment>(filePath);
        }

        public async Task<List<Equipment>> GetAllAsync()
            => await _store.ReadAllAsync();

        public async Task<Equipment?> GetByIdAsync(int id)
        {
            var items = await _store.ReadAllAsync();
            return items.FirstOrDefault(e => e.Id == id);
        }

        public async Task<Equipment> CreateAsync(Equipment equipment)
        {
            return await _store.UpdateAsync(items =>
            {
                equipment.Id = items.Any() ? items.Max(e => e.Id) + 1 : 1;
                items.Add(equipment);
                return equipment;
            });
        }

        public async Task<bool> UpdateAsync(int id, Equipment equipment)
        {
            return await _store.UpdateAsync(items =>
            {
                var index = items.FindIndex(e => e.Id == id);
                if (index == -1) return false;

                equipment.Id = id;
                items[index] = equipment;
                return true;
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _store.UpdateAsync(items =>
            {
                var removed = items.RemoveAll(e => e.Id == id);
                return removed > 0;
            });
        }
    }
}
