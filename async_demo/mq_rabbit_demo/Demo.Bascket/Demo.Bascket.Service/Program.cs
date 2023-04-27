using Demo.Bascket.Service.Entities;
using Demo.Catalog.MassTransit;
using Demo.Common.MongoDB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo()
    .AddMongoRepository<BascketItem>("bascketitems")
    .AddMongoRepository<CatalogItem>("catalogitems")
    .AddMassTransitWithRabbitMq();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var Configuration = builder.Configuration;
    var AllowedOriginSettings = "AllowedOrigin";
    app.UseCors(builder =>
    {
        builder.WithOrigins(Configuration[AllowedOriginSettings])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
