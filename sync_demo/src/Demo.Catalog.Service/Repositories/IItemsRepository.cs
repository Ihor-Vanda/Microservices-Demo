using Demo.Catalog.Service.Entities;
using MongoDB.Driver;

namespace Demo.Catalog.Service.Repositories
{
    public interface IItemsRepository
    {
        Task CreateAsync(Item entity);
        Task<IReadOnlyCollection<Item>> GetAllAsync();
        Task<Item> GetAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task UpdarteAsync(Item entity);
    }
}