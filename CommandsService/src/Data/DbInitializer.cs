using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public static class DbInitializer
{
  public static void Initialize(IApplicationBuilder applicationBuilder)
  {
    using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
    {
      var grpcClient =  serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
      var platforms = grpcClient!.ReturnAllPlatforms();
      var commandRepository = serviceScope.ServiceProvider.GetService<ICommandRepository>();
      
      SeedData(commandRepository, platforms);
    }
  }

  private static void SeedData(ICommandRepository? commandRepository, IEnumerable<Platform>? platforms)
  {
    if(commandRepository == null)
    {
      throw new ArgumentNullException(nameof(commandRepository));
    }

    if(platforms == null)
    {
      throw new ArgumentNullException(nameof(platforms));
    }

    Console.WriteLine("--> Grpc: Seeding new platforms");
    
    foreach(var platform in platforms)
    {
      if(!commandRepository.ExternalPlatformExists(platform.ExternalId))
      {
        commandRepository.CreatePlatform(platform);
      }

      commandRepository.SaveChanges();
    }
  }
}