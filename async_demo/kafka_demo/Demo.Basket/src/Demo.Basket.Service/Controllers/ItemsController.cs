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
        private readonly IRepository<BascketItem> bascketItemRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        public ItemsController(IRepository<BascketItem> itensRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            this.bascketItemRepository = itensRepository;
            this.catalogItemsRepository = catalogItemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BascketItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var bascketItemEntities = await bascketItemRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = bascketItemEntities.Select(item => item.CalatogItemId);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var bascketItemDtos = bascketItemEntities.Select(bascketItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == bascketItem.CalatogItemId);
                return bascketItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(bascketItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto)
        {
            var bascketItem = await bascketItemRepository.GetAsync(
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

                await bascketItemRepository.CreateAsync(bascketItem);
            }
            else
            {
                bascketItem.Quantity += grandItemsDto.Quantity;
                await bascketItemRepository.UpdateAsync(bascketItem);
            }

            return Ok();
        }
    }
}