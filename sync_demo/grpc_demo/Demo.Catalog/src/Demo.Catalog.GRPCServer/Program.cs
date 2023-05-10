using Demo.Catalog.GRPCServer.Services;
using Demo.Catalog.Service.Entities;
using Demo.Common.MongoDB;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddMongo().AddMongoRepository<Item>("items");

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    var Configuration = builder.Configuration;
    var AllowedOriginSettings = "AllowedOrigin";
    var origine = Configuration[AllowedOriginSettings];
    if (origine != null)
    {
        app.UseCors(builder =>
        {
            builder.WithOrigins(origine)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
// app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
