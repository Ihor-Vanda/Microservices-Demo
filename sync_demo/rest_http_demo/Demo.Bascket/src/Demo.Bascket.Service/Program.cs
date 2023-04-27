using Demo.Bascket.Service.Clients;
using Demo.Bascket.Service.Entities;
using Demo.Common.MongoDB;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo().AddMongoRepository<BascketItem>("bascketitems");

Random jitter = new Random();

builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5124");
})
.AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    5,
    retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp))
                    + TimeSpan.FromMilliseconds(jitter.Next(0, 1000))
))
.AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    3,
    TimeSpan.FromSeconds(15)
))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

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
