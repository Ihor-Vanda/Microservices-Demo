using Sync.Demo.Service.Dtos;
using Sync.Demo.Service.Entities;

namespace Sync.Demo.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}