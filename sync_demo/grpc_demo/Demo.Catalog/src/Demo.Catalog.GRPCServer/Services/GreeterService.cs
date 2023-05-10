using Grpc.Core;
using Demo.Common;
using Demo.Catalog.Service.Entities;
using System.Text.Json;
using Demo.Catalog.Service;

namespace Demo.Catalog.GRPCServer.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly IRepository<Item> itemsRepository;
    public GreeterService(IRepository<Item> itemsRepository)
    {
        this.itemsRepository = itemsRepository;
    }

    public override async Task<ServerResponse> Response(ClientRequest request, ServerCallContext context)
    {
        var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());

        return new ServerResponse
        {
            Message = JsonSerializer.Serialize(items)
        };
    }
}
