using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
// Craete an httpClient factory
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
// RabbitMQ
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();


if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQLServer DB");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"))
    );
}
else
{
    Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemory"));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();

var app = builder.Build();
Console.WriteLine($"--> CommandService endpoint configuration: {app.Configuration["CommandService"]}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

DbInitializer.Initialize(app, app.Environment.IsProduction());

app.Run();