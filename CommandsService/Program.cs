using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddScoped<ICommandRepository, CommandRepository>();
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
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Grpc
builder.Services.AddScoped<IPlatformDataClient,PlatformDataClient>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure CORS
app.UseCors();

// Configure Swagger.
app.UseSwagger(c => 
{
    c.RouteTemplate = "api/c/swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("swagger/v1/swagger.json", "CommandsService");
    c.RoutePrefix = "api/c";
});

app.MapControllers();

// Seeding using Grpc
DbInitializer.Initialize(app);

app.Run();