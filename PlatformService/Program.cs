using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace PlatformService;
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        // Craete an httpClient factory
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        // RabbitMQ
        services.AddSingleton<IMessageBusClient, MessageBusClient>();
        // Grpc
        services.AddGrpc();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        IHostingEnvironment env = serviceProvider.GetService<IHostingEnvironment>();

        if (env!.IsProduction())
        {
            Console.WriteLine("--> Using SQLServer DB");
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn"))
            );
        }
        else
        {
            Console.WriteLine("--> Using InMem DB");
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMemory"));
        }

        services.AddEndpointsApiExplorer();

        // CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddControllers();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSwaggerGen();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        Console.WriteLine($"--> CommandService endpoint configuration: {Configuration["CommandService"]}");

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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<GrpcPlatformService>();

            endpoints.MapGet("/protos/platforms.proto", async context =>
            {
                await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
            });
        });

        DbInitializer.Initialize(app, env.IsProduction());
    }
}