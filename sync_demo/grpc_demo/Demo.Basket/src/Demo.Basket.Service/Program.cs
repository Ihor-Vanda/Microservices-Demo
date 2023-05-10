using Demo.Bascket.Service.Entities;
using Demo.Common.MongoDB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo().AddMongoRepository<BascketItem>("bascketitems");

// builder.Services.AddHttpClient<CatalogClient>(client =>
// {
//     client.BaseAddress = new Uri("http://localhost:5124");
// })

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
