using Demo.Bascket.Service.Dtos;
using Demo.Bascket.Service.Entities;
using Demo.Common;
using Demo.Basket.GRPCClient;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Demo.Bascket.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<BascketItem> itemsRepository;

        public ItemsController(IRepository<BascketItem> itensRepository)
        {
            this.itemsRepository = itensRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BascketItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = JsonSerializer.Deserialize<IReadOnlyCollection<CatalogItemDto>>(await Client.GetAllAsync());
            var bascketItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);

            if (catalogItems != null)
            {
                var bascketItemDtos = bascketItemEntities.Select(bascketItem =>
                {
                    var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == bascketItem.CalatogItemId);
                    return bascketItem.AsDto(catalogItem.Name, catalogItem.Description);
                });

                return Ok(bascketItemDtos);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto)
        {
            var bascketItem = await itemsRepository.GetAsync(
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

                await itemsRepository.CreateAsync(bascketItem);
            }
            else
            {
                bascketItem.Quantity += grandItemsDto.Quantity;
                await itemsRepository.UpdateAsync(bascketItem);
            }

            return Ok();
        }
    }
}