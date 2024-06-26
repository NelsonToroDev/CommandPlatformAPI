using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
// Craete an httpClient factory
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
// RabbitMQ
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
// Grpc
builder.Services.AddGrpc();

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

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();

var app = builder.Build();
Console.WriteLine($"--> CommandService endpoint configuration: {app.Configuration["CommandService"]}");

// Configure CORS
app.UseCors();

// Configure Swagger.
app.UseSwagger(c => 
{
    c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("swagger/v1/swagger.json", "PlatformService");
    c.RoutePrefix = "api";
});

//app.UseHttpsRedirection();

app.MapControllers();

// Grpc Mapping endpoint
app.MapGrpcService<GrpcPlatformService>();

// Publish the proto contract just as a good practice
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

DbInitializer.Initialize(app, app.Environment.IsProduction());

app.Run();