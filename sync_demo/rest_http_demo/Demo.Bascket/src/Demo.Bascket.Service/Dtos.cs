namespace Demo.Bascket.Service.Dtos
{
    public record GrandItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);

    public record BascketItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);

    public record CatalogItemDto(Guid Id, string Name, string Description);
}