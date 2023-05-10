using System.Text.Json;
using Confluent.Kafka;
using Demo.Catalog.Contracts;
using Demo.Catalog.Service;
using Demo.Catalog.Service.Dtos;
using Demo.Catalog.Service.Entities;
using Demo.Common;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private readonly string bootstrapServers = "localhost:9092";

        public ItemController(IRepository<Item> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };


            try
            {
                var producer = new ProducerBuilder<Null, string>(config).Build();
                var result = await producer.ProduceAsync("topic-create", new Message<Null, string> { Value = JsonSerializer.Serialize(new CatalogItemCreated(item.Id, item.Name, item.Description)) });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {

            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);

            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };


            try
            {
                var producer = new ProducerBuilder<Null, string>(config).Build();
                var result = await producer.ProduceAsync("topic-update", new Message<Null, string> { Value = JsonSerializer.Serialize(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description)) });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);

            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };


            try
            {
                var producer = new ProducerBuilder<Null, string>(config).Build();
                var result = await producer.ProduceAsync("topic-delete", new Message<Null, string> { Value = JsonSerializer.Serialize(new CatalogItemDeleted(id)) });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}