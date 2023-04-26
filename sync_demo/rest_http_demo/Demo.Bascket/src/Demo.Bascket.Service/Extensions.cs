using Demo.Bascket.Service.Dtos;
using Demo.Bascket.Service.Entities;

namespace Demo.Bascket.Service
{
    public static class Extensions
    {
        public static BascketItemDto AsDto(this BascketItem item, string Name, string Description)
        {
            return new BascketItemDto(item.CalatogItemId, Name, Description, item.Quantity, item.AcquiredDate);
        }
    }
}