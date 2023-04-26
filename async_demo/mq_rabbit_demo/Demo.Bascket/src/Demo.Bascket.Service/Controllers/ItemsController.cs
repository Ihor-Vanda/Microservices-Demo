using Demo.Bascket.Service.Clients;
using Demo.Bascket.Service.Dtos;
using Demo.Bascket.Service.Entities;
using Demo.Common;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Bascket.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<BascketItem> itensRepository;
        private readonly CatalogClient catalogClient;

        public ItemsController(IRepository<BascketItem> itensRepository, CatalogClient catalogClient)
        {
            this.itensRepository = itensRepository;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BascketItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogClient.GetCalatogItemsAsync();
            var bascketItemEntities = await itensRepository.GetAllAsync(item => item.UserId == userId);

            var bascketItemDtos = bascketItemEntities.Select(bascketItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == bascketItem.CalatogItemId);
                return bascketItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(bascketItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto)
        {
            var bascketItem = await itensRepository.GetAsync(
                item => item.UserId == grandItemsDto.UserId && item.CalatogItemId == grandItemsDto.CatalogItemId);

            if (bascketItem == null)
            {
                bascketItem = new BascketItem
                {
                    CalatogItemId = grandItemsDto.CatalogItemId,
                    UserId = grandItemsDto.UserId,
                    Quantity = grandItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await itensRepository.CreateAsync(bascketItem);
            }
            else
            {
                bascketItem.Quantity += grandItemsDto.Quantity;
                await itensRepository.UpdateAsync(bascketItem);
            }

            return Ok();
        }
    }
}