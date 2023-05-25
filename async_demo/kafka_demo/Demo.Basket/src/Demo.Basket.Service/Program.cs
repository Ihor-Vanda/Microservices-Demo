using Demo.Bascket;
using Demo.Bascket.Service.Entities;
using Demo.Common;
using Demo.Common.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo()
    .AddMongoRepository<BascketItem>("basketitems")
    .AddMongoRepository<CatalogItem>("catalogitems");

builder.Services.AddHostedService<CatalogItemCreatedConsumer>();
builder.Services.AddHostedService<CatalogItemUpdatedConsumer>();
builder.Services.AddHostedService<CatalogItemDeletedConsumer>();

// Register the consumers as scoped services
// builder.Services.AddScoped<CatalogItemCreatedConsumer>();
// builder.Services.AddScoped<CatalogItemUpdatedConsumer>();
// builder.Services.AddScoped<CatalogItemDeletedConsumer>();

// Register an all-in-one service that depends on all the consumers
// builder.Services.AddSingleton<IHostedService, Consumer>();


// builder.Services.AddSingleton<IHostedService, CatalogItemCreatedConsumer>();
// builder.Services.AddSingleton<IHostedService, CatalogItemUpdatedConsumer>();
// builder.Services.AddSingleton<IHostedService, CatalogItemDeletedConsumer>();

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
    var origin = Configuration[AllowedOriginSettings];
    if (origin != null)
    {
        app.UseCors(builder =>
        {
            builder.WithOrigins(origin)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
