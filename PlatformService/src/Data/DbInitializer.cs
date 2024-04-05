using PlatformService.Models;

namespace PlatformService.Data;

public static class DbInitializer
{
  public static void Initialize(IApplicationBuilder applicationBuilder)
  {
    using( var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
    {
      SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
    }
  }

  private static void SeedData(AppDbContext? context)
  {
    if(context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if(!context.Platforms.Any())
    {
      Console.WriteLine("--> Seeding Data");
      context.Platforms.AddRange(
        new Platform() { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
        new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
        new Platform() { Name = "Kubernates", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
      );

      context.SaveChanges();
    }
    else
    {
      Console.WriteLine("--> We already have data in DB");
    }
  }
}